namespace Domain.DomainShared;

/// <summary>
/// Catálogo fijo de módulos del sistema. Se usa como Lenguaje Ubicuo para
/// los permisos asignados a un rol — evita nombres de módulo libres/inconsistentes.
/// </summary>
public enum ModuleEnum
{
    PhysicalStructure,
    Owner,
    Document,
    Guest,
    Users,
    Roles
}
