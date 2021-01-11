using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.WebApi.Authorization
{
    public class DynamicAuthorizeFilter : IAsyncActionFilter
    {
        private readonly IDynamicRolePermissionProvider _dynamicAuthorizeDataProvider;
        private readonly string _permissionName;

        public DynamicAuthorizeFilter(IDynamicRolePermissionProvider dynamicAuthorizeDataProvider, string permissionName)
        {
            _dynamicAuthorizeDataProvider = dynamicAuthorizeDataProvider;
            _permissionName = permissionName;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User == null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                //用户没有认证，应该是匿名访问,过滤器不生效
                await next();
                return;
            }

            if (_dynamicAuthorizeDataProvider == null)
            {
                throw new InvalidOperationException("没有实现IDynamicRolePermissionProvider接口并注入");
            }

            var rolePermissionMap = await _dynamicAuthorizeDataProvider.GetRolePermissionMapAsync();
            if (rolePermissionMap == null)
            {
                throw new InvalidOperationException("角色权限映射为空");
            }

            var permissionName = _permissionName
                ?? context.ActionDescriptor.AttributeRouteInfo.Template;
            if (permissionName == null)
            {
                throw new InvalidOperationException("没有设置权限名称");
            }

            if (rolePermissionMap.TryGetValue(permissionName, out var allowRoles))
            {
                foreach (var role in allowRoles)
                {
                    if (context.HttpContext.User.IsInRole(role))
                    {
                        await next();
                        return;
                    }
                }
            }

            context.Result = new ForbidResult();
        }
    }
}
