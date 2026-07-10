# Auth API - Documentación de Endpoints

## URL Base
```
http://localhost:5296/api/auth
```

## Autenticación
Todos los endpoints salvo `login` y `create-role` requieren:
```
Authorization: Bearer {token}
```
y que el usuario del token tenga el rol **`Administrator`** (`[Authorize(Roles = "Administrator")]`). Si falta el header → `401`. Si el token es válido pero el usuario no tiene el rol → `403`.

> 🆕 `register` es un caso especial: sigue sin el atributo `[Authorize]` a nivel HTTP (por eso no puede devolver `401`/`403` "de framework"), pero **el propio handler exige el header `Authorization` en la práctica** salvo para el registro del primer `Administrator` del sistema. Ver sección 2 para el detalle completo — no lo trates como un endpoint público más.

---

## 🆕 Última actualización — companyId también en consulta y edición de usuarios

La actualización anterior (justo abajo) ya había agregado `companyId` a `login`/`register`. Esta actualización completa el ciclo: ahora también se puede **ver** y **editar** la empresa de un usuario existente.

- **`GET /users/paginated`**: cada item de `UserDto` gana `companyId: string | null`.
- **`PUT /users/update`**: el body (`UpdateUserDto`) gana `companyId?: string | null` — opcional, para reasignar la empresa del usuario.
- **Misma regla que en `register`**: si `roles` incluye `Administrator`, el backend ignora cualquier `companyId` que mandes y lo deja en `null` — un usuario de plataforma nunca pertenece a una empresa.
- **Mismo error que en `register`** si mandas un `companyId` que no existe: `400`, mensaje *"La empresa indicada no existe."*
- Como `PUT /users/update` ya requería rol `Administrator` para poder llamarlo, no hay reglas adicionales de "quién puede reasignar la empresa de quién" — cualquier `Administrator` puede reasignar la empresa de cualquier usuario.
- No hay cambios de contrato en ningún otro endpoint — es aditivo en los dos DTOs mencionados arriba.

---

## 🆕 Actualización anterior — multi-tenant: usuarios atados a una empresa

Cada usuario (salvo `Administrator`, que es un rol de plataforma) ahora pertenece a **una única empresa** (`companyId`). Esto afecta `login` y `register`:

- **`AuthResponseDto` gana `companyId: string | null`** en `login` y `register` — aditivo, no rompe nada existente.
- **`AuthRegisterDto` gana `companyId?: string`** — opcional, y **casi nunca lo vas a mandar** (ver sección 2 para las reglas exactas de cuándo sí).
- **`register` cambia sus reglas de negocio**: ya no es un registro libre. El backend valida quién puede registrar a quién según el token de quien hace la llamada. Ver la tabla completa en la sección 2.
- Para dar de alta empresas y obtener el listado que alimenta el selector de `companyId`, usa el módulo `ManagementCompany` — ver [`COMPANY_API_DOCUMENTATION.md`](./COMPANY_API_DOCUMENTATION.md).

---

## 🆕 Dos actualizaciones atrás — el rol Administrator tiene acceso total automático

Esto es lo que cambió en la actualización previa a la de multi-tenant:

- **`Administrator` siempre resuelve a los 6 módulos completos** en `rolePermissions`/`permissions` (login, register, `users/paginated`, `roles/paginated`), sin importar qué se le haya asignado explícitamente. Es una regla fija del backend, no depende de configuración.
- **`PUT /roles/permissions` ahora rechaza** intentos de asignar permisos al rol `Administrator` con `400` — no tiene efecto porque ya tiene acceso total, así que el backend lo bloquea explícitamente en vez de aceptarlo silenciosamente sin hacer nada.
- **`POST /create-role`**: si `roleName` es `Administrator` y mandas `permissions`, el backend crea el rol igual pero **ignora** ese campo (no falla, simplemente no lo persiste).
- No hay cambios de contrato (mismos campos, mismos endpoints) — solo cambia el *valor* que puede venir en `permissions` para el rol `Administrator` y un nuevo caso de error en el endpoint 10.

---

## ⚠️ RESUMEN DE CAMBIOS — leer primero

El front ya tiene implementado `login`, `register` y `create-role` tal como estaban **antes** de este cambio. Todo lo demás es nuevo.

