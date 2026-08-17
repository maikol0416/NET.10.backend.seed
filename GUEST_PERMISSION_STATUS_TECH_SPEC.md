# Spec técnica — Desactivar GuestPermission (para implementación)

## Contexto

El front de administración (Angular) para invitados pasó a ser **solo lectura respecto a
la creación de permisos**: los permisos (`GuestPermission`) los otorga otra app (móvil), no
el backoffice web. Desde el backoffice solo se necesita:

1. Listar los permisos de un invitado, indicando propiedad, apartamento y horario. ✅ Ya
   soportado (`physicalStructureName`/`apartmentNumber` resueltos en `getById`/`getAll`/
   `getPaginated`, ver `GUEST_API_DOCUMENTATION.md`).
2. **Eliminar** un permiso. ✅ Ya funciona hoy reenviando el array completo de
   `guestPermissions` sin el permiso descartado vía `PUT /api/guest/update` (probado
   manualmente, funciona).
3. **Desactivar/activar** un permiso sin eliminarlo (para conservar el historial pero que
   dicho permiso deje de considerarse vigente). ❌ **No soportado hoy** — es el objeto de
   este documento.

## Lo que ya existe y se puede reutilizar

`GuestPermissionEntity` (`Domain/BoundedContext/People/Aggregates/GuestPermissionEntity.cs`)
hereda de `Entity` (`Domain/DomainShared/Entity.cs`), que **ya tiene** una propiedad
`Status` (string, inicializada a `StatusEnum.Active.ToString()` en el constructor) y el
enum `Domain/DomainShared/StatusEnum.cs` ya define `Disabled = 0`. El mapeo EF Core
(`Infraestructure/Entity/Context/EntityConfigurations/GuestConfig.cs`, dentro del
`OwnsMany(p => p.GuestPermissions, ...)`) **ya persiste `Status`** en la tabla
`GuestPermission` (`guestPermissionBuilder.Property(t => t.Status).IsRequired()...`).

**No se necesita migración de base de datos** — la columna ya existe. El trabajo es 100%
de dominio/aplicación/API.

## ⚠️ Bug prerequisito que hay que arreglar primero

Encontrado probando manualmente el flujo de "eliminar" desde el front: cualquier `PUT
/api/guest/update` que toque `guestPermissions` (que es **todas** las ediciones desde el
front actual, porque siempre reenvía el array completo de permisos vigentes, los toque o
no) **regenera el `Id` de cada permiso y le resetea el `Status` a `Active`**, incluso para
los permisos que el front no modificó.

Causa raíz, en `Domain/BoundedContext/People/Aggregates/GuestAgg.cs`:

```csharp
public void UpdateGuestPermissions(IEnumerable<GuestPermissionEntity> incomingGuestPermissions)
{
    GuestPermissions.Clear();
    if (incomingGuestPermissions != null)
    {
        foreach (var incomingGuestPermission in incomingGuestPermissions)
        {
            GuestPermissions.Add(new GuestPermissionEntity(incomingGuestPermission.StartDate, incomingGuestPermission.EndDate,
                incomingGuestPermission.PhysicalStructureId, incomingGuestPermission.ApartmentId));
        }
    }
}
```

Esto **ignora el `Id` entrante** (usa siempre el constructor sin `Id`, que genera uno
nuevo vía `Entity()`) y no preserva ningún estado existente — cada permiso "sobreviviente"
de un update se destruye y se vuelve a crear desde cero.

Confirmado empíricamente: se creó un `Guest` con un permiso (`Id` conocido), luego se
editó *solo el nombre* del guest desde el front (sin tocar permisos) y, al releer el
guest, el permiso seguía existiendo pero — de haber tenido un `Id` o `Status` distinto al
default — se habría perdido. Este bug es el motivo por el que **hay que arreglarlo antes
o junto con la feature de desactivar**: si no, cualquier desactivación se revertiría solas
en el siguiente `update` del guest que toque permisos (por ejemplo, al editar el teléfono
del invitado), porque el permiso se reconstruye con `Status = Active` de nuevo.

**Fix propuesto:** en `UpdateGuestPermissions`, reconciliar por `Id` en vez de
destruir-y-recrear todo:

```csharp
public void UpdateGuestPermissions(IEnumerable<GuestPermissionEntity> incomingGuestPermissions)
{
    var incoming = (incomingGuestPermissions ?? Enumerable.Empty<GuestPermissionEntity>()).ToList();
    var incomingIds = incoming.Where(p => p.Id != Guid.Empty).Select(p => p.Id).ToHashSet();

    // Elimina los que ya no vienen en el array (mismo comportamiento actual para "eliminar")
    GuestPermissions.RemoveAll(existing => !incomingIds.Contains(existing.Id));

    foreach (var incomingPermission in incoming)
    {
        var existing = GuestPermissions.FirstOrDefault(p => p.Id == incomingPermission.Id);
        if (existing != null)
        {
            // Ya existe: actualiza campos mutables in-place, preserva Id y Status.
            existing.Update(incomingPermission.StartDate, incomingPermission.EndDate,
                incomingPermission.PhysicalStructureId, incomingPermission.ApartmentId);
        }
        else
        {
            // Permiso nuevo (no debería ocurrir desde el front actual, que no crea
            // permisos, pero se mantiene por si otro cliente sí lo hace).
            GuestPermissions.Add(new GuestPermissionEntity(incomingPermission.StartDate, incomingPermission.EndDate,
                incomingPermission.PhysicalStructureId, incomingPermission.ApartmentId));
        }
    }
}
```

