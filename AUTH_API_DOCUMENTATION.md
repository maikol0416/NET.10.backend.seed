# Auth API - Documentación de Endpoints

## URL Base
```
http://localhost:5296/api/auth
```

## Autenticación
Todos los endpoints salvo `login`, `register` y `create-role` requieren:
```
Authorization: Bearer {token}
```
y que el usuario del token tenga el rol **`Administrator`** (`[Authorize(Roles = "Administrator")]`). Si falta el header → `401`. Si el token es válido pero el usuario no tiene el rol → `403`.

---

## 🆕 Última actualización — el rol Administrator tiene acceso total automático

Si ya integraste la versión anterior de este documento, esto es lo único que cambió desde entonces:

- **`Administrator` siempre resuelve a los 6 módulos completos** en `rolePermissions`/`permissions` (login, register, `users/paginated`, `roles/paginated`), sin importar qué se le haya asignado explícitamente. Es una regla fija del backend, no depende de configuración.
- **`PUT /roles/permissions` ahora rechaza** intentos de asignar permisos al rol `Administrator` con `400` — no tiene efecto porque ya tiene acceso total, así que el backend lo bloquea explícitamente en vez de aceptarlo silenciosamente sin hacer nada.
- **`POST /create-role`**: si `roleName` es `Administrator` y mandas `permissions`, el backend crea el rol igual pero **ignora** ese campo (no falla, simplemente no lo persiste).
- No hay cambios de contrato (mismos campos, mismos endpoints) — solo cambia el *valor* que puede venir en `permissions` para el rol `Administrator` y un nuevo caso de error en el endpoint 10.

---

## ⚠️ RESUMEN DE CAMBIOS — leer primero

El front ya tiene implementado `login`, `register` y `create-role` tal como estaban **antes** de este cambio. Todo lo demás es nuevo.

| Endpoint | Estado | Qué cambió |
|---|---|---|
| `POST /login` | 🟡 MODIFICADO | El response (`AuthResponseDto`) gana el campo `rolePermissions` (ver abajo). Nada se quitó ni renombró — es aditivo, no rompe nada existente. |
| `POST /register` | 🟡 MODIFICADO | Igual que login: gana `rolePermissions` en el response. El request no cambió. |
| `POST /create-role` | 🟡 MODIFICADO | El request (`CreateRoleDto`) gana el campo opcional `permissions: string[]`. Si no lo envías, el rol se crea sin permisos (igual que antes). |
| `GET /users/paginated` | 🟢 NUEVO | Lista usuarios paginados, incluye `rolePermissions` por usuario. |
| `GET /roles/paginated` | 🟢 NUEVO | Lista roles paginados, incluye `permissions` por rol. |
| `PUT /users/update` | 🟢 NUEVO | Actualiza email, nombre y roles de un usuario. |
| `DELETE /users/delete` | 🟢 NUEVO | Elimina un usuario. |
| `PUT /roles/update` | 🟢 NUEVO | Renombra un rol. |
| `DELETE /roles/delete` | 🟢 NUEVO | Elimina un rol (si no tiene usuarios asignados). |
| `PUT /roles/permissions` | 🟢 NUEVO | Reemplaza la lista completa de módulos permitidos de un rol. |

**Regla de negocio clave:** los permisos (módulos) se asignan al **rol**, nunca al usuario directamente. Un usuario puede tener varios roles; lo que ve el front en `rolePermissions` es el desglose de módulos **por cada rol** que tiene ese usuario — el front debe dejarlo elegir con cuál rol quiere trabajar en la sesión (ver sección "Flujo recomendado" al final).

**Regla especial:** el rol `Administrator` siempre tiene acceso a **todos** los módulos automáticamente — nunca vas a ver a `Administrator` con una lista de `permissions` incompleta en ninguna respuesta, y no se le pueden restringir permisos desde `PUT /roles/permissions` (ver detalle en cada sección).

---

## 1. LOGIN (POST) — 🟡 MODIFICADO
**Endpoint:** `POST /api/auth/login`
**Auth:** Público (`AllowAnonymous`)

