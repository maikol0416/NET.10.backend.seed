# Guest API - Documentación de Endpoints

## URL Base
```
http://localhost:5297/api/guest
```

---

## ⚠️ Cambio reciente — permisos ahora indican propiedad y apartamento

Cada `GuestPermission` antes solo tenía `startDate`/`endDate`, sin ningún vínculo a una propiedad horizontal. Un mismo huésped podía tener varios permisos, pero no había forma de saber a cuál propiedad de la empresa correspondía cada uno. Ahora:

| Campo | Antes | Ahora | Impacto en el front |
|---|---|---|---|
| `guestPermissions[].physicalStructureId` | No existía | **Obligatorio** (UUID) | Si falta en `create`/`update`, el backend responde **400** con `GuestPermissionPhysicalStructureIdEmpty` — hazlo campo requerido en el formulario |
| `guestPermissions[].apartmentId` | No existía | Opcional (UUID o `null`) | Si se omite, el permiso aplica a toda la propiedad, no a un apartamento puntual |
| `guestPermissions[].physicalStructureName` | No existía | **Solo lectura**, viene en `getById`/`getAll`/`getPaginated` | Nombre de la propiedad ya resuelto por el backend — úsalo directo, no lo mandes en `create`/`update` (se ignora) |
| `guestPermissions[].apartmentNumber` | No existía | **Solo lectura**, viene en `getById`/`getAll`/`getPaginated` | Número del apartamento ya resuelto por el backend (`null` si no hay `apartmentId`) — no lo mandes en `create`/`update` (se ignora) |

**Qué hacer en el front:**
1. En el formulario de crear/editar huésped, cada permiso necesita un selector de propiedad horizontal (`physicalStructureId`, alimentado por `GET /api/PhysicalStructure/getAll` o `getPaginated`) y, opcionalmente, un selector de apartamento dependiente de esa propiedad (`apartmentId`, filtrando `PhysicalStructureDto.towers[].apartments[]` de la propiedad elegida).
2. En listados/detalle, usa `physicalStructureName` y `apartmentNumber` directamente — el backend ya resolvió esos nombres, no hace falta otra llamada.
3. Si `apartmentId` no aplica, no lo mandes o mándalo como `null` — nunca como `""` (string vacío), eso sí lo rechaza el backend.

Esto es un **breaking change** si tu integración actual crea/edita `GuestPermission` sin `physicalStructureId`: esos payloads van a empezar a fallar con 400.

---

## 1. CREAR GUEST (POST)
**Endpoint:** `POST /api/guest/create`

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (Entrada - GuestDto):**
```json
{
  "name": "Ana",
  "lastName": "Torres",
  "documentType": 1,
  "documentNumber": "1122334455",
  "phoneNumber": "3001112233",
  "email": "ana.torres@example.com",
  "termsAndCondition": "v1.0",
  "responseTermsAndCondition": "Aceptado",
  "mediaId": "media-789",
  "guestPermissions": [
    {
      "startDate": "2026-07-05T10:00:00Z",
      "endDate": "2026-07-06T10:00:00Z",
      "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
      "apartmentId": "9c8b7a6d-5e4f-3d2c-1b0a-9f8e7d6c5b4a"
    },
    {
      "startDate": "2026-07-10T10:00:00Z",
      "endDate": "2026-07-15T10:00:00Z",
      "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
    }
  ]
}
```

> El segundo permiso no manda `apartmentId` — aplica a toda la propiedad, no a un apartamento puntual. Es válido omitirlo.

**cURL:**
```bash
curl -X POST http://localhost:5297/api/guest/create \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Ana",
    "lastName": "Torres",
    "documentType": 1,
    "documentNumber": "1122334455",
    "phoneNumber": "3001112233",
    "email": "ana.torres@example.com",
    "termsAndCondition": "v1.0",
    "responseTermsAndCondition": "Aceptado",
    "mediaId": "media-789",
    "guestPermissions": [
      {
        "startDate": "2026-07-05T10:00:00Z",
        "endDate": "2026-07-06T10:00:00Z",
        "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
      }
    ]
  }'
```

