using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.Set.Parameters;
using System.Linq;
using System.Threading.Tasks;

namespace Sixnet.Security.Permission
{
    /// <summary>
    /// Permission manager
    /// </summary>
    public static class SixnetPermissionManager
    {
        /// <summary>
        /// Set object permission
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="permissionObjectType">Authorization object</param>
        /// <param name="objectValue">Object value</param>
        /// <param name="permissions">Permissions</param>
        public static void SetObjectPermission(string appTag, PermissionObjectType permissionObjectType, string objectValue, List<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(objectValue))
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var operationKey = GetObjectPermissionKey(appTag, permissionObjectType, objectValue);
            if (permissions.IsNullOrEmpty())
            {
                SixnetCacher.Keys.Delete(new DeleteParameter()
                {
                    CacheObject = cacheObject,
                    Keys = new List<CacheKey> { operationKey }
                });
            }
            else
            {
                var currentPermissions = SixnetCacher.Set.Members(new SetMembersParameter()
                {
                    CacheObject = cacheObject,
                    Key = operationKey,
                })?.Members ?? new List<string>();
                SixnetCacher.Set.Add(new SetAddParameter()
                {
                    CacheObject = cacheObject,
                    Key = operationKey,
                    Members = permissions
                });
                var removeMembers = currentPermissions.Except(permissions).ToList();
                if (!removeMembers.IsNullOrEmpty())
                {
                    SixnetCacher.Set.Remove(new SetRemoveParameter()
                    {
                        CacheObject = cacheObject,
                        Key = operationKey,
                        Members = removeMembers
                    });
                }
            }
        }

