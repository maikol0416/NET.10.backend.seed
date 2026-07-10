# Physical Structure API — Documentación técnica del cambio (Multi-tenant)

## URL Base
```
http://localhost:5296/api/physicalstructure
```

## Contexto

Este documento cubre **solo el cambio nuevo**: `PhysicalStructure` ahora está atada a una empresa (`companyId`), siguiendo el mismo patrón multi-tenant que ya se aplicó a `login`/`register`/usuarios (ver `AUTH_API_DOCUMENTATION.md`). El resto del contrato de esta API (`create`, `update`, `delete`, `getAll`, `getById`, `getPaginated`, y todos los campos existentes de torres/apartamentos/áreas comunes) **no cambió** — front sigue usándolo igual.

Relación de negocio: **una empresa puede tener muchas estructuras físicas; una estructura física pertenece a exactamente una empresa**, y esa pertenencia es **inmutable** una vez creada (no existe un endpoint para "trasladar" una estructura de una empresa a otra).

---

## ⚠️ Lo más importante — leer antes de tocar código

1. **`companyId` es de solo lectura para el front.** El backend **siempre** lo sobrescribe con la empresa del usuario autenticado al crear o actualizar — cualquier valor que mandes en `companyId` en `create`/`update` **se ignora**. No agregues un input editable para este campo en los formularios de creación/edición.
2. **`getAll`/`getById`/`getPaginated` ahora filtran automáticamente por empresa.** Antes de este cambio, cualquier usuario autenticado veía **todas** las estructuras del sistema. Ahora:
   - Un usuario normal (atado a una empresa) solo ve las estructuras **de su propia empresa**.
   - Un usuario `Administrator` (rol de plataforma, sin empresa) las ve **todas**.
   - Esto es transparente para el front — no hay que mandar ningún filtro nuevo, el backend lo resuelve solo con el token. Pero **si tenías datos de prueba visibles para todos los usuarios antes, ahora pueden desaparecer** de la vista de usuarios normales (ver punto 4).
3. **Nuevo caso de error: un `Administrator` ya NO puede crear ni editar estructuras físicas.** Antes de este cambio, cualquier usuario autenticado (incluido `Administrator`) podía crear/editar. Ahora, como `Administrator` no pertenece a ninguna empresa, `create`/`update` le devuelven `400`: *"Tu usuario no pertenece a ninguna empresa; no puedes crear ni modificar este recurso."* — si tu UI permite que un `Administrator` gestione estructuras físicas, oculta o deshabilita esa opción para ese rol, o maneja el error explícitamente.
4. **Dato importante sobre estructuras creadas antes de este cambio:** cualquier estructura física que ya existiera en la base de datos antes de esta migración quedó con `companyId` vacío (`00000000-0000-0000-0000-000000000000`). Eso significa que **ya no es visible para ningún usuario normal** (solo para `Administrator`, que ve todo). Si tenías datos de prueba de sesiones anteriores, es esperable que la lista de un usuario normal aparezca vacía hasta que se creen estructuras nuevas (que sí quedan correctamente atadas a su empresa).

---

## Qué se agregó / qué se ajustó / qué NO cambió

| Elemento | Estado | Detalle |
|---|---|---|
| `PhysicalStructureDto.companyId` | 🆕 AGREGADO | `string \| null`. Visible en todas las respuestas (`create`, `update`, `getById`, `getAll`, `getPaginated`). |
| Body de `POST /create` y `PUT /update` | 🟡 AJUSTADO (comportamiento, no forma) | Sigue aceptando `companyId` si lo mandas (no rompe si lo omites), pero **se ignora siempre** — el backend nunca confía en el valor del cliente. |
| `GET /getAll`, `GET /getById`, `GET /getPaginated` | 🟡 AJUSTADO (comportamiento) | Ahora filtran por empresa automáticamente. Mismo endpoint, mismos parámetros, **resultados distintos** según quién consulta. |
| Estructura de `towers`, `apartments`, `commonAreas`, `location` | ⚪ SIN CAMBIOS | Ningún campo existente se quitó ni se renombró. |
| Endpoints (`create`/`update`/`delete`/`getAll`/`getById`/`getPaginated`) | ⚪ SIN CAMBIOS | Mismas rutas, mismos verbos HTTP. |
| Autorización (`[Authorize]`, sin restricción de rol) | ⚪ SIN CAMBIOS a nivel de atributo | Pero en la práctica, `Administrator` ahora recibe error de negocio al crear/editar (ver punto 3 arriba) — la restricción real está en el handler, no en el atributo (mismo patrón que `register` en Auth). |

---

## 1. CREATE (POST) — 🟡 comportamiento ajustado
**Endpoint:** `POST /api/physicalstructure/create`
**Auth:** `Bearer {token}` (cualquier usuario autenticado, salvo `Administrator` — ver caso de error abajo)

**Body:** igual que antes. Si incluyes `companyId`, se ignora.

```json
{
  "name": "Edificio Central Medellín",
  "nit": "890123456-7",
  "unitCount": 24,
  "number": "Cra 45 # 50-20",
  "detailLocation": "Apto 1001",
  "country": "Colombia",
  "city": "Medellín",
  "neighborhood": "Laureles",
  "commonAreas": [],
  "towers": []
}
```

