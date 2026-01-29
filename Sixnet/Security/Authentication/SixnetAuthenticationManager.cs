// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.App;
using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.String.Parameters;
using Sixnet.DependencyInjection;
using Sixnet.Security.Permission;

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
            await SixnetCacher.String.SetAsync(new SixnetStringSetParameter()
            {
                CacheObject = GetCacheObject(),
                Items =
                [
                    new SixnetCacheEntry()
                    {
                        Key = userKey,
                        Value = token
                    }
                ]
            }).ConfigureAwait(false);
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
            SixnetCacher.String.Set(new SixnetStringSetParameter()
            {
                CacheObject = GetCacheObject(),
                Items =
                [
                    new SixnetCacheEntry()
                    {
                        Key = userKey,
                        Value = token,
                        Expiration = new SixnetCacheExpiration()
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(setting.ExpireSeconds)
                        }
                    }
                ]
            });
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
            await SixnetCacher.Keys.DeleteAsync(new SixnetDeleteParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = [userKey]
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
            SixnetCacher.Keys.Delete(new SixnetDeleteParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = [userKey]
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
            var userKey = GetUserKey(setting);
            var token = (await SixnetCacher.String.GetAsync(new SixnetStringGetParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<SixnetCacheKey>() { userKey }
            }).ConfigureAwait(false)).Values?.FirstOrDefault()?.Value?.ToString();

            if (string.IsNullOrWhiteSpace(token))
            {
                if (setting.IgnoreServerValidation)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (setting.IsUnlimited)
            {
                return true;
            }
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
            var userKey = GetUserKey(setting);
            var token = SixnetCacher.String.Get(new SixnetStringGetParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<SixnetCacheKey>() { userKey }
            }).Values?.FirstOrDefault()?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(token))
            {
                if (setting.IgnoreServerValidation)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (setting.IsUnlimited)
            {
                return true;
            }
            return token == setting.Token;
        }

        /// <summary>
        /// Get authentication token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetAuthenticationToken(string userId, Action<AuthenticationTokenSetting> configure = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return string.Empty;
            }
            var authOptions = SixnetContainer.GetOptions<SixnetAuthenticationOptions>() ?? new SixnetAuthenticationOptions();
            var tokenSetting = new AuthenticationTokenSetting()
            {
                AppTag = SixnetApplication.Current.GetDefaultAppTag(),
                Score = authOptions.Score,
                UserId = userId,
            };
            configure?.Invoke(tokenSetting);
            var userKey = GetUserKey(tokenSetting);
            return SixnetCacher.String.Get(new SixnetStringGetParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<SixnetCacheKey>() { userKey }
            }).Values?.FirstOrDefault()?.Value?.ToString();
        }

        /// <summary>
        /// Get authentication token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<string> GetAuthenticationTokenAsync(string userId, Action<AuthenticationTokenSetting> configure = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return string.Empty;
            }
            var authOptions = SixnetContainer.GetOptions<SixnetAuthenticationOptions>() ?? new SixnetAuthenticationOptions();
            var tokenSetting = new AuthenticationTokenSetting()
            {
                AppTag = SixnetApplication.Current.GetDefaultAppTag(),
                Score = authOptions.Score,
                UserId = userId,
            };
            configure?.Invoke(tokenSetting);
            var userKey = GetUserKey(tokenSetting);
            return (await SixnetCacher.String.GetAsync(new SixnetStringGetParameter()
            {
                CacheObject = GetCacheObject(),
                Keys = new List<SixnetCacheKey>() { userKey }
            })).Values?.FirstOrDefault()?.Value?.ToString();
        }

        /// <summary>
        /// Get user key
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        static string GetUserKey(AuthenticationTokenSetting setting)
        {
            var keyNameSplitChar = SixnetCacher.GetKeyNameSplitChar();
            var authorizationObject = PermissionObjectType.User;
            var appTag = setting.Score == AuthenticationScore.Application ? setting.AppTag : "";
            var userId = setting.UserId;
            var userKey = $"{authorizationObject}{keyNameSplitChar}{userId}{keyNameSplitChar}Token";
            if (!string.IsNullOrWhiteSpace(appTag))
            {
                userKey = $"{appTag}{keyNameSplitChar}{userKey}";
            }
            return userKey;
        }

        /// <summary>
        /// Get cache object
        /// </summary>
        /// <returns></returns>
        static SixnetCacheObject GetCacheObject()
        {
            return new SixnetCacheObject()
            {
                ObjectName = nameof(SixnetAuthenticationManager)
            };
        }
    }

    /// <summary>
    /// Sixnet authentication score
    /// </summary>
    public enum AuthenticationScore
    {
        Single = 1,
        Application = 2,
        Unlimited = 3
    }
}
