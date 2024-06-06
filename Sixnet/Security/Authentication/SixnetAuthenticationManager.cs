using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.Set.Parameters;
using Sixnet.Cache.String.Parameters;
using Sixnet.Security.Permission;
using Sixnet.Session;

namespace Sixnet.Security.Authentication
{
    /// <summary>
    /// Authentication manager
    /// </summary>
    public class SixnetAuthenticationManager
    {
        /// <summary>
        /// Set authentication token
        /// </summary>
        /// <param name="configure"></param>
        public static async Task SetAuthenticationTokenAsync(Action<AuthenticationTokenSetting> configure)
        {
            var setting = new AuthenticationTokenSetting();
            configure?.Invoke(setting);

            var token = string.IsNullOrWhiteSpace(setting.Token) ? Guid.NewGuid().ToString() : setting.Token;
            var userKey = GetUserKey(setting);
            if (setting.IsUnlimited)
            {
                await SixnetCacher.Keys.DeleteAsync(new DeleteParameter()
                {
                    CacheObject = GetCacheObject(),
                    Keys = new List<CacheKey> { userKey }
                }).ConfigureAwait(false);
            }
            else
            {
                await SixnetCacher.String.SetAsync(new StringSetParameter()
                {
                    CacheObject = GetCacheObject(),
                    Items = new List<CacheEntry>()
                    {
                        new CacheEntry()
                        {
                            Key = userKey,
                            Value = token
                        }
                    }
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Set authentication token
        /// </summary>
        /// <param name="configure"></param>
        public static void SetAuthenticationToken(Action<AuthenticationTokenSetting> configure)
        {
            var setting = new AuthenticationTokenSetting();
            configure?.Invoke(setting);

            var token = string.IsNullOrWhiteSpace(setting.Token) ? Guid.NewGuid().ToString() : setting.Token;
            var userKey = GetUserKey(setting);
            if (setting.IsUnlimited)
            {
                SixnetCacher.Keys.Delete(new DeleteParameter()
                {
                    CacheObject = GetCacheObject(),
                    Keys = new List<CacheKey> { userKey }
                });
            }
            else
            {
                SixnetCacher.String.Set(new StringSetParameter()
                {
                    CacheObject = GetCacheObject(),
                    Items = new List<CacheEntry>()
                    {
                        new CacheEntry()
                        {
                            Key = userKey,
                            Value = token,
                            Expiration = new CacheExpiration()
                            {
                                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(setting.ExpireSeconds)
                            }
                        }
                    }
                }); ;
            }
        }

        /// <summary>
        /// Remove authentication token
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static async Task RemoveAuthenticationTokenAsync(Action<AuthenticationTokenSetting> configure)
        {
            var setting = new AuthenticationTokenSetting();
            configure?.Invoke(setting);

            var userKey = GetUserKey(setting);
            await SixnetCacher.Keys.DeleteAsync(new DeleteParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<CacheKey> { userKey }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove authentication token
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void RemoveAuthenticationToken(Action<AuthenticationTokenSetting> configure)
        {
            var setting = new AuthenticationTokenSetting();
            configure?.Invoke(setting);

            var userKey = GetUserKey(setting);
            SixnetCacher.Keys.Delete(new DeleteParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<CacheKey> { userKey }
            });
        }

        /// <summary>
        /// Validate authentication token
        /// </summary>
        /// <param name="configure"></param>
        public static async Task<bool> ValidateAuthenticationTokenAsync(Action<AuthenticationTokenSetting> configure)
        {
            var setting = new AuthenticationTokenSetting();
            configure?.Invoke(setting);

            if (setting.IsUnlimited)
            {
                return true;
            }
            var userKey = GetUserKey(setting);
            var token = (await SixnetCacher.String.GetAsync(new StringGetParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<CacheKey>() { userKey }
            }).ConfigureAwait(false)).Values?.FirstOrDefault()?.Value?.ToString();
            return token == setting.Token;
        }

        /// <summary>
        /// Validate authentication token
        /// </summary>
        /// <param name="configure"></param>
        public static bool ValidateAuthenticationToken(Action<AuthenticationTokenSetting> configure)
        {
            var setting = new AuthenticationTokenSetting();
            configure?.Invoke(setting);

            if (setting.IsUnlimited)
            {
                return true;
            }
            var userKey = GetUserKey(setting);
            var token = SixnetCacher.String.Get(new StringGetParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<CacheKey>() { userKey }
            }).Values?.FirstOrDefault()?.Value?.ToString();
            return token == setting.Token;
        }

        static string GetUserKey(AuthenticationTokenSetting setting)
        {
            var keyNameSplitChar = SixnetCacher.GetKeyNameSplitChar();
            var authorizationObject = PermissionObjectType.User;
            var appTag = setting.Score == AuthenticationScore.Application ? setting.AppTag : "";
            var userId = setting.UserId;
            return string.IsNullOrWhiteSpace(appTag)
                        ? $"{authorizationObject}{keyNameSplitChar}{userId}{keyNameSplitChar}Token"
                        : $"{appTag}{keyNameSplitChar}{authorizationObject}{keyNameSplitChar}{userId}{keyNameSplitChar}Token";
        }

        static CacheObject GetCacheObject()
        {
            return new CacheObject()
            {
                ObjectName = nameof(SixnetAuthenticationManager)
            };
        }
    }

    public enum AuthenticationScore
    {
        Single = 1,
        Application = 2,
        Unlimited = 3
    }
}