(`GuestPermissionEntity.Update(...)` ya existe y no toca `Status`, así que reutilizarlo es
seguro.)

## Cambios requeridos

### 1. Dominio — `GuestPermissionEntity`

Agregar métodos de negocio para cambiar el estado (no existen hoy):

```csharp
public bool IsActive => Status == StatusEnum.Active.ToString();

public void Deactivate()
{
    Status = StatusEnum.Disabled.ToString();
    UpdateAt = DateTime.UtcNow;
}

public void Activate()
{
    Status = StatusEnum.Active.ToString();
    UpdateAt = DateTime.UtcNow;
}
```

(`Status`/`UpdateAt` tienen setter `protected` en `Entity` — al estar `GuestPermissionEntity`
en el mismo árbol de herencia puede asignarlos directamente.)

### 2. Aplicación — DTO

`Application/Guest/Dtos/GuestPermissionDto.cs` — agregar:

```csharp
/// <summary>
/// true = permiso activo/vigente para control de acceso, false = desactivado.
/// Solo lectura en create/update masivo (ver mapeo abajo); se cambia con los
/// endpoints dedicados de activar/desactivar.
/// </summary>
public bool IsActive { get; set; } = true;
```

### 3. Aplicación — Mapper

`Application/Guest/Mapper/GuestMapper.cs`:

- En `GuestAgg -> GuestDto` (lectura): agregar `IsActive = gp.IsActive` al `Select` de
  `GuestPermissions`.
- En `GuestDto -> GuestAgg` (create/update): **no leer `IsActive` del DTO entrante** — el
  estado no se controla desde `create`/`update` masivo, solo desde los endpoints
  dedicados de abajo. Así se evita que alguien reactive/desactive por accidente
  reenviando el array completo con un `isActive` desactualizado.

### 4. API — endpoints dedicados

En vez de forzar el activar/desactivar por el `PUT /api/guest/update` (que reemplaza todo
el array y no da un lugar natural para una acción puntual sobre *un* permiso), agregar dos
endpoints chicos y explícitos, en `GuestController` (o uno nuevo si se prefiere separar
por sub-recurso):

```
PATCH /api/guest/permissions/{permissionId}/deactivate
PATCH /api/guest/permissions/{permissionId}/activate
```

**Headers:** `Authorization: Bearer {token}`, sin body.

**Response (200 OK):**
```json
{
  "data": {
    "id": "7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5g",
    "startDate": "2026-07-10T10:00:00Z",
    "endDate": "2026-07-15T10:00:00Z",
    "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
    "apartmentId": null,
    "physicalStructureName": "Torres del Parque",
    "apartmentNumber": null,
    "isActive": false
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

**Errores:**
- `404` si `permissionId` no existe.
- `400`/texto plano de dominio si ya está en el estado pedido (opcional — también es
  válido que sea idempotente y devuelva 200 sin cambios).

Implementación: nuevo `Command`/`Handler` (o método directo en `IGuestService`, según el
patrón CQRS que ya usa el resto del proyecto) que:
1. Carga el `Guest` dueño del permiso (o el permiso directamente si el repositorio lo
   permite).
2. Llama a `guestPermission.Deactivate()` / `.Activate()`.
3. Guarda cambios.
4. Devuelve el `GuestPermissionDto` actualizado (con `physicalStructureName`/
   `apartmentNumber` resueltos, igual que en `getById`).

### 5. Filtrado en listados (a decidir con negocio)

Hoy `getAll`/`getPaginated`/`getById` devuelven **todos** los permisos sin filtrar por
estado. Al agregar `isActive`, decidir si:
- (a) se sigue devolviendo todo y el front decide qué mostrar/atenuar visualmente según
  `isActive` (recomendado — mantiene el historial completo visible, que es justamente lo
  que pidió el front), o
- (b) se agrega un filtro opcional (`?includeInactive=true/false`) a los endpoints de
  listado.

El front actual (ya implementado) espera **(a)**: listar todo, mostrar estado con badge.

## Qué asume el front (ya implementado, a la espera de este cambio)

- `GuestPermissionsModal` (`src/app/features/people/guests/components/guest-permissions-modal/`)
  ya lista `physicalStructureName`, `apartmentNumber`, fechas y un botón **Eliminar**
  funcional (vía `PUT /update` reenviando el array sin ese permiso).
- **No** se agregó todavía un botón "Desactivar/Activar" — se deja pendiente a propósito
  hasta que este endpoint exista, para no dejar UI a medio terminar. Una vez implementado
  el backend, agregar en el front:
  - `isActive: boolean` a `GuestPermission`/`GuestPermissionDto` (modelos en
    `src/app/features/people/guests/models/`).
  - Un botón "Desactivar"/"Activar" en `guest-permissions-modal.html` que llame a
    `GuestService.deactivatePermission(id)` / `.activatePermission(id)` (nuevos métodos,
    `PATCH` a los endpoints de arriba).

## Opcional / no bloqueante

El pedido original también mencionaba mostrar "en qué hora se dio" el permiso. Hoy el
front usa `startDate` (inicio de vigencia) para eso, porque `GuestPermissionDto` no expone
la fecha real de creación (`CreatedAt`, ya existe en la entidad vía `Entity` pero no está
en el DTO). Si se quiere mostrar el momento exacto en que se otorgó (no el inicio de
vigencia), exponer `CreatedAt` como `grantedAt` en `GuestPermissionDto` — mismo patrón que
`IsActive`, sin migración porque la columna ya existe.
