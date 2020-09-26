using MvcTemplate.Components;
using MvcTemplate.Objects;
using System;
using System.Linq;

namespace MvcTemplate.Services
{
    public interface IRoleService : IService
    {
        IQueryable<RoleView> GetViews();
        RoleView? GetView(Int64 id);

        void Seed(MvcTree permissions);
        void Create(RoleView view);
        void Edit(RoleView view);
        void Delete(Int64 id);
    }
}