| Endpoint | Estado | Qué cambió |
|---|---|---|
| `POST /login` | 🟡 MODIFICADO | El response (`AuthResponseDto`) gana `rolePermissions` y 🆕 `companyId`. Nada se quitó ni renombró — es aditivo, no rompe nada existente. |
| `POST /register` | 🟡 MODIFICADO | Gana `rolePermissions` y 🆕 `companyId` en el response, y 🆕 `companyId` opcional en el request. 🆕 Además, las reglas de quién puede registrar a quién cambiaron por completo — ver sección 2. |
| `POST /create-role` | 🟡 MODIFICADO | El request (`CreateRoleDto`) gana el campo opcional `permissions: string[]`. Si no lo envías, el rol se crea sin permisos (igual que antes). |
| `GET /users/paginated` | 🟢 NUEVO | Lista usuarios paginados, incluye `rolePermissions` y 🆕 `companyId` por usuario. |
| `GET /roles/paginated` | 🟢 NUEVO | Lista roles paginados, incluye `permissions` por rol. |
| `PUT /users/update` | 🟢 NUEVO | Actualiza email, nombre, roles y 🆕 `companyId` de un usuario. |
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

**Response (200 OK) — 🟡 campos nuevos: `rolePermissions` y `companyId`:**
```json
{
  "data": {
    "token": "eyJhbGciOi...",
    "email": "gerente@losrobles.com",
    "fullName": "Gerente Los Robles",
    "expiration": "2026-07-07T15:30:00Z",
    "roles": ["Supervisor"],
    "role": "Supervisor",
    "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b",
    "rolePermissions": [
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

### 🆕 De dónde sale `companyId` y qué hacer con él

- **No lo pides tú, no lo calculas tú** — lo devuelve el backend en cada `login`/`register`, ya resuelto. El front solo lo **guarda** (junto al token, igual que `roles`) y lo usa para lo que necesite mostrar en UI (ej. "Empresa: Los Robles", si decides pedir el nombre por separado a `GET /api/managementcompany/getById?id={companyId}`).
- **Vale `null` únicamente cuando el usuario logueado tiene el rol `Administrator`** (rol de plataforma, no pertenece a ninguna empresa). Para cualquier otro rol, `companyId` siempre trae un UUID válido.
- Úsalo para decidir si mostrar el selector de empresa en la pantalla de registro (ver sección 2): `esAdministradorDePlataforma = roles.includes("Administrator")`.

---

## 2. REGISTER (POST) — 🟡 MODIFICADO EN PROFUNDIDAD (multi-tenant)
**Endpoint:** `POST /api/auth/register`
**Auth:** Técnicamente sigue siendo `[AllowAnonymous]` a nivel HTTP (necesario para poder crear al primer `Administrator` en una base vacía), pero 🆕 **el backend ahora valida quién puede registrar a quién** según el token con el que se llama (o la ausencia de token). Ya no es un endpoint de registro libre — si tu front hoy lo llama siempre sin `Authorization`, **se va a romper** para cualquier caso que no sea el bootstrap inicial. Lee esta sección completa antes de tocar la pantalla de registro/invitación de usuarios.

### Body (Entrada - AuthRegisterDto) — 🆕 campo nuevo opcional `companyId`
```json
{
  "email": "nuevo@test.com",
  "password": "Password123!",
  "fullName": "Usuario Nuevo",
  "role": "Supervisor",
  "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
}
```

### 🆕 Reglas de negocio — quién puede registrar a quién, y de dónde sale `companyId`

| # | ¿Quién llama? | `role` que se pide | ¿Mando `companyId`? | Qué pasa |
|---|---|---|---|---|
| 1 | Nadie (sin header `Authorization`) | `Administrator` | No | ✅ Permitido **solo si todavía no existe ningún `Administrator`** en el sistema (bootstrap, caso único de arranque). El usuario queda sin empresa (`companyId: null`). |
| 2 | Usuario logueado con rol `Administrator` | `Administrator` | No | ✅ Permitido. El nuevo usuario también queda sin empresa. |
| 3 | Usuario logueado con rol `Administrator` | Cualquier otro rol | **Sí, obligatorio** | ✅ Permitido. El nuevo usuario queda en la empresa indicada. El front debe mostrar un **selector de empresa** en este caso (ver abajo de dónde sacarlo). |
| 4 | Usuario logueado con cualquier rol que **no** sea `Administrator` | Cualquier rol ≠ `Administrator` | **No lo mandes** (si lo mandas, se ignora) | ✅ Permitido. El nuevo usuario hereda automáticamente **la misma empresa** de quien está haciendo la llamada. |
| — | Sin header `Authorization`, fuera del caso 1 | — | — | ❌ `400` — "Debes iniciar sesión para registrar usuarios." |

**En la práctica, el front solo necesita dos flujos de UI:**

- **Pantalla "invitar usuario a mi empresa"** (la inmensa mayoría de los casos, caso 4 de la tabla): el usuario logueado (con cualquier rol que no sea `Administrator`) llena el formulario **sin** campo de empresa — ni lo muestres, ni lo mandes en el body. El backend ya sabe a qué empresa pertenece quien invita y se la asigna automáticamente al nuevo usuario.
- **Pantalla de plataforma "crear usuario para una empresa"** (uso exclusivo de `Administrator`, caso 3 de la tabla): sí necesitas un selector de empresa. Se alimenta con `GET /api/managementcompany/getAll` (ver [`COMPANY_API_DOCUMENTATION.md`](./COMPANY_API_DOCUMENTATION.md#4-obtener-todas-las-empresas-get)) — listas el `name` de cada empresa y mandas su `id` como `companyId` en el body de `register`.

Para saber cuál de las dos pantallas mostrar, revisa el `roles` que guardaste del `login`/`register` de la sesión actual: `if (roles.includes("Administrator")) { /* mostrar selector de empresa */ } else { /* invitar sin selector */ }`.

**cURL — caso 4 (el común): invitar un compañero de mi propia empresa, sin `companyId`:**
```bash
curl -X POST http://localhost:5296/api/auth/register \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"email": "nuevo@test.com", "password": "Password123!", "fullName": "Usuario Nuevo", "role": "Supervisor"}'
```

**cURL — caso 3: un Administrator crea un usuario para una empresa específica:**
```bash
curl -X POST http://localhost:5296/api/auth/register \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {tokenDeAdministrator}" \
  -d '{
    "email": "gerente@losrobles.com",
    "password": "Password123!",
    "fullName": "Gerente Los Robles",
    "role": "Supervisor",
    "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
  }'
