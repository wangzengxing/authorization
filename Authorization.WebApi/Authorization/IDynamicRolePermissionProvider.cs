using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.WebApi.Authorization
{
    public interface IDynamicRolePermissionProvider
    {
        /// <summary>
        /// Key:Permission,Value:Roles
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, IEnumerable<string>>> GetRolePermissionMapAsync();
    }
}
