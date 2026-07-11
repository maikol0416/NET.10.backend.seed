# Physical Structure API — Documentación técnica de cambios (Multi-tenant + Imagen + Administrador asignado)

## URL Base
```
http://localhost:5296/api/physicalstructure
```

## 🆕 Última actualización — administrador asignado a la estructura física (`administratorUserId`)

Se agregó un campo para asignar, a una estructura física, el usuario (del BC Identity, tabla `AspNetUsers`) que actúa como su administrador. Resumen para integrar:

- **`create` y `update` ganan el campo de entrada `administratorUserId`** — un `string` (no un UUID: es el `Id` de `AspNetUsers`, que es texto), **opcional/nullable**. Puedes omitirlo, mandarlo en `null`, o mandar el `id` de un usuario real.
- **Todas las respuestas (`create`, `update`, `getAll`, `getById`, `getPaginated`) ganan el campo `administratorUserId`** en cada estructura — viene en `null` si todavía no se le asignó administrador.
- **Es mutable en cualquier momento**: a diferencia de `companyId` (inmutable, el backend lo controla), `administratorUserId` sí se actualiza con lo que mandes en `update` — incluido reemplazarlo por otro usuario o volverlo a `null` para "desasignar".
- **El backend NO valida que el usuario tenga un rol en particular** (ej. no exige que sea `Property Administrator`) — acepta cualquier `Id` de `AspNetUsers` que exista. La restricción de "qué usuarios mostrar en el selector" es responsabilidad del front (ver más abajo de dónde sacar la lista).
- **De dónde sale el valor a enviar:** usa `GET /api/auth/users/by-role?role={rol}` (documentado en `AUTH_API_DOCUMENTATION.md`, sección 11) para listar los usuarios candidatos (ej. `role=Property Administrator`) y toma el campo `id` (`string`) de cada uno — ese es el valor que se manda como `administratorUserId`.
- **Validación de forma:** si lo mandas, no puede exceder 450 caracteres (ancho de columna de `AspNetUsers.Id`) ni venir como cadena vacía o solo espacios — en ese caso el backend responde `400`. Para "no asignar todavía" o "quitar el administrador", **omite el campo o mándalo en `null`**, nunca `""`.
- Detalle completo, con ejemplos de request/response, en las secciones 1, 2 y 3 más abajo, y en la nueva sección "Cómo asignar el administrador" al final.

---

## 🆕 Actualización anterior — imagen de la estructura física

Se agregó soporte de imagen a `PhysicalStructure`. Resumen para integrar:

- **`create` y `update` ganan el campo de entrada `imageBytes`** — los bytes de la imagen, en base64 dentro del JSON (así serializa .NET un `byte[]`). Es **opcional**.
- **Todas las respuestas (`create`, `update`, `getAll`, `getById`, `getPaginated`) ganan el campo `pathImg`**: la **URL absoluta y completa** donde quedó guardada la imagen (`http://localhost:5296/uploads/physical-structures/{archivo}`) — **no** es una ruta relativa, no hace falta concatenar nada, se usa tal cual en un `<img src>`.
- El backend **nunca devuelve `imageBytes`** en ninguna respuesta — solo se usa como entrada, se guarda en disco y se descarta del payload.
- Si haces un `update` **sin** mandar `imageBytes`, la imagen que ya tenía la estructura **se conserva tal cual** (no se borra ni se pone en null).
- `pathImg` es de **solo lectura** desde el front: si lo mandas en el body de `create`/`update`, el backend lo ignora — siempre lo calcula él mismo a partir de `imageBytes`.
- Cuando no hay imagen (todavía no se subió ninguna), `pathImg` viene en `null`.
- Detalle completo, con los dos sub-casos de `update` y cómo convertir un archivo a base64 desde el front, en la sección 1, 2 y "Cómo enviar la imagen" más abajo.

---

## Contexto