**Response (Salida - 200 OK):**
```json
{
  "data": {
    "id": "8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f",
    "name": "Ana",
    "lastName": "Torres",
    "documentType": 1,
    "documentNumber": "1122334455",
    "phoneNumber": "3001112233",
    "email": "ana.torres@example.com",
    "termsAndCondition": "v1.0",
    "responseTermsAndCondition": "Aceptado",
    "mediaId": "media-789",
    "guestPermissions": [
      {
        "id": "7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5g",
        "startDate": "2026-07-05T10:00:00Z",
        "endDate": "2026-07-06T10:00:00Z",
        "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
        "apartmentId": null,
        "physicalStructureName": null,
        "apartmentNumber": null
      }
    ]
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

> Nota: `physicalStructureName`/`apartmentNumber` vienen `null` en la respuesta de `create`/`update` — solo se resuelven en las consultas (`getById`/`getAll`/`getPaginated`), ver ejemplos más abajo.

---

## 2. ACTUALIZAR GUEST (PUT)
**Endpoint:** `PUT /api/guest/update`

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (Entrada - GuestDto con Id):**
```json
{
  "id": "8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f",
  "name": "Luisa",
  "lastName": "Ramirez",
  "documentType": 2,
  "documentNumber": "9988776655",
  "phoneNumber": "3009998877",
  "email": "luisa.ramirez@example.com",
  "termsAndCondition": "v2.0",
  "responseTermsAndCondition": "Rechazado",
  "mediaId": "media-999",
  "guestPermissions": [
    {
      "id": "7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5g",
      "startDate": "2026-07-10T10:00:00Z",
      "endDate": "2026-07-15T10:00:00Z",
      "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
      "apartmentId": "9c8b7a6d-5e4f-3d2c-1b0a-9f8e7d6c5b4a"
    }
  ]
}
```

> `UpdateGuestPermissions` **reemplaza toda la colección**: si mandas un solo permiso en el array, los demás que tuviera el guest se eliminan. No es un merge/patch parcial — manda siempre el array completo con todos los permisos vigentes.

**cURL:**
```bash
curl -X PUT http://localhost:5297/api/guest/update \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f",
    "name": "Luisa",
    "lastName": "Ramirez",
    "documentType": 2,
    "documentNumber": "9988776655",
    "phoneNumber": "3009998877",
    "email": "luisa.ramirez@example.com",
    "termsAndCondition": "v2.0",
    "responseTermsAndCondition": "Rechazado",
    "mediaId": "media-999",
    "guestPermissions": [
      {
        "startDate": "2026-07-10T10:00:00Z",
        "endDate": "2026-07-15T10:00:00Z",
        "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
      }
    ]
  }'