**Body (Entrada - AuthLoginDto):**
```json
{
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

**cURL:**
```bash
curl -X POST http://localhost:5296/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@test.com", "password": "Admin123!"}'
```

**Response (200 OK) — 🟡 campo nuevo: `rolePermissions`:**
```json
{
  "data": {
    "token": "eyJhbGciOi...",
    "email": "admin@test.com",
    "fullName": "Administrador",
    "expiration": "2026-07-07T15:30:00Z",
    "roles": ["Administrator", "Supervisor"],
    "role": "Administrator",
    "rolePermissions": [
      {
        "roleId": "b1c2d3e4-....",
        "roleName": "Administrator",
        "permissions": ["PhysicalStructure", "Owner", "Document", "Guest", "Users", "Roles"]
      },
      {
        "roleId": "c2d3e4f5-....",
        "roleName": "Supervisor",
        "permissions": ["PhysicalStructure", "Owner"]
      }
    ]
  },
  "status": true,
  "message": "Inicio de sesión exitoso."
}
```
> `role` (singular) sigue existiendo igual que antes (es el primer elemento de `roles`) — no lo quites del front todavía si ya lo usas en algún lado, sigue funcionando.

---

## 2. REGISTER (POST) — 🟡 MODIFICADO
**Endpoint:** `POST /api/auth/register`
**Auth:** Público (`AllowAnonymous`)

**Body (Entrada - AuthRegisterDto, sin cambios):**
```json
{
  "email": "nuevo@test.com",
  "password": "Password123!",
  "fullName": "Usuario Nuevo",
  "role": "Supervisor"
}
```

**cURL:**
```bash
curl -X POST http://localhost:5296/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email": "nuevo@test.com", "password": "Password123!", "fullName": "Usuario Nuevo", "role": "Supervisor"}'
```

**Response (200 OK):** misma forma que `login`, con `rolePermissions` incluido (ver arriba).

> `role` sigue siendo un solo string en el request — el registro **no** cambió a aceptar una lista de roles. Si el usuario necesita más de un rol, se le asignan después con `PUT /users/update`.

---

## 3. CREATE ROLE (POST) — 🟡 MODIFICADO
**Endpoint:** `POST /api/auth/create-role`
**Auth:** Público (`AllowAnonymous`) — ⚠️ esto es así en el backend actual, revisa con el equipo si se restringirá a Admin más adelante.

**Body (Entrada - CreateRoleDto) — 🟡 campo nuevo opcional `permissions`:**
```json
{
  "roleName": "Supervisor",
  "permissions": ["PhysicalStructure", "Owner"]
}
```
Si no necesitas asignar permisos al crear, omite `permissions` o envía `[]`/`null` — el rol se crea sin módulos, igual que antes de este cambio.

> 🆕 Si `roleName` es `Administrator`, el rol se crea correctamente pero el backend **ignora** cualquier valor en `permissions` (no falla, simplemente no lo guarda) — `Administrator` siempre tiene acceso total, sin importar lo que se le mande aquí.

**cURL:**
```bash
curl -X POST http://localhost:5296/api/auth/create-role \
  -H "Content-Type: application/json" \
  -d '{"roleName": "Supervisor", "permissions": ["PhysicalStructure", "Owner"]}'