Este documento cubre los cambios de **multi-tenant** (`companyId`) y de **imagen** (`imageBytes`/`pathImg`) sobre `PhysicalStructure`. El resto del contrato de esta API (`create`, `update`, `delete`, `getAll`, `getById`, `getPaginated`, y todos los campos existentes de torres/apartamentos/áreas comunes) **no cambió** — front sigue usándolo igual.

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
| `PhysicalStructureDto.administratorUserId` | 🆕 AGREGADO | `string \| null`. **Entrada y salida** — se manda en `create`/`update` y viene en todas las respuestas. A diferencia de `companyId`, el backend **sí respeta** el valor que mandes. |
| `PhysicalStructureDto.companyId` | 🆕 AGREGADO | `string \| null`. Visible en todas las respuestas (`create`, `update`, `getById`, `getAll`, `getPaginated`). |
| `PhysicalStructureDto.imageBytes` | 🆕 AGREGADO | `string` (base64) — **solo entrada**, opcional, en `create`/`update`. Nunca aparece en ninguna respuesta. |
| `PhysicalStructureDto.pathImg` | 🆕 AGREGADO | `string \| null` — **URL absoluta y completa**, solo lectura, calculada siempre por el backend. Visible en todas las respuestas. |
| Body de `POST /create` y `PUT /update` | 🟡 AJUSTADO (comportamiento, no forma) | Sigue aceptando `companyId`/`pathImg` si los mandas (no rompe si los omites), pero **se ignoran siempre** — el backend nunca confía en esos valores del cliente. |
| `GET /getAll`, `GET /getById`, `GET /getPaginated` | 🟡 AJUSTADO (comportamiento) | Ahora filtran por empresa automáticamente. Mismo endpoint, mismos parámetros, **resultados distintos** según quién consulta. |
| Estructura de `towers`, `apartments`, `commonAreas`, `location` | ⚪ SIN CAMBIOS | Ningún campo existente se quitó ni se renombró. |
| Endpoints (`create`/`update`/`delete`/`getAll`/`getById`/`getPaginated`) | ⚪ SIN CAMBIOS | Mismas rutas, mismos verbos HTTP. |
| Autorización (`[Authorize]`, sin restricción de rol) | ⚪ SIN CAMBIOS a nivel de atributo | Pero en la práctica, `Administrator` ahora recibe error de negocio al crear/editar (ver punto 3 arriba) — la restricción real está en el handler, no en el atributo (mismo patrón que `register` en Auth). |

---

## 1. CREATE (POST) — 🟡 comportamiento ajustado
**Endpoint:** `POST /api/physicalstructure/create`
**Auth:** `Bearer {token}` (cualquier usuario autenticado, salvo `Administrator` — ver caso de error abajo)

**Body:** igual que antes, más los campos opcionales 🆕 `imageBytes` y 🆕 `administratorUserId`. Si incluyes `companyId` o `pathImg`, se ignoran.

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
  "towers": [],
  "imageBytes": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=",
  "administratorUserId": "4a3b2c1d-e5f6-47a8-9b0c-1d2e3f4a5b6c"
}
```
> 🆕 `imageBytes` es **opcional** — si no tienes imagen todavía, omite el campo (o manda `null`). Ver "Cómo enviar la imagen" más abajo para saber cómo se genera ese string.
> 🆕 `administratorUserId` es **opcional** — si la estructura todavía no tiene administrador asignado, omite el campo (o manda `null`). **Es un `string`** (el `Id` de `AspNetUsers`), no un UUID típico de dominio — cópialo tal cual viene del endpoint de usuarios, sin transformarlo. Ver "Cómo asignar el administrador" más abajo.

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
    "towers": [],
    "imageBytes": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=",
    "administratorUserId": "4a3b2c1d-e5f6-47a8-9b0c-1d2e3f4a5b6c"
  }'
```

