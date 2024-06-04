using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.Set.Parameters;
using System.Linq;
using System.Threading.Tasks;

namespace Sixnet.Security.Authorization
{
    /// <summary>
    /// Authorization manager
    /// </summary>
    public static class SixnetAuthorizationManager
    {
        /// <summary>
        ///  Init authorization
        /// </summary>
        public static void InitAuthorization(Action<SixnetAuthorizationSetting> configure)
        {
            var authSetting = new SixnetAuthorizationSetting();
            configure?.Invoke(authSetting);

            // authorization
            var allAuths = authSetting.GetAuthorizationFunc?.Invoke();
            if (!allAuths.IsNullOrEmpty())
            {

                foreach (var authObj in allAuths)
                {
                    if (authObj.Value.IsNullOrEmpty())
                    {
                        continue;
                    }
                    foreach (var authObjItem in authObj.Value)
                    {
                        if (string.IsNullOrWhiteSpace(authObjItem?.AuthObjectId))
                        {
                            continue;
                        }
                        var objectAuthKey = GetObjectAuthorizationKey(authSetting.AppTag, authObj.Key, authObjItem.AuthObjectId);
                        DeleteObjectAuthorization(objectAuthKey);
                        if (!authObjItem.Permissions.IsNullOrEmpty())
                        {
                            SetObjectAuthorization(objectAuthKey, authObjItem.Permissions);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set object authorization
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="authorizationObject">Authorization object</param>
        /// <param name="objectId">Object id</param>
        /// <param name="permissions">Permissions</param>
        public static void SetObjectAuthorization(string appTag, AuthorizationObject authorizationObject, string objectId, List<string> permissions)
        {
            if (string.IsNullOrWhiteSpace(objectId))
            {
                return;
            }
            var cacheObject = GetCacheObject();
            var operationKey = GetObjectAuthorizationKey(appTag, authorizationObject, objectId);
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
        /// Validate
        /// </summary>
        /// <param name="appTag">App tag</param>
        /// <param name="operation">Operation</param>
        /// <param name="authObjects">Auth objects</param>
        /// <returns></returns>
        public static async Task<bool> Validate(string appTag, string operation, Dictionary<AuthorizationObject, List<string>> authObjects)
        {
            if (string.IsNullOrWhiteSpace(operation) || authObjects.IsNullOrEmpty())
            {
                return false;
            }
            var operationAuthKey = GetObjectAuthorizationKey(appTag, AuthorizationObject.Operation, operation);
            var authObjectAuthKeys = new List<CacheKey>() { operationAuthKey };
            foreach (var authObj in authObjects)
            {
                if (authObj.Value.IsNullOrEmpty())
                {
                    continue;
                }
                foreach (var authObjId in authObj.Value)
                {
                    authObjectAuthKeys.Add(GetObjectAuthorizationKey(appTag, authObj.Key, authObjId));
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

        static void DeleteObjectAuthorization(string objectAuthKey)
        {
            SixnetCacher.Keys.Delete(new DeleteParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<CacheKey> { objectAuthKey }
            });
        }

        static void SetObjectAuthorization(string objectAuthKey, List<string> permissions)
        {
            if (!string.IsNullOrWhiteSpace(objectAuthKey) && !permissions.IsNullOrEmpty())
            {
                SixnetCacher.Set.Add(new SetAddParameter()
                {
                    CacheObject = GetCacheObject(),
                    Key = objectAuthKey,
                    Members = permissions
                });
            }
        }

        static CacheObject GetCacheObject()
        {
            return new CacheObject { ObjectName = nameof(SixnetAuthorizationManager) };
        }

        static string GetObjectAuthorizationKey(string appTag, AuthorizationObject authorizationObject, string objectId)
        {
            var keyNameSplitChar = SixnetCacher.GetKeyNameSplitChar();
            return string.IsNullOrWhiteSpace(appTag)
                        ? $"{authorizationObject}{keyNameSplitChar}{objectId}{keyNameSplitChar}Auth"
                        : $"{appTag}{keyNameSplitChar}{authorizationObject}{keyNameSplitChar}{objectId}{keyNameSplitChar}Auth";
        }
    }

    /// <summary>
    /// Authorization object
    /// </summary>
    public enum AuthorizationObject
    {
        Operation = 1,
        Role = 2,
        User = 3
    }
}
