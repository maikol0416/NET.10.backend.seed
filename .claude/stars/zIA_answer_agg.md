1. ¿Cuál es el nombre del Bounded Context al que pertenece este agregado?
   DocumentManagement

2. ¿Cuál es el nombre del Agregado en Lenguaje Ubicuo del negocio?
   Document (singular)

3. Lista los campos propios del Agregado con su tipo y si son requeridos:
   Formato: NombreCampo : Tipo : Requerido(si/no)
   Ejemplo:
     - Name        : string  : si
     - Description : string  : no
     - Path        : string  : si

   ℹ️ No incluyas: Id, Status, CreatedAt, UpdateAt (se heredan de Entity automáticamente).

4. ¿El agregado contiene Value Objects?
   Para cada uno indica:
     a) Nombre del VO (en Lenguaje Ubicuo)
        Signature
     b) Relación: OwnsOne (uno solo) u OwnsMany (colección)
        OwnsMany
     c) Campos del VO con su tipo y si son requeridos
        - Name        : string  : si
        - Rol      : string  : si

5. Define las reglas de invarianza del agregado (validaciones en ExcecuteDomainInvariants):
   Formato: Campo | Condición | Mensaje de error en español

   Ejemplo:
     - Name      | IsNullOrWhiteSpace    | "El nombre es obligatorio."
     - Name      | Length > 150          | "El nombre no puede exceder 150 caracteres."
     - Path | IsNullOrWhiteSpace            | "La ruta es obligatoria."

6. ¿Cómo se llamará la tabla principal en la base de datos?
   Si el agregado tiene Value Objects, indica el nombre de tabla para cada uno.
   - Tabla : Document  aggregate : Document
   - Tabla : DocumentSignature VO: Signature
   
     
   