```

**Response (200 OK):** misma forma que `login`, con `rolePermissions` y 🆕 `companyId` incluidos (ver sección 1) — el `companyId` de la respuesta es el que efectivamente quedó asignado (útil para confirmar en UI, aunque el front no lo haya mandado).

> `role` sigue siendo un solo string en el request — el registro **no** cambió a aceptar una lista de roles. Si el usuario necesita más de un rol, se le asignan después con `PUT /users/update`.

### 🆕 Nuevos errores de negocio (400, mensaje libre de dominio — sin `ErrorCode` fijo, viene en el texto de `message`)

| Mensaje (contiene) | Cuándo ocurre | Qué debe hacer el front |
|---|---|---|
| "Solo un administrador de la plataforma puede crear otro administrador." | Alguien que no es `Administrator` intentó registrar `role: "Administrator"` (y ya existe uno). | No debería pasar si ocultas la opción de rol `Administrator` en el selector de roles para cualquiera que no sea `Administrator`. |
| "Debes indicar la empresa (CompanyId) para el nuevo usuario." | Un `Administrator` mandó `register` sin `companyId` para un rol que no es `Administrator`. | Valida en el front que `companyId` sea obligatorio antes de enviar, cuando quien llama es `Administrator`. |
| "La empresa indicada no existe." | El `companyId` mandado no corresponde a ninguna empresa real. | No debería pasar si el selector se alimenta de `GET /managementcompany/getAll` y no de un valor tipeado a mano. |
| "Tu usuario no pertenece a ninguna empresa; no puedes registrar usuarios." | Caso anómalo: usuario autenticado, no `Administrator`, sin empresa asignada. | Contactar soporte/backoffice — es un estado de datos inconsistente, no un error de UI. |
| "Debes iniciar sesión para registrar usuarios." | Se llamó sin `Authorization` fuera del caso de bootstrap. | Asegúrate de mandar el header `Authorization: Bearer {token}` en toda invitación de usuario, salvo el registro inicial del primer `Administrator`. |

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
        ],
        "companyId": null
      },
      {
        "id": "4a3b2c1d-....",
        "email": "gerente@losrobles.com",
        "fullName": "Gerente Los Robles",
        "roles": ["Supervisor"],
        "rolePermissions": [
          {
            "roleId": "c2d3e4f5-....",
            "roleName": "Supervisor",
            "permissions": ["PhysicalStructure", "Owner"]
          }
        ],
        "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
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
> ⚠️ El objeto paginado **no** trae `totalPages` — calcúlalo en el front: `Math.ceil(totalCount / pageSize)`.
> 🆕 `companyId` es `null` para usuarios de plataforma (`Administrator`) y un GUID para cualquier otro usuario.

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

**Body (Entrada - UpdateUserDto) — 🆕 campo nuevo opcional `companyId`:**
```json
{
  "id": "3f2f1a2b-....",
  "email": "usuario.editado@test.com",
  "fullName": "Usuario Editado",
  "roles": ["Supervisor"],
  "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
}
```
`roles` es la lista **completa** final de roles del usuario (reemplaza, no hace merge) — si el usuario tenía `["Supervisor"]` y envías `["Administrator"]`, pierde `Supervisor` y gana `Administrator`.

> 🆕 `companyId` reemplaza la empresa actual del usuario. Envía `null` (u omite el campo) para dejarlo sin empresa. **Si `roles` incluye `Administrator`, el backend ignora `companyId` y lo deja en `null`** sin importar qué mandes — un usuario de plataforma nunca pertenece a una empresa.

**cURL:**
```bash
curl -X PUT http://localhost:5296/api/auth/users/update \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "id": "3f2f1a2b-....",
    "email": "usuario.editado@test.com",
    "fullName": "Usuario Editado",
    "roles": ["Supervisor"],
    "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
  }'
