using System.Reflection;
using AutoMapper;
using Domain.DomainShared;
using Domain.Ports.Identity;
using Domain.Ports.Repository.Base;

namespace Application.Base;

public abstract partial class ApplicationService<ENT, DTO> : ApplicationServiceMapper<ENT, DTO>, IApplicationService<ENT, DTO>
        where ENT : class, new()
        where DTO : class, new()
    {
        private static readonly PropertyInfo? CompanyIdProperty =
            typeof(DTO).GetProperty("CompanyId", typeof(Guid?));

        protected IBaseRepository<ENT> RepositoryBase { get; set; }
        protected ICurrentUserService CurrentUser { get; set; }

        public ApplicationService(IBaseRepository<ENT> repositoryBase, ICurrentUserService currentUser)
        {
            RepositoryBase = repositoryBase;
            CurrentUser = currentUser;
        }

        public virtual async Task<DTO> CreateAsync(DTO dto)
        {
            StampCompany(dto);
            var entity = MapToENT(dto);
            return MapToDTO(await RepositoryBase.CreateAsync(entity));
        }

        public virtual async Task<bool> CreateListAsync(List<DTO> dtos)
        {
            dtos.ForEach(StampCompany);
            var entities = MapLstToENT(dtos);
            await RepositoryBase.CreateListAsync(entities);
            return await Task.FromResult(true);
        }

        public virtual async Task<bool> DeleteEntity(Guid id)
        {
            return await RepositoryBase.DeleteEntity(id);
        }

        public virtual async Task<DTO> UpdateAsync(DTO dto)
        {
            StampCompany(dto);
            return MapToDTO(await RepositoryBase.UpdateAsync(MapToENT(dto)));
        }

        /// <summary>
        /// Multi-tenant: si el DTO expone una propiedad CompanyId (Guid?), la sobrescribe
        /// siempre con la empresa del usuario autenticado — nunca confía en lo que mande
        /// el cliente. Para DTOs sin esa propiedad (agregados que no son tenant-scoped) no
        /// hace nada.
        /// </summary>
        private void StampCompany(DTO dto)
        {
            if (CompanyIdProperty is null)
            {
                return;
            }

            if (CurrentUser.CompanyId is null)
            {
                throw new DomainException("Tu usuario no pertenece a ninguna empresa; no puedes crear ni modificar este recurso.");
            }

            CompanyIdProperty.SetValue(dto, CurrentUser.CompanyId);
        }
    }
