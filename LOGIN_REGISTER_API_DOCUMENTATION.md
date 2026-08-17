# Login & Registro — Documentación para Frontend

Documento enfocado **solo** en autenticación (login + registro) para implementar un cliente nuevo. Para el resto del módulo de usuarios/roles (listar, editar, permisos, etc.) ver [`AUTH_API_DOCUMENTATION.md`](./AUTH_API_DOCUMENTATION.md).

## URL Base (Development)

```
http://localhost:5297/api/Auth
```

También disponible por HTTPS: `https://localhost:7138/api/Auth` (si se pega al puerto HTTP en modo `https`, el servidor redirige con 307 automáticamente).

> Las rutas son case-insensitive (`/api/Auth/login` y `/api/auth/login` funcionan igual), pero usa `Auth` con mayúscula para que coincida con el código fuente.

Documentación interactiva (Scalar, solo en Development): `https://localhost:7138/scalar/v1` — tiene un esquema de seguridad "Bearer" preconfigurado, útil para probar el token pegándolo ahí.

### CORS — importante antes de integrar

El backend solo permite peticiones desde estos orígenes (`Api/Program.cs`):
```
http://localhost:4200
http://localhost:4201
```
Si tu app corre en otro puerto (ej. Vite `5173`, Next.js `3000`), **las llamadas van a fallar por CORS** hasta que se agregue tu origen a la política `AllowFrontend` en el backend. Avisa al equipo de backend qué puerto vas a usar.

---

## Formato de respuesta (envelope)

Toda respuesta, exitosa o con error, tiene esta forma (`camelCase`):

```json
{
  "status": true,
  "message": "texto descriptivo",
  "data": { }
}
```

- `status`: `true` en éxito, `false` en error.
- `message`: texto humano (en español). En errores de validación tiene un formato especial, ver abajo.
- `data`: el payload en éxito, o `null` en error.

### Manejo de errores

| HTTP | Cuándo | Forma de `message` |
|---|---|---|
| 400 | Credenciales inválidas, reglas de negocio (email duplicado, rol inexistente, etc.) | Texto plano, ej. `"Credenciales inválidas."` |
| 400 | Falla de validación de campos (FluentValidation) | **String JSON serializado dos veces** — ver ejemplo abajo, hay que hacer `JSON.parse` sobre `message` |
| 500 | Error inesperado del servidor | Texto genérico: `"An unexpected error occurred. Please try again later."` |

Ejemplo de error de validación (nota que `message` es un string que a su vez contiene JSON):
```json
{
  "status": false,
  "message": "[{\"PropertyName\":\"Email\",\"AttemptedValue\":\"no-es-un-email\",\"ErrorCode\":\"EmailInvalid\",\"ErrorMessage\":\"El formato del email no es válido.\",\"Severity\":0}]",
  "data": null
}
```
Para mostrar errores por campo en el formulario, el front debe parsear `message` como JSON una segunda vez:
```ts
type FieldError = { PropertyName: string; ErrorMessage: string; ErrorCode: string };

function parseValidationErrors(message: string): FieldError[] | null {
  try {
    const parsed = JSON.parse(message);
    return Array.isArray(parsed) ? parsed : null;
  } catch {
    return null; // no era un error de validación, es un mensaje de negocio en texto plano
  }
}
```

No hay `ProblemDetails` estándar de ASP.NET ni códigos de error HTTP granulares más allá de 400/401/403/500 — todo pasa por este envelope.

---

## Autenticación (JWT Bearer)

- El token se recibe en `data.token` al hacer login o registro exitoso.
- No es una cookie: el front debe guardarlo (ej. `localStorage`) y mandarlo manualmente en cada request protegido:
  ```
  Authorization: Bearer {token}
  ```
- **No existe refresh token ni endpoint de logout.** El JWT es stateless; al expirar, el usuario debe volver a hacer login. "Cerrar sesión" en el front es simplemente borrar el token guardado.
- **Duración real del token**: 120 minutos en Development (`Jwt:ExpirationMinutes` en `appsettings.Development.json`), 60 minutos en el resto de entornos.
- ⚠️ **Bug conocido**: el campo `data.expiration` que devuelve `login`/`register` está **hardcodeado a +60 minutos** y no refleja la duración real configurada (120 min en Development). **No confíes en ese campo** para decidir cuándo expira la sesión — decodifica el claim `exp` del JWT en su lugar (ej. con `jwt-decode`), o simplemente maneja el `401` cuando ocurra y redirige a login.
- Un `401` en cualquier endpoint protegido significa: falta el header `Authorization` o el token expiró/es inválido. Un `403` significa: el token es válido pero el usuario no tiene el rol requerido para ese endpoint.

---

## 1. LOGIN

**`POST /api/Auth/login`** — público (`AllowAnonymous`).

