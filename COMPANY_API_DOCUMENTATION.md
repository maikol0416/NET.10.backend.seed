# Company API - Documentación de Endpoints

## URL Base
```
http://localhost:5296/api/managementcompany
```

## Autenticación
Todos los endpoints requieren:
```
Authorization: Bearer {token}
```
No hay restricción de rol adicional — cualquier usuario autenticado puede crear, editar, eliminar y listar empresas administradoras. Si falta el header o el token es inválido/expiró → `401`.

> ℹ️ Este agregado todavía no forma parte del esquema multi-tenant (no existe `CompanyId` en ningún otro módulo, no hay aislamiento por empresa ni restricción a un rol tipo `Administrator`). Es un CRUD simple, sin relaciones ni Value Objects.

---

## 1. CREAR EMPRESA (POST)
**Endpoint:** `POST /api/managementcompany/create`

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (Entrada - ManagementCompanyDto):**
```json
{
  "name": "Administradora Los Robles",
  "nit": "900123456-7",
  "contactEmail": "contacto@losrobles.com",
  "contactPhone": "3001234567"
}
```

**cURL:**
```bash
curl -X POST http://localhost:5296/api/managementcompany/create \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Administradora Los Robles",
    "nit": "900123456-7",
    "contactEmail": "contacto@losrobles.com",
    "contactPhone": "3001234567"
  }'
```

**Response (Salida - 200 OK):**
```json
{
  "data": {
    "id": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
    "name": "Administradora Los Robles",
    "nit": "900123456-7",
    "contactEmail": "contacto@losrobles.com",
    "contactPhone": "3001234567"
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```

---

## 2. ACTUALIZAR EMPRESA (PUT)
**Endpoint:** `PUT /api/managementcompany/update`

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Body (Entrada - ManagementCompanyDto con Id):**
```json
{
  "id": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
  "name": "Administradora Los Robles S.A.S.",
  "nit": "900123456-7",
  "contactEmail": "contacto@losrobles.com",
  "contactPhone": "3009999999"
}
```

**cURL:**
```bash
curl -X PUT http://localhost:5296/api/managementcompany/update \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
    "name": "Administradora Los Robles S.A.S.",
    "nit": "900123456-7",
    "contactEmail": "contacto@losrobles.com",
    "contactPhone": "3009999999"
  }'
```

**Response (Salida - 200 OK):**
```json
{
  "data": {
    "id": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
    "name": "Administradora Los Robles S.A.S.",
    "nit": "900123456-7",
    "contactEmail": "contacto@losrobles.com",
    "contactPhone": "3009999999"
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```

---

## 3. OBTENER EMPRESA POR ID (GET)
**Endpoint:** `GET /api/managementcompany/getById?id={companyId}`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X GET "http://localhost:5296/api/managementcompany/getById?id=3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):**
```json
{
  "data": {
    "id": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
    "name": "Administradora Los Robles S.A.S.",
    "nit": "900123456-7",
    "contactEmail": "contacto@losrobles.com",
    "contactPhone": "3009999999"
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```

---

## 4. OBTENER TODAS LAS EMPRESAS (GET)
**Endpoint:** `GET /api/managementcompany/getAll`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X GET "http://localhost:5296/api/managementcompany/getAll" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):**
```json
{
  "data": [
    {
      "id": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
      "name": "Administradora Los Robles S.A.S.",
      "nit": "900123456-7",
      "contactEmail": "contacto@losrobles.com",
      "contactPhone": "3009999999"
    }
  ],
  "status": true,
  "message": "Operation carried out successfully."
}
```

---

## 5. OBTENER EMPRESAS PAGINADAS (GET)
**Endpoint:** `GET /api/managementcompany/getPaginated?pageNumber=1&pageSize=10`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X GET "http://localhost:5296/api/managementcompany/getPaginated?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):**
```json
{
  "data": {
    "items": [
      {
        "id": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
        "name": "Administradora Los Robles S.A.S.",
        "nit": "900123456-7",
        "contactEmail": "contacto@losrobles.com",
        "contactPhone": "3009999999"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1
  },
  "status": true,
  "message": "Operation carried out successfully."
}
```

---

## 6. ELIMINAR EMPRESA (DELETE)
**Endpoint:** `DELETE /api/managementcompany/delete?code={companyId}`

**Headers:**
```
Authorization: Bearer {token}
```

**cURL:**
```bash
curl -X DELETE "http://localhost:5296/api/managementcompany/delete?code=3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b" \
  -H "Authorization: Bearer {token}"
```

**Response (Salida - 200 OK):**
```json
{
  "data": true,
  "status": true,
  "message": "Operation carried out successfully."
}
```
> ⚠️ El backend actual **no valida** dependencias antes de borrar (por ejemplo, si en el futuro hay usuarios o estructuras físicas asociadas a la empresa). Usa confirmación explícita en el front antes de llamar este endpoint.

---

## ESTRUCTURA DE OBJETOS DTO

### ManagementCompanyDto
```typescript
interface ManagementCompanyDto {
  id?: string;              // UUID (opcional en CREATE, requerido en UPDATE)
  name: string;              // requerido
  nit: string;                // requerido
  contactEmail: string;       // requerido
  contactPhone: string;       // requerido
}
```
Los 4 campos son strings planos — el DTO no tiene objetos anidados ni colecciones.

---

## VALIDACIONES CRÍTICAS

### ManagementCompanyDto
- ❌ `name`: No puede ser nulo ni vacío.
- ❌ `nit`: No puede ser nulo ni vacío.
- ❌ `contactEmail`: No puede ser nulo ni vacío. **No se valida formato de email todavía** — el backend solo exige que no esté vacío.
- ❌ `contactPhone`: No puede ser nulo ni vacío.

> ℹ️ Todavía no hay validación de unicidad de `nit` entre empresas — el backend permite crear dos empresas con el mismo NIT. Si el negocio lo requiere, confírmalo con el equipo para agregarlo.

---

## CÓDIGOS DE ERROR COMUNES

| ErrorCode | HTTP | Causa |
|-----------|------|-------|
| NameEmpty | 400 | `name` vacío/nulo |
| NitEmpty | 400 | `nit` vacío/nulo |
| ContactEmailEmpty | 400 | `contactEmail` vacío/nulo |
| ContactPhoneEmpty | 400 | `contactPhone` vacío/nulo |
| 401 | 401 | Falta el header `Authorization` o el token expiró/es inválido |
| 500 | 500 | Error interno del servidor |