        /// <summary>
        /// Set object permission
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="permissionObjectType">Authorization object</param>
        /// <param name="objectValue">Object value</param>
        /// <param name="permissions">Permissions</param>
        public static async Task SetObjectPermissionAsync(string appTag, PermissionObjectType permissionObjectType, string objectValue, List<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(objectValue))
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var operationKey = GetObjectPermissionKey(appTag, permissionObjectType, objectValue);
            if (permissions.IsNullOrEmpty())
            {
                await SixnetCacher.Keys.DeleteAsync(new DeleteParameter()
                {
                    CacheObject = cacheObject,
                    Keys = new List<CacheKey> { operationKey }
                }).ConfigureAwait(false);
            }
            else
            {
                var currentPermissions = (await SixnetCacher.Set.MembersAsync(new SetMembersParameter()
                {
                    CacheObject = cacheObject,
                    Key = operationKey,
                }).ConfigureAwait(false))?.Members ?? new List<string>();
                await SixnetCacher.Set.AddAsync(new SetAddParameter()
                {
                    CacheObject = cacheObject,
                    Key = operationKey,
                    Members = permissions
                }).ConfigureAwait(false);
                var removeMembers = currentPermissions.Except(permissions).ToList();
                if (!removeMembers.IsNullOrEmpty())
                {
                    await SixnetCacher.Set.RemoveAsync(new SetRemoveParameter()
                    {
                        CacheObject = cacheObject,
                        Key = operationKey,
                        Members = removeMembers
                    }).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Add object permission
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="permissionObjectType">Authorization object</param>
        /// <param name="objectValue">Object value</param>
        /// <param name="permissions">Permissions</param>
        public static async Task AddObjectPermissionAsync(string appTag, PermissionObjectType permissionObjectType, string objectValue, List<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(objectValue) || permissions.IsNullOrEmpty())
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var operationKey = GetObjectPermissionKey(appTag, permissionObjectType, objectValue);
            await SixnetCacher.Set.AddAsync(new SetAddParameter()
            {
                CacheObject = cacheObject,
                Key = operationKey,
                Members = permissions
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Add object permission
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="permissionObjectType">Authorization object</param>
        /// <param name="objectValue">Object value</param>
        /// <param name="permissions">Permissions</param>
        public static void AddObjectPermission(string appTag, PermissionObjectType permissionObjectType, string objectValue, List<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(objectValue) || permissions.IsNullOrEmpty())
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var operationKey = GetObjectPermissionKey(appTag, permissionObjectType, objectValue);
            SixnetCacher.Set.Add(new SetAddParameter()
            {
                CacheObject = cacheObject,
                Key = operationKey,
                Members = permissions
            });
        }

        /// <summary>
        /// Validate operation
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="operation">Operation</param>
        /// <param name="objects">Auth objects</param>
        /// <returns></returns>
        public static async Task<bool> ValidateOperationAsync(string appTag, string operation, Dictionary<PermissionObjectType, List<string>> objects)
        {
            if (string.IsNullOrWhiteSpace(operation) || objects.IsNullOrEmpty())
            {
                return false;
            }
            var operationAuthKey = GetObjectPermissionKey(appTag, PermissionObjectType.Operation, operation);
            var authObjectAuthKeys = new List<CacheKey>() { operationAuthKey };
            foreach (var authObj in objects)
            {
                if (authObj.Value.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var authObjId in authObj.Value)
                {
                    authObjectAuthKeys.Add(GetObjectPermissionKey(appTag, authObj.Key, authObjId));
                }
            }
            var combineResult = await SixnetCacher.Set.CombineAsync(new SetCombineParameter()
            {
                CacheObject = GetCacheObject(),
                CombineOperation = CombineOperation.Intersect,
                Keys = authObjectAuthKeys
            }).ConfigureAwait(false);
            return !(combineResult?.CombineValues?.IsNullOrEmpty() ?? true);
        }

        /// <summary>
        /// Validate operation
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="operation">Operation</param>
        /// <param name="objects">Auth objects</param>
        /// <returns></returns>
        public static bool ValidateOperation(string appTag, string operation, Dictionary<PermissionObjectType, List<string>> objects)
        {
            if (string.IsNullOrWhiteSpace(operation) || objects.IsNullOrEmpty())
            {
                return false;
            }
            var operationAuthKey = GetObjectPermissionKey(appTag, PermissionObjectType.Operation, operation);
            var authObjectAuthKeys = new List<CacheKey>() { operationAuthKey };
            foreach (var authObj in objects)
            {
                if (authObj.Value.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var authObjId in authObj.Value)
                {
                    authObjectAuthKeys.Add(GetObjectPermissionKey(appTag, authObj.Key, authObjId));
                }
            }
            var combineResult = SixnetCacher.Set.Combine(new SetCombineParameter()
            {
                CacheObject = GetCacheObject(),
                CombineOperation = CombineOperation.Intersect,
                Keys = authObjectAuthKeys
            });
            return !(combineResult?.CombineValues?.IsNullOrEmpty() ?? true);
        }

        /// <summary>
        /// Delete object permission
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="permissionObjectType">Permission object type</param>
        /// <param name="objectValue">Object value</param>
        /// <returns></returns>
        public static async Task DeleteObjectPermissionAsync(string appTag, PermissionObjectType permissionObjectType, string objectValue)
        {
            if (string.IsNullOrWhiteSpace(objectValue))
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var objectKey = GetObjectPermissionKey(appTag, permissionObjectType, objectValue);
            await SixnetCacher.Keys.DeleteAsync(new DeleteParameter()
            {
                CacheObject = cacheObject,
                Keys = new List<CacheKey> { objectKey }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete object permission
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="permissionObjectType">Permission object type</param>
        /// <param name="objectValue">Object value</param>
        /// <returns></returns>
        public static void DeleteObjectPermission(string appTag, PermissionObjectType permissionObjectType, string objectValue)
        {
            if (string.IsNullOrWhiteSpace(objectValue))
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var objectKey = GetObjectPermissionKey(appTag, permissionObjectType, objectValue);
            SixnetCacher.Keys.Delete(new DeleteParameter()
            {
                CacheObject = cacheObject,
                Keys = new List<CacheKey> { objectKey }
            });
        }

        /// <summary>
        /// Delete object permission
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="permissionObjectType">Permission object type</param>
        /// <param name="objectValue">Object value</param>
        /// <returns></returns>
        public static async Task DeleteObjectPermissionAsync(string appTag, PermissionObjectType permissionObjectType, string objectValue, List<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(objectValue) || permissions.IsNullOrEmpty())
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var objectKey = GetObjectPermissionKey(appTag, permissionObjectType, objectValue);
            await SixnetCacher.Set.RemoveAsync(new SetRemoveParameter()
            {
                CacheObject = cacheObject,
                Key = objectKey,
                Members = permissions
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete object permission
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="permissionObjectType">Permission object type</param>
        /// <param name="objectValue">Object value</param>
        /// <returns></returns>
        public static void DeleteObjectPermission(string appTag, PermissionObjectType permissionObjectType, string objectValue, List<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(objectValue) || permissions.IsNullOrEmpty())
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var objectKey = GetObjectPermissionKey(appTag, permissionObjectType, objectValue);
            SixnetCacher.Set.Remove(new SetRemoveParameter()
            {
                CacheObject = cacheObject,
                Key = objectKey,
                Members = permissions
            });
        }

        /// <summary>
        /// Get cache object
        /// </summary>
        /// <returns></returns>
        static CacheObject GetCacheObject()
        {
            return new CacheObject { ObjectName = nameof(SixnetPermissionManager) };
        }

        /// <summary>
        /// Get object permission key
        /// </summary>
        /// <param name="appTag"></param>
        /// <param name="permissionObjectType"></param>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        static string GetObjectPermissionKey(string appTag, PermissionObjectType permissionObjectType, string objectValue)
        {
            var keyNameSplitChar = SixnetCacher.GetKeyNameSplitChar();
            return string.IsNullOrWhiteSpace(appTag)
                        ? $"{permissionObjectType}{keyNameSplitChar}{objectValue}{keyNameSplitChar}Auth"
                        : $"{appTag}{keyNameSplitChar}{permissionObjectType}{keyNameSplitChar}{objectValue}{keyNameSplitChar}Auth";
        }
    }

    /// <summary>
    /// Permission object type
    /// </summary>
    public enum PermissionObjectType
    {
        Operation = 1,
        Role = 2,
        User = 3
    }
}