```

**Response (200 OK, sin cambios):**
```json
{
  "data": true,
  "status": true,
  "message": "Rol creado exitosamente."
}
```

---

## 4. LISTAR USUARIOS PAGINADOS (GET) — 🟢 NUEVO
**Endpoint:** `GET /api/auth/users/paginated?pageNumber=1&pageSize=10`
**Auth:** `Bearer {token}` + rol `Administrator`

**cURL:**
```bash
curl -X GET "http://localhost:5296/api/auth/users/paginated?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}"
```

**Response (200 OK):**
```json
{
  "data": {
    "items": [
      {
        "id": "3f2f1a2b-....",
        "email": "admin@test.com",
        "fullName": "Administrador",
        "roles": ["Administrator"],
        "rolePermissions": [
          {
            "roleId": "b1c2d3e4-....",
            "roleName": "Administrator",
            "permissions": ["PhysicalStructure", "Owner", "Document", "Guest", "Users", "Roles"]
          }
        ]
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
> ⚠️ El objeto paginado **no** trae `totalPages` — calcúlalo en el front: `Math.ceil(totalCount / pageSize)`.

---

## 5. LISTAR ROLES PAGINADOS (GET) — 🟢 NUEVO
**Endpoint:** `GET /api/auth/roles/paginated?pageNumber=1&pageSize=10`
**Auth:** `Bearer {token}` + rol `Administrator`

**cURL:**
```bash
curl -X GET "http://localhost:5296/api/auth/roles/paginated?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {token}"
```

**Response (200 OK):**
```json
{
  "data": {
    "items": [
      {
        "id": "b1c2d3e4-....",
        "name": "Administrator",
        "permissions": ["PhysicalStructure", "Owner", "Document", "Guest", "Users", "Roles"]
      },
      {
        "id": "c2d3e4f5-....",
        "name": "Supervisor",
        "permissions": ["PhysicalStructure", "Owner"]
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

---

## 6. ACTUALIZAR USUARIO (PUT) — 🟢 NUEVO
**Endpoint:** `PUT /api/auth/users/update`
**Auth:** `Bearer {token}` + rol `Administrator`

**Body (Entrada - UpdateUserDto):**
```json
{
  "id": "3f2f1a2b-....",
  "email": "usuario.editado@test.com",
  "fullName": "Usuario Editado",
  "roles": ["Supervisor", "Administrator"]
}
```
`roles` es la lista **completa** final de roles del usuario (reemplaza, no hace merge) — si el usuario tenía `["Supervisor"]` y envías `["Administrator"]`, pierde `Supervisor` y gana `Administrator`.

**cURL:**
```bash
curl -X PUT http://localhost:5296/api/auth/users/update \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"id": "3f2f1a2b-....", "email": "usuario.editado@test.com", "fullName": "Usuario Editado", "roles": ["Supervisor"]}'
```

**Response (200 OK):**
```json
{ "data": true, "status": true, "message": "Usuario actualizado exitosamente." }
```

**Errores de negocio esperables (HTTP 400, mensaje en `message`/excepción de dominio):**
- Email ya usado por otro usuario.
- Intentar quitar el rol `Administrator` al **último** usuario que lo tiene.
- Un rol de la lista `roles` no existe (debe crearse primero con `create-role`).

---

## 7. ELIMINAR USUARIO (DELETE) — 🟢 NUEVO
**Endpoint:** `DELETE /api/auth/users/delete?userId={id}`
**Auth:** `Bearer {token}` + rol `Administrator`

**cURL:**
```bash
curl -X DELETE "http://localhost:5296/api/auth/users/delete?userId=3f2f1a2b-...." \
  -H "Authorization: Bearer {token}"
```

**Response (200 OK):**
```json
{ "data": true, "status": true, "message": "Usuario eliminado exitosamente." }
```

**Errores de negocio esperables:**
- Un usuario **no puede eliminarse a sí mismo** (se compara contra el usuario del token).
- No se puede eliminar al **último** usuario con rol `Administrator`.

---

## 8. ACTUALIZAR ROL — RENOMBRAR (PUT) — 🟢 NUEVO
**Endpoint:** `PUT /api/auth/roles/update`
**Auth:** `Bearer {token}` + rol `Administrator`

**Body (Entrada - UpdateRoleDto):**
```json
{
  "id": "c2d3e4f5-....",
  "name": "Supervisor Senior"
}
```
> Este endpoint **solo renombra**. No toca los permisos del rol — para eso usa el endpoint 10 (`roles/permissions`).

**cURL:**
```bash
curl -X PUT http://localhost:5296/api/auth/roles/update \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"id": "c2d3e4f5-....", "name": "Supervisor Senior"}'
```

**Response (200 OK):**
```json
{ "data": true, "status": true, "message": "Rol actualizado exitosamente." }
```

**Errores de negocio esperables:** ya existe otro rol con ese nombre.

---

## 9. ELIMINAR ROL (DELETE) — 🟢 NUEVO
**Endpoint:** `DELETE /api/auth/roles/delete?roleId={id}`
**Auth:** `Bearer {token}` + rol `Administrator`

**cURL:**
```bash
curl -X DELETE "http://localhost:5296/api/auth/roles/delete?roleId=c2d3e4f5-...." \
  -H "Authorization: Bearer {token}"
```

**Response (200 OK):**
```json
{ "data": true, "status": true, "message": "Rol eliminado exitosamente." }
```

**Errores de negocio esperables:** el rol tiene usuarios asignados — el front debe reasignarlos (vía `users/update`) antes de poder borrar el rol.

---

## 10. ASIGNAR PERMISOS A UN ROL (PUT) — 🟢 NUEVO
**Endpoint:** `PUT /api/auth/roles/permissions`
**Auth:** `Bearer {token}` + rol `Administrator`

**Body (Entrada - AssignRolePermissionsDto):**
```json
{
  "roleId": "c2d3e4f5-....",
  "permissions": ["PhysicalStructure", "Owner", "Guest"]
}
```
> ⚠️ **Reemplaza la lista completa**, no es incremental. Si el rol ya tenía `["PhysicalStructure", "Document"]` y envías `["Owner"]`, el rol queda con **solo** `["Owner"]` — `Document` y `PhysicalStructure` se quitan. El front siempre debe enviar el array final completo (ej. un multi-select con todos los módulos marcados/desmarcados).

**cURL:**
```bash
curl -X PUT http://localhost:5296/api/auth/roles/permissions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"roleId": "c2d3e4f5-....", "permissions": ["PhysicalStructure", "Owner", "Guest"]}'
```

**Response (200 OK):**
```json
{ "data": true, "status": true, "message": "Permisos actualizados exitosamente." }
```

**Errores de negocio esperables:**
- Algún valor de `permissions` no es un módulo válido (ver catálogo abajo) → `400` con el detalle por campo.
- 🆕 `roleId` corresponde al rol `Administrator` → `400`, mensaje: *"El rol Administrator siempre tiene acceso a todos los módulos; no es necesario ni posible asignarle permisos."* Si tu UI de gestión de permisos tiene un multi-select por rol, deshabilítalo (o muéstralo con los 6 módulos marcados y sin edición) cuando el rol seleccionado sea `Administrator`, para no dejar que el usuario intente guardar y se encuentre con el error.

---

## ESTRUCTURA DE OBJETOS DTO (TypeScript)

```typescript
interface AuthResponseDto {
  token: string;
  email: string;
  fullName: string;
  expiration: string;              // ISO 8601 DateTime
  roles: string[];
  role: string;                    // = roles[0], se mantiene por compatibilidad
  rolePermissions: RolePermissionsDto[];  // 🆕
}

interface RolePermissionsDto {     // 🆕
  roleId: string;
  roleName: string;
  permissions: string[];           // nombres de ModuleEnum
}

interface UserDto {
  id: string;
  email: string;
  fullName: string;
  roles: string[];
  rolePermissions: RolePermissionsDto[];  // 🆕
}

interface RoleDto {
  id: string;
  name: string;
  permissions: string[];           // 🆕 — nombres de ModuleEnum
}

interface CreateRoleDto {
  roleName: string;                // max 50 chars, requerido
  permissions?: string[];          // 🆕 opcional — nombres de ModuleEnum
}

interface UpdateUserDto {          // 🆕
  id: string;                      // requerido
  email: string;                   // requerido, formato email válido
  fullName: string;                // requerido, max 200 chars
  roles: string[];                 // lista completa final (reemplaza)
}

interface UpdateRoleDto {          // 🆕
  id: string;                      // requerido
  name: string;                    // requerido, max 50 chars
}

interface AssignRolePermissionsDto {  // 🆕
  roleId: string;                  // requerido
  permissions: string[];           // lista completa final (reemplaza) — nombres de ModuleEnum
}

// Paginación genérica — usada por users/paginated y roles/paginated
interface PaginatedList<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  // NO incluye totalPages — calcúlalo: Math.ceil(totalCount / pageSize)
}
```

---

## CATÁLOGO DE MÓDULOS (ModuleEnum)

Los valores válidos para cualquier campo `permissions`/`Permissions` (en `CreateRoleDto`, `AssignRolePermissionsDto`, y los que devuelven `RoleDto.permissions` / `RolePermissionsDto.permissions`) son **exactamente** estos strings (case-sensitive):

```
PhysicalStructure
Owner
Document
Guest
Users
Roles
```

Cualquier otro valor es rechazado por el validador con `400` (`InvalidModuleName`). Si necesitas agregar un módulo nuevo, requiere un cambio de backend (`Domain/DomainShared/ModuleEnum.cs`) — no es un catálogo editable desde el front.

---

## VALIDACIONES CRÍTICAS

### CreateRoleDto
- ❌ `roleName`: obligatorio, max 50 caracteres.
- ❌ `permissions[i]`: si se envía el array, cada elemento debe ser un nombre válido del catálogo de módulos.

### UpdateUserDto
- ❌ `id`: obligatorio.
- ❌ `email`: obligatorio, formato de email válido.
- ❌ `fullName`: obligatorio, max 200 caracteres.

### UpdateRoleDto
- ❌ `id`: obligatorio.
- ❌ `name`: obligatorio, max 50 caracteres.

### AssignRolePermissionsDto
- ❌ `roleId`: obligatorio.
- ❌ `permissions[i]`: cada elemento debe ser un nombre válido del catálogo de módulos.

---

## REGLAS DE NEGOCIO (no son errores de validación de formato, son 400 de dominio)

| Regla | Dónde aplica |
|---|---|
| Un usuario no puede eliminarse a sí mismo | `DELETE /users/delete` |
| No se puede eliminar ni quitarle el rol `Administrator` al último usuario que lo tiene | `DELETE /users/delete`, `PUT /users/update` |
| No se puede eliminar un rol que tiene usuarios asignados | `DELETE /roles/delete` |
| No se puede renombrar un rol a un nombre ya usado por otro rol | `PUT /roles/update` |
| No se puede crear un rol con un nombre ya existente | `POST /create-role` |
| `AssignRolePermissionsDto.permissions` y `CreateRoleDto.permissions` reemplazan la lista completa, no son incrementales | `PUT /roles/permissions`, `POST /create-role` |
| Los permisos se asignan al **rol**, nunca directamente al usuario | Todo el módulo |
| 🆕 El rol `Administrator` siempre tiene acceso a todos los módulos automáticamente; no se le pueden asignar/restringir permisos | `login`, `register`, `users/paginated`, `roles/paginated`, `PUT /roles/permissions`, `POST /create-role` |

---

## CÓDIGOS DE ERROR COMUNES

| ErrorCode | HTTP | Causa |
|---|---|---|
| RoleNameEmpty / RoleNameTooLong | 400 | `roleName` inválido en create-role |
| InvalidModuleName | 400 | Un valor de `permissions` no está en el catálogo de `ModuleEnum` |
| IdEmpty / RoleIdEmpty | 400 | Falta `id`/`roleId` en update/permissions |
| EmailEmpty / EmailInvalid | 400 | Email inválido en `UpdateUserDto` |
| FullNameEmpty / FullNameTooLong | 400 | `fullName` inválido |
| (mensaje libre de dominio) | 400 | Reglas de negocio de la tabla de arriba (último admin, rol en uso, email duplicado, intento de asignar permisos a `Administrator`, etc.) — vienen en el texto de la excepción, no tienen ErrorCode fijo |
| 401 | 401 | Falta el header `Authorization` o el token expiró/es inválido |
| 403 | 403 | El usuario del token no tiene el rol `Administrator` |

---

## Flujo recomendado en el front para selección de rol activo

1. Tras `login`/`register`, guarda `rolePermissions` completo (no solo `roles`).
2. Si `rolePermissions.length === 1`, trabaja directo con ese rol — no hace falta selector.
3. Si `rolePermissions.length > 1`, muestra un selector ("¿con cuál rol quieres trabajar?") usando `roleName` de cada entrada.
4. Al elegir un rol, usa **su** array `permissions` (no la unión de todos) para decidir qué módulos/menús mostrar en la UI. Esto es puramente una decisión de UI — el backend no tiene noción de "rol activo de la sesión", el JWT sigue trayendo todos los roles del usuario en el claim de roles.
5. Si el usuario cambia de rol de trabajo dentro de la misma sesión, no hace falta volver a loguearse — el front simplemente cambia qué `permissions` usa para renderizar, ya tiene el desglose completo desde el login.