```

**Response (200 OK):**
```json
{ "data": true, "status": true, "message": "Usuario actualizado exitosamente." }
```

**Errores de negocio esperables (HTTP 400, mensaje en `message`/excepción de dominio):**
- Email ya usado por otro usuario.
- Intentar quitar el rol `Administrator` al **último** usuario que lo tiene.
- Un rol de la lista `roles` no existe (debe crearse primero con `create-role`).
- 🆕 "La empresa indicada no existe." — el `companyId` mandado no corresponde a ninguna empresa real. No debería pasar si el selector se alimenta de `GET /managementcompany/getAll` y no de un valor tipeado a mano.

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
  companyId: string | null;        // 🆕 UUID — null solo para usuarios con rol Administrator
  rolePermissions: RolePermissionsDto[];  // 🆕
}

interface AuthRegisterDto {        // 🆕 (request de POST /register)
  email: string;
  password: string;
  fullName: string;
  role: string;
  companyId?: string;              // 🆕 UUID — SOLO lo manda un Administrator (ver sección 2).
                                    // Cualquier otro usuario lo omite: el backend ignora lo que
                                    // venga acá y usa la empresa de quien está autenticado.
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
  companyId: string | null;               // 🆕 null = usuario de plataforma (Administrator)
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
  companyId?: string | null;       // 🆕 opcional — ignorado (queda null) si roles incluye Administrator
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
| 🆕 Un usuario nuevo hereda automáticamente la empresa (`companyId`) de quien lo registra; nunca puede elegir la suya propia | `POST /register` |
| 🆕 Un usuario con rol `Administrator` nunca queda atado a una empresa — `companyId` siempre se fuerza a `null`, sin importar lo que se envíe | `POST /register`, `PUT /users/update` |
| 🆕 El `companyId` enviado debe corresponder a una empresa existente (`ManagementCompany`) | `POST /register`, `PUT /users/update` |
| 🆕 Solo un `Administrator` puede indicar `companyId` explícito al registrar (y solo porque él mismo no pertenece a ninguna empresa) | `POST /register` |
| 🆕 Solo un `Administrator` puede registrar a otro usuario con rol `Administrator` (salvo el primero del sistema, bootstrap) | `POST /register` |

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
