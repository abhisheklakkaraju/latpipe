using System;

namespace MvcTemplate.Components.Security
{
    public interface IAuthorization
    {
        Boolean IsGrantedFor(Int64? accountId, String permission);

        void Refresh();
    }
}
