using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Algorithm.Selection;
using Sixnet.Security.Permission;

namespace Sixnet.Security.Authorization
{
    /// <summary>
    /// Authorization options
    /// </summary>
    [Serializable]
    public class SixnetAuthorizationOptions
    {
        /// <summary>
        /// Gets or sets the Servers
        /// </summary>
        public List<string> Servers { get; set; }

        /// <summary>
        /// Gets or sets the server select mode
        /// </summary>
        public SelectionMatchPattern ServerSelectMode { get; set; } = SelectionMatchPattern.EquiprobableRandom;

        /// <summary>
        /// Gets or sets whether enable remote authorization verify
        /// </summary>
        public bool Remote { get; set; }

        /// <summary>
        /// Whether ignore authentication
        /// </summary>
        public bool IgnoreAuthentication { get; set; }

        /// <summary>
        /// Whether ignore default authorize
        /// </summary>
        public bool IgnoreDefaultAuthorize { get; set; }

        /// <summary>
        /// Admin authorize
        /// </summary>
        public bool AdminAuthorize { get; set; }

        /// <summary>
        /// Gets or sets the authorize delegate
        /// </summary>
        public Func<SixnetAuthorizationContext, SixnetAuthorizationResult> Authorize { get; set; } = DefaultPermissionAuthorize;

        /// <summary>
        /// Gets or sets the authorize delegate
        /// </summary>
        public Func<SixnetAuthorizationContext, Task<SixnetAuthorizationResult>> AuthorizeAsync { get; set; } = DefaultPermissionAuthorizeAsync;

        static async Task<SixnetAuthorizationResult> DefaultPermissionAuthorizeAsync(SixnetAuthorizationContext context)
        {
            var user = context.User;
            var operation = context.Operation;
            if (user == null)
            {
                return SixnetAuthorizationResult.ChallengeResult();
            }
            if (string.IsNullOrWhiteSpace(operation))
            {
                return SixnetAuthorizationResult.ForbidResult();
            }
            var permissionObjects = new Dictionary<PermissionObjectType, List<string>>();
            permissionObjects[PermissionObjectType.Operation] = new List<string>(1) { context.Operation };
            permissionObjects[PermissionObjectType.User] = new List<string>(1) { user.Id };
            if (!user.Roles.IsNullOrEmpty())
            {
                permissionObjects[PermissionObjectType.Role] = new List<string>(user.Roles);
            }
            var hasPermission = await SixnetPermissionManager.ValidateOperationAsync(context?.User?.AppTag, context.Operation, permissionObjects).ConfigureAwait(false);
            return hasPermission
                   ? SixnetAuthorizationResult.SuccessResult()
                   : SixnetAuthorizationResult.ForbidResult();
        }

        static SixnetAuthorizationResult DefaultPermissionAuthorize(SixnetAuthorizationContext context)
        {
            var user = context.User;
            var operation = context.Operation;
            if (user == null)
            {
                return SixnetAuthorizationResult.ChallengeResult();
            }
            if (string.IsNullOrWhiteSpace(operation))
            {
                return SixnetAuthorizationResult.ForbidResult();
            }
            var permissionObjects = new Dictionary<PermissionObjectType, List<string>>();
            permissionObjects[PermissionObjectType.Operation] = new List<string>(1) { context.Operation };
            permissionObjects[PermissionObjectType.User] = new List<string>(1) { user.Id };
            if (!user.Roles.IsNullOrEmpty())
            {
                permissionObjects[PermissionObjectType.Role] = new List<string>(user.Roles);
            }
            var hasPermission = SixnetPermissionManager.ValidateOperation(context?.User?.AppTag, context.Operation, permissionObjects);
            return hasPermission
                   ? SixnetAuthorizationResult.SuccessResult()
                   : SixnetAuthorizationResult.ForbidResult();
        }
    }
}