### Request body
```json
{
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

| Campo | Tipo | Validación |
|---|---|---|
| `email` | string | Requerido, formato de email válido |
| `password` | string | Requerido (sin regla de longitud mínima en login) |

### cURL
```bash
curl -X POST http://localhost:5297/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@test.com", "password": "Admin123!"}'
```

### Respuesta exitosa (200 OK)
```json
{
  "status": true,
  "message": "Inicio de sesión exitoso.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "admin@test.com",
    "fullName": "Administrador",
    "expiration": "2026-08-04T16:30:00Z",
    "roles": ["Administrator"],
    "role": "Administrator",
    "companyId": null,
    "rolePermissions": [
      {
        "roleId": "b1c2d3e4-....",
        "roleName": "Administrator",
        "permissions": ["PhysicalStructure", "Owner", "Document", "Guest", "Users", "Roles", "ManagementCompany"]
      }
    ]
  }
}
```

| Campo de `data` | Tipo | Notas |
|---|---|---|
| `token` | string | JWT, guardarlo y usarlo en `Authorization: Bearer` |
| `email` | string | |
| `fullName` | string | |
| `expiration` | string (ISO datetime) | ⚠️ no confiable, ver sección de JWT arriba |
| `roles` | string[] | todos los roles del usuario |
| `role` | string | = `roles[0]`, conveniencia |
| `companyId` | string \| null | `null` únicamente si el usuario tiene rol `Administrator` (rol de plataforma). Para cualquier otro rol, siempre trae un UUID |
| `rolePermissions` | array | desglose de módulos por cada rol que tiene el usuario — útil si el usuario tiene más de un rol y necesita elegir con cuál trabajar |

### Errores esperables
- **400** — credenciales inválidas: `{"status":false,"message":"Credenciales inválidas.","data":null}`. Se devuelve el mismo mensaje tanto si el email no existe como si la contraseña es incorrecta (no reveles cuál de las dos falló en la UI, por seguridad).
- **400** — validación de formato (email vacío/inválido, password vacío) — ver formato de error de validación arriba.
- **500** — error inesperado.

---

## 2. REGISTRO

**`POST /api/Auth/register`** — técnicamente `AllowAnonymous` a nivel HTTP, pero **el handler exige estar autenticado en la práctica**, salvo para crear al primer `Administrator` del sistema. No es un endpoint de registro libre tipo "cualquiera se crea una cuenta" — léelo completo antes de implementar la pantalla.

### Request body
```json
{
  "email": "nuevo@test.com",
  "password": "Password123!",
  "fullName": "Usuario Nuevo",
  "role": "Supervisor",
  "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
}
```

| Campo | Tipo | Validación |
|---|---|---|
| `email` | string | Requerido, formato de email válido, único en el sistema |
| `password` | string | Requerido, mínimo 6 caracteres, **al menos 1 dígito, 1 mayúscula y 1 minúscula** (regla de ASP.NET Identity, no solo del validador de campo) |
| `fullName` | string | Requerido, máx 200 caracteres |
| `role` | string | Requerido, máx 50 caracteres. **El rol debe existir de antemano** (creado vía `POST /api/Auth/create-role`) — si no existe, error 400 |
| `companyId` | string (UUID) | Opcional — ver tabla de reglas de negocio abajo, casi nunca lo manda el front directamente |

### Reglas de negocio — quién puede registrar a quién

| Caso | ¿Quién llama? | `role` pedido | ¿Mando `companyId`? | Resultado |
|---|---|---|---|---|
| 1 | Nadie (sin header `Authorization`) | `Administrator` | No | ✅ Permitido **solo si no existe ningún `Administrator` todavía** en el sistema (bootstrap inicial). Queda con `companyId: null`. |
| 2 | Usuario logueado con rol `Administrator` | `Administrator` | No | ✅ Permitido. Nuevo usuario sin empresa. |
| 3 | Usuario logueado con rol `Administrator` | Cualquier otro rol | **Sí, obligatorio** | ✅ Permitido. El nuevo usuario queda en la empresa indicada — mostrar selector de empresa en la UI. |
| 4 | Usuario logueado con cualquier rol ≠ `Administrator` | Cualquier rol ≠ `Administrator` | No lo mandes (se ignora si lo mandas) | ✅ Permitido. El nuevo usuario hereda automáticamente la empresa de quien invita. |
| — | Sin `Authorization`, fuera del caso 1 | — | — | ❌ 400 — "Debes iniciar sesión para registrar usuarios." |

**En la práctica, dos flujos de UI:**

- **Registro inicial (bootstrap)**: solo aplica la primerísima vez que se levanta el sistema, sin usuarios todavía. Rol `Administrator`, sin token, sin `companyId`. Es un caso único, no construyas un flujo de "registro público" pensando en que se repetirá.
- **"Invitar usuario"** (caso normal, 99% de los casos): un usuario ya logueado invita a otro. Si es `Administrator`, necesita elegir la empresa (`companyId`) desde un selector alimentado por el listado de empresas (`ManagementCompany`, fuera de este documento). Si es cualquier otro rol, ni muestres ni mandes `companyId` — se asigna solo.

### cURL — caso 1: bootstrap del primer Administrator (una sola vez, DB vacía)
```bash
curl -X POST http://localhost:5297/api/Auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@test.com",
    "password": "Admin123!",
    "fullName": "Administrador",
    "role": "Administrator"
  }'