**Response (200 OK) — 🆕 `companyId`, `pathImg` y `administratorUserId` en la respuesta:**
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
    "pathImg": "http://localhost:5296/uploads/physical-structures/7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5a.png",
    "administratorUserId": "4a3b2c1d-e5f6-47a8-9b0c-1d2e3f4a5b6c",
    "commonAreas": [],
    "towers": []
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```
El `companyId` que vuelve es siempre el de la empresa del usuario que hizo la llamada — **nunca** el que se haya mandado en el request. `pathImg` ya viene como **URL absoluta completa** (esquema + host + ruta), lista para usar directo en un `<img src>`; es `null` si no se mandó `imageBytes`. `administratorUserId` viene exactamente lo que se mandó (o `null` si no se mandó). El response **nunca** trae `imageBytes` de vuelta.

**🆕 Nuevo error de negocio (400):**
```json
{ "status": false, "message": "Tu usuario no pertenece a ninguna empresa; no puedes crear ni modificar este recurso." }
```
Ocurre cuando quien llama es un `Administrator` (o, en un caso anómalo, cualquier usuario sin empresa asignada).

**🆕 Error de validación si `administratorUserId` viene vacío (400):**
```json
{ "status": false, "message": "[{\"PropertyName\":\"AdministratorUserId\",\"ErrorMessage\":\"El identificador del administrador no puede exceder los 450 caracteres.\", ...}]" }
```
Este caso puntual (`MaximumLength`) solo se dispara si mandas una cadena de más de 450 caracteres. Si mandas `administratorUserId: ""` (cadena vacía, no `null`), el error es distinto y viene del dominio: `400` con mensaje `"El administrador asignado no puede ser un identificador vacío."` — **para "sin administrador" usa siempre `null` o simplemente omite el campo, nunca `""`**.

---

## 2. UPDATE (PUT) — 🟡 comportamiento ajustado
**Endpoint:** `PUT /api/physicalstructure/update`
**Auth:** `Bearer {token}` (mismas reglas que create)

Igual que `create`: si mandas `companyId` o `pathImg`, se ignoran — el backend mantiene el `companyId` que ya tenía la estructura (recuerda: es inmutable, no se puede "trasladar" a otra empresa desde este endpoint).

🆕 **`administratorUserId` sí se respeta en `update`** — a diferencia de `companyId`/`pathImg`, este campo **no** tiene un valor "protegido" por el backend:
- Mándalo con un nuevo `Id` de usuario para **reasignar** el administrador.
- Mándalo en `null` (explícito) para **quitar** el administrador asignado.
- **Omítelo del body** para dejarlo **tal cual estaba** — a diferencia de `pathImg` (que se conserva solo si omites `imageBytes`), aquí aplica la regla normal de PUT: si el campo no viene en el JSON, .NET lo deserializa como `null` en el DTO, así que **omitir el campo tiene el mismo efecto que mandarlo en `null`: lo borra**. Si tu intención es "no tocar el administrador actual", tu formulario debe recuperar primero el valor actual (`GET /getById`) y reenviarlo tal cual en el `update`, igual que haces con el resto de campos del formulario de edición.

### 2a. Actualizar SIN cambiar la imagen (caso más común)
🆕 Omite `imageBytes` del todo (o mándalo en `null`) — el backend conserva el `pathImg` que ya tenía la estructura, **no lo borra**.

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

**Response (200 OK):** `pathImg` viene **igual que antes** de este update — no se tocó.
```json
{
  "data": {
    "id": "8f5a9c1d-....",
    "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
    "name": "Edificio Central Medellín (renovado)",
    "pathImg": "http://localhost:5296/uploads/physical-structures/7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5a.png",
    "...": "resto de campos sin cambios"
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```

### 2b. Actualizar CAMBIANDO la imagen
🆕 Manda `imageBytes` con la nueva imagen — el backend la guarda como un archivo **nuevo** y `pathImg` queda apuntando a ese archivo nuevo. El archivo anterior no se borra automáticamente (queda huérfano en disco).

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
    "towers": [],
    "imageBytes": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII="
  }'
```

**Response (200 OK):**
```json
{
  "data": {
    "id": "8f5a9c1d-....",
    "pathImg": "http://localhost:5296/uploads/physical-structures/a1b2c3d4-5678-90ab-cdef-1234567890ab.png",
    "...": "resto de campos sin cambios"
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```

**Nota de seguridad (efecto colateral del filtro por empresa):** si un usuario intenta actualizar el `id` de una estructura que pertenece a **otra** empresa, el backend la trata como si no existiera (el filtro de tenant la excluye de la búsqueda) y devuelve el error genérico de "no encontrado" — no hay forma de editar ni de enterarte de que existe una estructura de otra empresa con ese Id.

---

## 3. GET BY ID / GET ALL / GET PAGINATED — 🟡 comportamiento ajustado
**Endpoints:** `GET /getById?id={id}`, `GET /getAll`, `GET /getPaginated?pageNumber=1&pageSize=10`
**Auth:** `Bearer {token}`

Mismos parámetros y forma de respuesta que antes, **más los campos `companyId`, 🆕 `pathImg` y 🆕 `administratorUserId`** en cada estructura. La diferencia real es el **filtrado automático**:

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
        "pathImg": "http://localhost:5296/uploads/physical-structures/a1b2c3d4-5678-90ab-cdef-1234567890ab.png",
        "administratorUserId": "4a3b2c1d-e5f6-47a8-9b0c-1d2e3f4a5b6c",
        "...": "resto de campos sin cambios"
      },
      {
        "id": "3c4d5e6f-....",
        "name": "Conjunto Los Alamos",
        "pathImg": null,
        "administratorUserId": null,
        "...": "resto de campos sin cambios"
      }
    ],
    "totalCount": 2,
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
- 🆕 `pathImg` viene en `null` cuando la estructura todavía no tiene imagen guardada.
- 🆕 `administratorUserId` viene en `null` cuando la estructura todavía no tiene administrador asignado.

