using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace Authorization.WebApi.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DynamicAuthorizeAttribute : AuthorizeAttribute, IFilterFactory
    {
        public DynamicAuthorizeAttribute()
        {

        }

        public DynamicAuthorizeAttribute(string policy)
            : base(policy)
        {

        }

        public string PermissionName { get; set; }

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var dynamicAuthorizeDataProvider = serviceProvider.GetService<IDynamicRolePermissionProvider>();
            return new DynamicAuthorizeFilter(dynamicAuthorizeDataProvider, PermissionName);
        }
    }
}
