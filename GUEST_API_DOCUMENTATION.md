# Guest API - Documentación de Endpoints

## URL Base
```
http://localhost:5000/api/guest
```

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
      "endDate": "2026-07-06T10:00:00Z"
    },
    {
      "startDate": "2026-07-10T10:00:00Z",
      "endDate": "2026-07-15T10:00:00Z"
    }
  ]
}
```

**cURL:**
```bash
curl -X POST http://localhost:5000/api/guest/create \
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
        "endDate": "2026-07-06T10:00:00Z"
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
        "endDate": "2026-07-06T10:00:00Z"
      }
    ]
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

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
      "endDate": "2026-07-15T10:00:00Z"
    }
  ]
}
```

**cURL:**
```bash
curl -X PUT http://localhost:5000/api/guest/update \
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
        "endDate": "2026-07-15T10:00:00Z"
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
        "endDate": "2026-07-15T10:00:00Z"
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
curl -X GET "http://localhost:5000/api/guest/getById?id=8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f" \
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
        "endDate": "2026-07-15T10:00:00Z"
      }
    ]
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

---

## 4. OBTENER TODOS LOS GUESTS (GET)
**Endpoint:** `GET /api/guest/getAll`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X GET "http://localhost:5000/api/guest/getAll" \
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
          "endDate": "2026-07-15T10:00:00Z"
        }
      ]
    }
  ],
  "status": true,
  "message": "Operación completada exitosamente."
}
```

---

## 5. OBTENER GUESTS PAGINADOS (GET)
**Endpoint:** `GET /api/guest/getPaginated?pageNumber=1&pageSize=10`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X GET "http://localhost:5000/api/guest/getPaginated?pageNumber=1&pageSize=10" \
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
            "endDate": "2026-07-15T10:00:00Z"
          }
        ]
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1
  },
  "status": true,
  "message": "Operación completada exitosamente."
}
```

---

## 6. ELIMINAR GUEST (DELETE)
**Endpoint:** `DELETE /api/guest/delete?id={guestId}`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X DELETE "http://localhost:5000/api/guest/delete?id=8f5a9c1d-2b4e-4c7f-9a2d-1e5b8c3a6d9f" \
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
}
```

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

---

## CÓDIGOS DE ERROR COMUNES

| ErrorCode | HTTP | Causa |
|-----------|------|-------|
| NameEmpty | 400 | Nombre vacío/nulo |
| NameMaxLength | 400 | Nombre > 150 caracteres |
| EmailEmpty | 400 | Email vacío/nulo |
| EmailInvalidFormat | 400 | Email no válido (no cumple regex) |
| GuestPermissionDateRangeInvalid | 400 | StartDate >= EndDate |
| 401 | 401 | Token expirado o inválido |
| 500 | 500 | Error interno del servidor |