---

## 🆕 Cómo mostrar la imagen en el front

`pathImg` ya es una **URL absoluta y completa** (esquema + host + ruta) — el backend la arma con el mismo host desde el que se sirvió la petición. **No** hay que concatenar nada: úsala directo.

```ts
const imageUrl = data.pathImg; // ya es la URL completa, o null si no tiene imagen
// <img src={imageUrl} /> si no es null; si es null, muestra un placeholder
```

> ⚠️ Como la URL se arma dinámicamente a partir del host de la petición, si la Api corre detrás de un proxy/gateway que expone un host distinto al que ve el proceso .NET (por ejemplo, un dominio público en frente de un contenedor interno), avisa al equipo de backend — hay que configurar el forwarding de headers (`X-Forwarded-Host`) para que la URL guardada sea la pública y no la interna.

---

## 🆕 Cómo enviar la imagen desde el front (de dónde sale `imageBytes`)

`imageBytes` es un `byte[]` de .NET — en JSON viaja como **string en base64**, no como array de números. Si el front captura la imagen desde un `<input type="file">`, conviértela así antes de armar el body:

```ts
async function fileToBase64(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      // reader.result viene como "data:image/png;base64,AAAA..." — hay que quitar el prefijo
      const base64 = result.split(',')[1];
      resolve(base64);
    };
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

// Uso:
const imageBytes = await fileToBase64(selectedFile);
const body = { ...restoDelFormulario, imageBytes };
```
No hace falta mandar el nombre del archivo ni el tipo MIME — el backend detecta el formato (PNG/JPEG/GIF/WEBP) a partir de los propios bytes; si no reconoce el formato, lo guarda igual como `.png`.

---

## 🆕 Cómo asignar el administrador (`administratorUserId`)

`administratorUserId` guarda el `Id` (`string`) de un usuario del sistema (tabla `AspNetUsers`, BC Identity) — **no** un recurso propio de `PhysicalStructure`. El front necesita dos cosas: un selector para elegir el usuario, y saber qué valor mandar.

### 1. Alimentar el selector

Usa el endpoint de Auth documentado en [`AUTH_API_DOCUMENTATION.md`](./AUTH_API_DOCUMENTATION.md) (sección 11, "Listar usuarios por rol, sin paginar") — pensado justo para este caso ("elegir qué usuario asignar como administrador de una propiedad"):

```bash
curl -X GET "http://localhost:5296/api/auth/users/by-role?role=Property%20Administrator" \
  -H "Authorization: Bearer {token}"
```

```json
{
  "data": [
    {
      "id": "4a3b2c1d-e5f6-47a8-9b0c-1d2e3f4a5b6c",
      "email": "propadmin1@losrobles.com",
      "fullName": "Carlos Pérez",
      "roles": ["Property Administrator"],
      "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
    }
  ],
  "status": true,
  "message": "Operation carried out successfully."
}
```

- El nombre del rol (`Property Administrator` en el ejemplo) **no está fijo en el backend** — es un rol más, creado vía `POST /api/auth/create-role`. Confirma con el equipo/backoffice cuál es el rol real que se usa para "administrador de estructura física" en este ambiente.
- Si quien está logueado es `Company Administrator`, no hace falta mandar `companyId` en la query — el backend acota automáticamente a su propia empresa. Si es `Administrator` (plataforma), sí puedes mandar `companyId` para acotar a una empresa puntual.
- Del array de respuesta, muestra `fullName`/`email` en el selector y guarda el `id` de cada opción — ese `id` (string) es el valor que se manda como `administratorUserId`.