```

### cURL — caso 4 (el común): invitar a alguien de mi propia empresa
```bash
curl -X POST http://localhost:5297/api/Auth/register \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "email": "nuevo@test.com",
    "password": "Password123!",
    "fullName": "Usuario Nuevo",
    "role": "Supervisor"
  }'
```

### cURL — caso 3: un Administrator crea un usuario para una empresa específica
```bash
curl -X POST http://localhost:5297/api/Auth/register \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {tokenDeAdministrator}" \
  -d '{
    "email": "gerente@empresa.com",
    "password": "Password123!",
    "fullName": "Gerente Empresa",
    "role": "Supervisor",
    "companyId": "3f2f1a2b-4c5d-6e7f-8a9b-0c1d2e3f4a5b"
  }'
```

### Respuesta exitosa (200 OK)
Misma forma que login, `message: "Registro exitoso."` — el `companyId` en la respuesta es el que efectivamente quedó asignado (útil para confirmar en UI aunque el front no lo haya mandado).

### Errores de negocio (400, texto libre en `message`)

| Mensaje | Cuándo | Qué hacer en el front |
|---|---|---|
| "Solo un administrador de la plataforma puede crear otro administrador." | Alguien que no es `Administrator` intentó registrar `role: "Administrator"` | No debería pasar si ocultas la opción de rol `Administrator` para cualquiera que no sea `Administrator` |
| "El rol '{role}' no existe. Debe crearlo primero." | El `role` enviado no fue creado antes con `create-role` | Alimenta el selector de rol desde `GET /api/Auth/roles/paginated`, no un input libre |
| "Debes indicar la empresa (CompanyId) para el nuevo usuario." | Un `Administrator` mandó `register` sin `companyId` para un rol que no es `Administrator` | Validar `companyId` obligatorio en el front cuando quien llama es `Administrator` |
| "La empresa indicada no existe." | El `companyId` no corresponde a ninguna empresa real | No debería pasar si el selector viene de la API y no de un valor tipeado a mano |
| "Ya existe un usuario registrado con este email." | Email duplicado | Mostrar error en el campo email |
| "Debes iniciar sesión para registrar usuarios." | Se llamó sin `Authorization` fuera del caso bootstrap | Asegúrate de mandar el header en toda invitación |
| Errores de password de Identity (ej. "Passwords must have at least one uppercase...") | El password no cumple la política (6+ caracteres, 1 dígito, 1 mayúscula, 1 minúscula) | Validar el patrón en el front antes de enviar para dar feedback inmediato |

Además, errores de validación de formato (400, mismo formato JSON-doble-serializado que en login) para email/password/fullName/role vacíos o fuera de rango.

---

## Tipos TypeScript

```typescript
interface LoginRequest {
  email: string;
  password: string;
}

interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
  role: string;
  companyId?: string; // solo lo manda un Administrator creando usuario para otra empresa
}

interface RolePermissions {
  roleId: string;
  roleName: string;
  permissions: string[]; // nombres de ModuleEnum: PhysicalStructure, Owner, Document, Guest, Users, Roles, ManagementCompany
}

interface AuthResponse {
  token: string;
  email: string;
  fullName: string;
  expiration: string;        // ISO datetime — no confiable, decodificar el JWT en su lugar
  roles: string[];
  role: string;               // = roles[0]
  companyId: string | null;   // null solo para rol Administrator
  rolePermissions: RolePermissions[];
}

interface ApiEnvelope<T> {
  status: boolean;
  message: string;
  data: T | null;
}
```

---

## Flujo recomendado en el front

1. Formulario de login → `POST /api/Auth/login`. Si `status: false`, mostrar `message` (parseando como JSON si aplica, ver sección de errores).
2. Al recibir `data`, guardar `token`, `roles`, `role`, `companyId`, `rolePermissions` (ej. en `localStorage` + estado global). No guardar `expiration` como fuente de verdad.
3. En cada request protegido, mandar `Authorization: Bearer {token}`.
4. Si cualquier request devuelve `401`, limpiar el estado de sesión y redirigir a login.
5. Si `rolePermissions.length > 1`, dejar que el usuario elija con qué rol quiere trabajar en esta sesión (afecta solo qué se muestra en la UI, el JWT ya trae todos los roles).
6. Para "cerrar sesión": simplemente borrar el token guardado — no hay endpoint de logout que llamar.
7. Para la pantalla de registro/invitación: revisar `roles` de la sesión actual — si incluye `Administrator`, mostrar selector de empresa; si no, omitirlo (ver tabla de reglas de negocio).

---

## Notas / posibles bugs a tener en cuenta

- `data.expiration` en login/register está hardcodeado a +60 min independientemente de la duración real del token (120 min en Development). No lo uses para lógica de expiración de sesión.
- `POST /api/Auth/create-role` está marcado `[AllowAnonymous]` en el código actual aunque el comentario XML dice que requiere autenticación — probablemente un bug pendiente de corregir en backend, no lo asumas protegido.
- CORS solo permite `localhost:4200` y `localhost:4201` por ahora — coordina con backend si tu app corre en otro puerto.