**cURL:**
```bash
curl -X POST http://localhost:5296/api/physicalstructure/create \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "name": "Edificio Central Medellín",
    "nit": "890123456-7",
    "unitCount": 24,
    "number": "Cra 45 # 50-20",
    "detailLocation": "Apto 1001",
    "country": "Colombia",
    "city": "Medellín",
    "neighborhood": "Laureles",
    "commonAreas": [],
    "towers": []
  }'
```

**Response (200 OK) — 🆕 `companyId` en la respuesta:**
```json
{
  "data": {
    "id": "8f5a9c1d-....",
    "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
    "name": "Edificio Central Medellín",
    "nit": "890123456-7",
    "unitCount": 24,
    "number": "Cra 45 # 50-20",
    "detailLocation": "Apto 1001",
    "country": "Colombia",
    "city": "Medellín",
    "neighborhood": "Laureles",
    "commonAreas": [],
    "towers": []
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```
El `companyId` que vuelve es siempre el de la empresa del usuario que hizo la llamada — **nunca** el que se haya mandado en el request.

**🆕 Nuevo error de negocio (400):**
```json
{ "status": false, "message": "Tu usuario no pertenece a ninguna empresa; no puedes crear ni modificar este recurso." }
```
Ocurre cuando quien llama es un `Administrator` (o, en un caso anómalo, cualquier usuario sin empresa asignada).

---

## 2. UPDATE (PUT) — 🟡 comportamiento ajustado
**Endpoint:** `PUT /api/physicalstructure/update`
**Auth:** `Bearer {token}` (mismas reglas que create)

Igual que `create`: si mandas `companyId`, se ignora — el backend lo mantiene igual al que ya tenía la estructura (recuerda: es inmutable, no se puede "trasladar" a otra empresa desde este endpoint).

**cURL:**
```bash
curl -X PUT http://localhost:5296/api/physicalstructure/update \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "id": "8f5a9c1d-....",
    "name": "Edificio Central Medellín (renovado)",
    "nit": "890123456-7",
    "unitCount": 24,
    "number": "Cra 45 # 50-20",
    "detailLocation": "Apto 1001",
    "country": "Colombia",
    "city": "Medellín",
    "neighborhood": "Laureles",
    "commonAreas": [],
    "towers": []
  }'
```

**Nota de seguridad (efecto colateral del filtro por empresa):** si un usuario intenta actualizar el `id` de una estructura que pertenece a **otra** empresa, el backend la trata como si no existiera (el filtro de tenant la excluye de la búsqueda) y devuelve el error genérico de "no encontrado" — no hay forma de editar ni de enterarte de que existe una estructura de otra empresa con ese Id.

---

## 3. GET BY ID / GET ALL / GET PAGINATED — 🟡 comportamiento ajustado
**Endpoints:** `GET /getById?id={id}`, `GET /getAll`, `GET /getPaginated?pageNumber=1&pageSize=10`
**Auth:** `Bearer {token}`

Mismos parámetros y forma de respuesta que antes, **más el campo `companyId`** en cada estructura. La diferencia real es el **filtrado automático**:

```bash
curl -X GET "http://localhost:5296/api/physicalstructure/getPaginated?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}"
```

```json
{
  "data": {
    "items": [
      {
        "id": "8f5a9c1d-....",
        "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
        "name": "Edificio Central Medellín",
        "...": "resto de campos sin cambios"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```

- Si el usuario logueado pertenece a la empresa `3f2f1a2b-...`, solo ve estructuras con ese `companyId`.
- Si el usuario logueado es `Administrator`, ve estructuras de **todas** las empresas — útil para una vista de soporte/backoffice, si la necesitas.
- `getById` de una estructura de otra empresa devuelve **no encontrado**, no un `403` — no reveles al front que la estructura existe pero no es accesible; simplemente no aparece.

---

## Estructura de objetos (TypeScript)

```typescript
interface PhysicalStructureDto {
  id?: string;
  companyId?: string | null;   // 🆕 — solo lectura desde el front, el backend lo controla
  name: string;
  nit: string;
  unitCount: number;
  number: string;
  detailLocation: string;
  country: string;
  city: string;
  neighborhood: string;
  commonAreas: CommonAreaDto[];
  towers: TowerDto[];
  // ... el resto de campos de towers/apartments no cambió
}
```

---

## Checklist para el front

- [ ] Mostrar `companyId` (resuelto a nombre de empresa vía `GET /managementcompany/getAll`) en el listado/detalle de estructuras, **solo como dato informativo** — nunca como campo editable en los formularios de create/update.
- [ ] Si algún flujo de `Administrator` permite crear/editar estructuras físicas, ocultarlo o mostrar un mensaje claro — ya no es una operación soportada para ese rol.
- [ ] No asumas que `getAll`/`getPaginated` devuelve el mismo listado para todos los usuarios — cada empresa ve solo lo suyo.
- [ ] Si estabas usando datos de prueba creados antes de este cambio, espera verlos desaparecer de la vista de usuarios normales (quedaron con empresa vacía) — créalos de nuevo con una sesión de usuario perteneciente a una empresa real.
