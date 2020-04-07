using System;

namespace MvcTemplate.Objects
{
    public class RolePermission : BaseModel
    {
        public Int64 RoleId { get; set; }
        public virtual Role Role { get; set; }

        public Int64 PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