### 2. Mandarlo en `create`/`update`

```json
{
  "...": "resto del formulario de la estructura",
  "administratorUserId": "4a3b2c1d-e5f6-47a8-9b0c-1d2e3f4a5b6c"
}
```

### 3. Mostrarlo de vuelta

La respuesta de `create`/`update`/`getById`/`getAll`/`getPaginated` solo trae el `administratorUserId` (el Id crudo) — **no** trae el nombre ni el email del administrador embebido. Si tu UI necesita mostrar "Administrador: Carlos Pérez" en el listado/detalle de la estructura, resuelve el nombre por tu cuenta cruzando ese `id` contra la lista que ya trajiste de `GET /users/by-role` (o pide `GET /users/paginated` si necesitas buscarlo entre todos los usuarios, no solo los de un rol).

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
  imageBytes?: string;         // 🆕 base64 — SOLO entrada (create/update), nunca viene en la respuesta
  pathImg?: string | null;     // 🆕 URL absoluta y completa de la imagen — SOLO lectura, lista para usar en <img src>, el backend la calcula siempre
  administratorUserId?: string | null; // 🆕 Id (string) de un usuario de AspNetUsers — entrada Y salida, mutable en cualquier update, null si no hay administrador asignado
  // ... el resto de campos de towers/apartments no cambió
}
```

---

## Checklist para el front

- [ ] Mostrar `companyId` (resuelto a nombre de empresa vía `GET /managementcompany/getAll`) en el listado/detalle de estructuras, **solo como dato informativo** — nunca como campo editable en los formularios de create/update.
- [ ] Si algún flujo de `Administrator` permite crear/editar estructuras físicas, ocultarlo o mostrar un mensaje claro — ya no es una operación soportada para ese rol.
- [ ] No asumas que `getAll`/`getPaginated` devuelve el mismo listado para todos los usuarios — cada empresa ve solo lo suyo.
- [ ] Si estabas usando datos de prueba creados antes de este cambio, espera verlos desaparecer de la vista de usuarios normales (quedaron con empresa vacía) — créalos de nuevo con una sesión de usuario perteneciente a una empresa real.
- [ ] 🆕 En los formularios de crear/editar estructura, agrega un `<input type="file" accept="image/*">` opcional, conviértelo a base64 (ver "Cómo enviar la imagen") y mándalo como `imageBytes` — sin nombre de archivo ni tipo MIME, el backend los detecta solo.
- [ ] 🆕 En edición, si el usuario no toca el input de imagen, **no mandes `imageBytes`** — así se conserva la imagen actual. Solo mándalo cuando el usuario explícitamente selecciona un archivo nuevo.
- [ ] 🆕 En listados y detalle, usa `pathImg` directo como `src` de la imagen (ya es una URL absoluta completa, no hace falta concatenar nada) — maneja el caso `pathImg: null` con un placeholder (estructura sin imagen todavía).
- [ ] 🆕 Agrega un selector de "Administrador" en los formularios de crear/editar estructura, alimentado por `GET /api/auth/users/by-role?role={rol}` (ver "Cómo asignar el administrador"). Manda el `id` elegido como `administratorUserId`; incluye una opción "Sin asignar" que mande `null`.
- [ ] 🆕 En el formulario de **editar**, precarga el `administratorUserId` actual (viene en la respuesta de `getById`) y reenvíalo tal cual si el usuario no lo cambia — a diferencia de `imageBytes`/`pathImg`, este campo **no se conserva solo**: si lo omites en el `update`, el backend lo interpreta como `null` y borra el administrador asignado.
- [ ] 🆕 Nunca mandes `administratorUserId: ""` (cadena vacía) para "sin administrador" — usa `null` u omite el campo; `""` provoca un error `400` de dominio.
- [ ] 🆕 En listados/detalle, si necesitas mostrar el nombre del administrador (no solo su Id), resuélvelo cruzando `administratorUserId` contra los datos ya traídos de `GET /users/by-role` — el backend no lo embebe en la respuesta de `PhysicalStructure`.