```

**Response (Salida - 200 OK):**
```json
{
  "data": {
    "id": "8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f",
    "name": "Luisa",
    "lastName": "Ramirez",
    "documentType": 2,
    "documentNumber": "9988776655",
    "phoneNumber": "3009998877",
    "email": "luisa.ramirez@example.com",
    "termsAndCondition": "v2.0",
    "responseTermsAndCondition": "Rechazado",
    "mediaId": "media-999",
    "guestPermissions": [
      {
        "id": "7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5g",
        "startDate": "2026-07-10T10:00:00Z",
        "endDate": "2026-07-15T10:00:00Z",
        "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
        "apartmentId": null,
        "physicalStructureName": null,
        "apartmentNumber": null
      }
    ]
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

---

## 3. OBTENER GUEST POR ID (GET)
**Endpoint:** `GET /api/guest/getById?id={guestId}`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X GET "http://localhost:5297/api/guest/getById?id=8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):**
```json
{
  "data": {
    "id": "8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f",
    "name": "Luisa",
    "lastName": "Ramirez",
    "documentType": 2,
    "documentNumber": "9988776655",
    "phoneNumber": "3009998877",
    "email": "luisa.ramirez@example.com",
    "termsAndCondition": "v2.0",
    "responseTermsAndCondition": "Rechazado",
    "mediaId": "media-999",
    "guestPermissions": [
      {
        "id": "7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5g",
        "startDate": "2026-07-10T10:00:00Z",
        "endDate": "2026-07-15T10:00:00Z",
        "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
        "apartmentId": "9c8b7a6d-5e4f-3d2c-1b0a-9f8e7d6c5b4a",
        "physicalStructureName": "Torres del Parque",
        "apartmentNumber": "302"
      }
    ]
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

> A diferencia de `create`/`update`, en `getById`/`getAll`/`getPaginated` el backend resuelve `physicalStructureName` y `apartmentNumber` a partir de los IDs (una sola consulta batched, no N+1). Úsalos directo para mostrar texto legible en la UI.

---

## 4. OBTENER TODOS LOS GUESTS (GET)
**Endpoint:** `GET /api/guest/getAll`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X GET "http://localhost:5297/api/guest/getAll" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):**
```json
{
  "data": [
    {
      "id": "8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f",
      "name": "Luisa",
      "lastName": "Ramirez",
      "documentType": 2,
      "documentNumber": "9988776655",
      "phoneNumber": "3009998877",
      "email": "luisa.ramirez@example.com",
      "termsAndCondition": "v2.0",
      "responseTermsAndCondition": "Rechazado",
      "mediaId": "media-999",
      "guestPermissions": [
        {
          "id": "7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5g",
          "startDate": "2026-07-10T10:00:00Z",
          "endDate": "2026-07-15T10:00:00Z",
          "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
          "apartmentId": null,
          "physicalStructureName": "Torres del Parque",
          "apartmentNumber": null
        }
      ]
    }
  ],
  "status": true,
  "message": "Operación completada exitosamente."
}
```

> Este permiso de ejemplo no tiene `apartmentId` (aplica a toda la propiedad) — por eso `apartmentNumber` viene `null` aunque `physicalStructureName` sí se resuelve.

---

## 5. OBTENER GUESTS PAGINADOS (GET)
**Endpoint:** `GET /api/guest/getPaginated?pageNumber=1&pageSize=10`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X GET "http://localhost:5297/api/guest/getPaginated?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):**
```json
{
  "data": {
    "items": [
      {
        "id": "8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f",
        "name": "Luisa",
        "lastName": "Ramirez",
        "documentType": 2,
        "documentNumber": "9988776655",
        "phoneNumber": "3009998877",
        "email": "luisa.ramirez@example.com",
        "termsAndCondition": "v2.0",
        "responseTermsAndCondition": "Rechazado",
        "mediaId": "media-999",
        "guestPermissions": [
          {
            "id": "7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5g",
            "startDate": "2026-07-10T10:00:00Z",
            "endDate": "2026-07-15T10:00:00Z",
            "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
            "apartmentId": "9c8b7a6d-5e4f-3d2c-1b0a-9f8e7d6c5b4a",
            "physicalStructureName": "Torres del Parque",
            "apartmentNumber": "302"
          }
        ]
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

> Nota: corregido — `totalPages` **no existe** en la respuesta real (`PaginatedList<T>` solo expone `items`, `totalCount`, `pageNumber`, `pageSize`). Si lo necesitas para la UI de paginación, calcúlalo en el front: `Math.ceil(totalCount / pageSize)`.

---

## 6. OBTENER GUESTS PAGINADOS FILTRADOS POR PROPIEDAD/APARTAMENTO (GET)

**Nuevo endpoint** — responde a la necesidad de filtrar el listado de huéspedes por la propiedad horizontal (y opcionalmente el apartamento) a la que tienen acceso, en vez de traer todos los guests y filtrar en el cliente.

**Endpoint:** `GET /api/guest/getPaginatedByProperty`

**Headers:**
```
Authorization: Bearer {token}
```

**Query params:**

| Param | Tipo | Requerido | Descripción |
|---|---|---|---|
| `physicalStructureId` | UUID | **Sí** | Trae los guests que tengan al menos un `GuestPermission` para esta propiedad. |
| `apartmentId` | UUID | No | Si se manda, acota más: el guest debe tener un `GuestPermission` para esa propiedad **y** ese apartamento puntual. Si se omite, no filtra por apartamento (trae permisos de toda la propiedad, tengan o no `apartmentId`). |
| `pageNumber` | int | No (default `1`) | |
| `pageSize` | int | No (default `10`) | |

**Semántica del filtro:** un guest aparece en el resultado si **al menos uno** de sus `guestPermissions` cumple el criterio — no hace falta que todos sus permisos sean de esa propiedad. El objeto `guestPermissions` que viene en la respuesta trae **todos** los permisos del guest (igual que en `getAll`/`getPaginated`), no solo el que hizo match con el filtro — si un guest tiene permisos en 3 propiedades y filtras por una, en la respuesta igual vas a ver las 3.

**cURL:**
```bash
curl -X GET "http://localhost:5297/api/guest/getPaginatedByProperty?physicalStructureId=3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b&pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}"
```

**cURL (filtrando también por apartamento):**
```bash
curl -X GET "http://localhost:5297/api/guest/getPaginatedByProperty?physicalStructureId=3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b&apartmentId=9c8b7a6d-5e4f-3d2c-1b0a-9f8e7d6c5b4a&pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):** misma forma que `getPaginated` (sección 5) — `physicalStructureName`/`apartmentNumber` vienen resueltos igual.
```json
{
  "data": {
    "items": [
      {
        "id": "8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f",
        "name": "Luisa",
        "lastName": "Ramirez",
        "documentType": 2,
        "documentNumber": "9988776655",
        "phoneNumber": "3009998877",
        "email": "luisa.ramirez@example.com",
        "termsAndCondition": "v2.0",
        "responseTermsAndCondition": "Rechazado",
        "mediaId": "media-999",
        "guestPermissions": [
          {
            "id": "7d4c2a9e-1b3f-4e6a-8c5d-2f7a9b1c3e5g",
            "startDate": "2026-07-10T10:00:00Z",
            "endDate": "2026-07-15T10:00:00Z",
            "physicalStructureId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
            "apartmentId": "9c8b7a6d-5e4f-3d2c-1b0a-9f8e7d6c5b4a",
            "physicalStructureName": "Torres del Parque",
            "apartmentNumber": "302"
          }
        ]
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

### ⚠️ Comportamientos a tener en cuenta

- **`physicalStructureId` es "requerido" solo en el sentido de que hace falta para que el filtro tenga sentido — el backend no lo valida.** Si lo omites por accidente, ASP.NET **no devuelve 400**: lo bindea silenciosamente como `00000000-0000-0000-0000-000000000000` y el endpoint responde `200 OK` con `totalCount: 0` (porque ningún permiso real tiene un id vacío — está prohibido a nivel de dominio). Es decir, un bug en el front de "olvidé mandar el query param" se ve como "no hay resultados", no como un error explícito. **Verificado en vivo**: probé el binding exacto que usa este endpoint contra un servidor de prueba — confirmar que siempre mandas `physicalStructureId` es responsabilidad del front.
- **Si mandas un valor que no es un GUID válido** (ej. `physicalStructureId=abc`), ahí sí falla, pero con **400 en el formato estándar de ASP.NET** (`ProblemDetails`), **no** en el envelope `{status, message, data}` del resto de la API:
  ```json
  {
    "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": { "physicalStructureId": ["The value 'abc' is not valid."] }
  }
  ```
  Este es el único endpoint de `Guest` donde puede pasarte esto — todos los demás reciben el body completo vía `[FromBody]` con FluentValidation, que sí usa el envelope. Maneja este caso por separado en el cliente HTTP (revisa si `data`/`status` existen en la respuesta antes de asumir el formato de siempre).
- No hay endpoint para traer, además del listado de guests, cuántos permisos distintos por propiedad hay (analytics) — esto es puramente "dame los guests que aplican a esta propiedad/apartamento".

---

## 7. ELIMINAR GUEST (DELETE)
**Endpoint:** `DELETE /api/guest/delete?id={guestId}`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X DELETE "http://localhost:5297/api/guest/delete?id=8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):**
```json
{
  "data": true,
  "status": true,
  "message": "Operación completada exitosamente."
}
```

---

## ESTRUCTURA DE OBJETOS DTO

### GuestDto
```typescript
interface GuestDto {
  id?: string;                           // UUID (opcional en CREATE, requerido en UPDATE)
  name: string;                          // Max 150 chars, requerido
  lastName: string;                      // Max 150 chars, requerido
  documentType: number;                  // Enum: CC=1, CE=2, TI=3, PA=4, CI=5
  documentNumber: string;                // Max 20 chars, requerido
  phoneNumber: string;                   // Max 15 chars, requerido
  email: string;                         // Max 255 chars, requerido, formato válido
  termsAndCondition: string;             // Max 500 chars, requerido
  responseTermsAndCondition: string;     // Max 500 chars, requerido
  mediaId: string;                       // Max 100 chars, requerido
  guestPermissions: GuestPermissionDto[]; // Array de permisos
}
```

### GuestPermissionDto
```typescript
interface GuestPermissionDto {
  id?: string;                           // UUID (opcional, se genera en servidor)
  startDate: string;                     // ISO 8601 DateTime, requerido
  endDate: string;                       // ISO 8601 DateTime, requerido (> startDate)
  physicalStructureId: string;           // UUID, REQUERIDO — propiedad horizontal a la que aplica el permiso
  apartmentId?: string | null;           // UUID, OPCIONAL — apartamento puntual dentro de esa propiedad
  physicalStructureName?: string | null; // SOLO LECTURA — resuelto por el backend, solo en getById/getAll/getPaginated
  apartmentNumber?: string | null;       // SOLO LECTURA — resuelto por el backend, null si no hay apartmentId o no se encontró
}
```

> `physicalStructureName` y `apartmentNumber` son campos de salida: el backend los ignora si los mandas en `create`/`update`.

---

## CÓDIGOS DE DocumentTypeEnum
```
1  = CC   (Cédula de Ciudadanía)
2  = CE   (Cédula de Extranjería)
3  = TI   (Tarjeta de Identidad)
4  = PA   (Pasaporte)
5  = CI   (Cédula de Identidad)
```

---

## VALIDACIONES CRÍTICAS

### GuestDto
- ❌ `name`: No puede ser nulo, vacío o solo espacios. Max 150 chars.
- ❌ `lastName`: No puede ser nulo, vacío o solo espacios. Max 150 chars.
- ❌ `documentNumber`: No puede ser nulo, vacío o solo espacios. Max 20 chars.
- ❌ `phoneNumber`: No puede ser nulo, vacío o solo espacios. Max 15 chars.
- ❌ `email`: No puede ser nulo/vacío. Debe cumplir formato de correo válido.
- ❌ `termsAndCondition`: No puede ser nulo, vacío o solo espacios. Max 500 chars.
- ❌ `responseTermsAndCondition`: No puede ser nulo, vacío o solo espacios. Max 500 chars.
- ❌ `mediaId`: No puede ser nulo, vacío o solo espacios. Max 100 chars.
- ❌ `documentType`: Debe ser uno de los valores válidos (1-5).

### GuestPermissionDto
- ❌ `startDate`: Debe ser menor que `endDate` (validación a nivel de dominio y application).
- ❌ `endDate`: Debe ser mayor que `startDate`.
- ❌ `physicalStructureId`: **Obligatorio**, no puede ser `Guid.Empty` (validación de FluentValidation en `create`/`update`).
- ⚠️ `apartmentId`: Opcional. Si se manda, no puede ser `Guid.Empty` (validación de dominio, no de FluentValidation — el error viene sin `ErrorCode`, ver nota abajo).

---

## CÓDIGOS DE ERROR COMUNES

| ErrorCode | HTTP | Causa |
|-----------|------|-------|
| NameEmpty | 400 | Nombre vacío/nulo |
| NameMaxLength | 400 | Nombre > 150 caracteres |
| EmailEmpty | 400 | Email vacío/nulo |
| EmailInvalidFormat | 400 | Email no válido (no cumple regex) |
| GuestPermissionDateRangeInvalid | 400 | StartDate >= EndDate |
| GuestPermissionPhysicalStructureIdEmpty | 400 | `physicalStructureId` vacío o ausente en algún permiso |
| 401 | 401 | Token expirado o inválido |
| 500 | 500 | Error interno del servidor |

### Notas sobre `physicalStructureId` / `apartmentId`

- `physicalStructureId` **no se valida contra la tabla de propiedades** a nivel de FluentValidation, solo se exige que no esté vacío. La integridad la garantiza una foreign key en base de datos: si mandas un `physicalStructureId` que no existe, vas a recibir un **500** (no un 400 legible), no una respuesta de validación de campo. Alimenta el selector siempre desde `GET /api/PhysicalStructure/getAll`/`getPaginated`, nunca un input libre.
- `apartmentId` **no tiene FK a nivel de base de datos** (el apartamento vive anidado dentro de la propiedad, no es una entidad de primer nivel). Si mandas un `apartmentId` que no existe dentro de esa propiedad, el backend **no lo rechaza**: el permiso se guarda igual, y en consultas posteriores `apartmentNumber` simplemente viene `null` porque no hubo coincidencia. Alimenta el selector de apartamento desde `PhysicalStructureDto.towers[].apartments[]` de la propiedad elegida para evitar este caso.
- El único caso donde `apartmentId` sí se rechaza es si mandas explícitamente el GUID vacío `00000000-0000-0000-0000-000000000000` en vez de `null`/omitirlo — ahí salta una validación de dominio ("El apartamento del permiso no puede ser un identificador vacío"), que llega como **400 en texto plano** (`message: "El apartamento del permiso no puede ser un identificador vacío."`), **no** en el formato JSON-doble-serializado de FluentValidation — no intentes hacer `JSON.parse` sobre ese `message`. Si mandas directamente un string que no es un GUID válido (ej. `""`), falla antes, en el binding del request (400 de ASP.NET, formato distinto también) — evita ambos casos simplemente omitiendo el campo o mandando `null`.
