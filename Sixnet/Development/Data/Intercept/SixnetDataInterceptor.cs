// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using Sixnet.Session;

namespace Sixnet.Development.Data.Intercept
{
    /// <summary>
    /// Data interceptor
    /// </summary>
    internal static class SixnetDataInterceptor
    {
        #region Fields

        /// <summary>
        /// Data interceptor
        /// </summary>
        static Action<SixnetInterceptDataContext> dataInterceptor;

        /// <summary>
        /// default interceptor field role
        /// </summary>
        static SixnetFieldRole defaultInterceptorFieldRole = SixnetFieldRole.CreateDate | SixnetFieldRole.CreateUserId | SixnetFieldRole.CreateUserName | SixnetFieldRole.CreateUserDisplayName
            | SixnetFieldRole.UpdateDate | SixnetFieldRole.UpdateUserId | SixnetFieldRole.UpdateUserName | SixnetFieldRole.UpdateUserDisplayName | SixnetFieldRole.Revision | SixnetFieldRole.Isolation;

        #endregion

        #region Intercept data

        /// <summary>
        /// Intercept datas
        /// </summary>
        /// <param name="dataCommand">Data command</param>
        /// <returns>Finally values</returns>
        internal static SixnetDataCommand InterceptData(SixnetDataCommand dataCommand)
        {
            var interceptContext = new SixnetInterceptDataContext()
            {
                DataCommand = dataCommand
            };

            // default interceptor
            if (defaultInterceptorFieldRole != SixnetFieldRole.None)
            {
                DefaultDataInterceptor(interceptContext);
            }

            // config interceptor
            dataInterceptor?.Invoke(interceptContext);

            return dataCommand;
        }

        #endregion

        #region Config interceptor

        /// <summary>
        /// Config data interceptor
        /// </summary>
        /// <param name="interceptor"></param>
        public static void ConfigDataInterceptor(Action<SixnetInterceptDataContext> interceptor)
        {
            if (interceptor != null)
            {
                dataInterceptor += interceptor;
            }
        }

        #endregion

        #region Default interceptor

        /// <summary>
        /// Clear default data interceptors
        /// </summary>
        public static void ClearDefaultDataInterceptor()
        {
            defaultInterceptorFieldRole = SixnetFieldRole.None;
        }

        /// <summary>
        /// Remove default interceptor
        /// </summary>
        /// <param name="fieldRoles">Field roles</param>
        public static void RemoveDefaultDataInterceptor(params SixnetFieldRole[] fieldRoles)
        {
            if (fieldRoles.IsNullOrEmpty())
            {
                return;
            }
            foreach (var fieldRole in fieldRoles)
            {
                if ((defaultInterceptorFieldRole & fieldRole) != 0)
                {
                    defaultInterceptorFieldRole &= ~fieldRole;
                }
            }
        }

        /// <summary>
        /// Default data interceptor
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="SixnetException"></exception>
        static void DefaultDataInterceptor(SixnetInterceptDataContext context)
        {
            var entityType = context.GetEntityType();
            var operationType = context.GetDataOperationType();
            var createDateField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.CreateDate);
            var updateDateField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.UpdateDate);
            var createUserIdField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.CreateUserId);
            var updateUserIdField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.UpdateUserId);
            var createUserNameField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.CreateUserName);
            var updateUserNameField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.UpdateUserName);
            var createUserDisplayNameField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.CreateUserDisplayName);
            var updateUserDisplayNameField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.UpdateUserDisplayName);
            var versionField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.Revision);
            var isolationField = SixnetEntityManager.GetField(entityType, defaultInterceptorFieldRole & SixnetFieldRole.Isolation);
            switch (operationType)
            {
                case SixnetDataOperationType.Insert:
                    if (AllowSetInterceptorValue(context, createDateField))
                    {
                        context.SetNewValue(createDateField.PropertyName, createDateField.DataType.GetNowDateTime());
                    }
                    if (AllowSetInterceptorValue(context, updateDateField))
                    {
                        dynamic updateDateValue = createDateField != null && updateDateField.DataType == createDateField.DataType
                            ? context.GetNewValue(createDateField.PropertyName)
                            : updateDateField.DataType.GetNowDateTime();
                        context.SetNewValue(updateDateField.PropertyName, updateDateValue);
                    }
                    if (AllowSetInterceptorValue(context, versionField))
                    {
                        if (versionField.DataType == typeof(DateTimeOffset))
                        {
                            context.SetNewValue(versionField.PropertyName, DateTimeOffset.Now);
                        }
                        else if (versionField.DataType == typeof(DateTime))
                        {
                            context.SetNewValue(versionField.PropertyName, DateTime.Now);
                        }
                        else
                        {
                            context.SetNewValue(versionField.PropertyName, 1.ConvertTo(versionField.DataType));
                        }
                    }
                    if (SixnetSessionContext.Current?.User != null)
                    {
                        if (AllowSetInterceptorValue(context, createUserIdField))
                        {
                            context.SetNewValue(createUserIdField.PropertyName, SixnetSessionContext.Current.User.Id.ConvertTo(createUserIdField.DataType));
                        }
                        if (AllowSetInterceptorValue(context, createUserNameField))
                        {
                            context.SetNewValue(createUserNameField.PropertyName, SixnetSessionContext.Current.User.Name ?? string.Empty);
                        }
                        if (AllowSetInterceptorValue(context, createUserDisplayNameField))
                        {
                            context.SetNewValue(createUserDisplayNameField.PropertyName, SixnetSessionContext.Current.User.DisplayName ?? string.Empty);
                        }
                        if (AllowSetInterceptorValue(context, updateUserIdField))
                        {
                            context.SetNewValue(updateUserIdField.PropertyName, SixnetSessionContext.Current.User.Id.ConvertTo(updateUserIdField.DataType));
                        }
                        if (AllowSetInterceptorValue(context, updateUserNameField))
                        {
                            context.SetNewValue(updateUserNameField.PropertyName, SixnetSessionContext.Current.User.Name ?? string.Empty);
                        }
                        if (AllowSetInterceptorValue(context, updateUserDisplayNameField))
                        {
                            context.SetNewValue(updateUserDisplayNameField.PropertyName, SixnetSessionContext.Current.User.DisplayName ?? string.Empty);
                        }
                    }
                    if (SixnetSessionContext.Current?.Isolation != null && AllowSetInterceptorValue(context, isolationField))
                    {
                        context.SetNewValue(isolationField.PropertyName, SixnetSessionContext.Current.Isolation.Id.ConvertTo(isolationField.DataType));
                    }
                    break;
                case SixnetDataOperationType.Update:
                    // update field
                    if (AllowSetInterceptorValue(context, updateDateField))
                    {
                        context.SetNewValue(updateDateField.PropertyName, updateDateField.DataType.GetNowDateTime());
                    }
                    if (SixnetSessionContext.Current?.User != null && AllowSetInterceptorValue(context, updateUserIdField))
                    {
                        context.SetNewValue(updateUserIdField.PropertyName, SixnetSessionContext.Current.User.Id.ConvertTo(updateUserIdField.DataType));
                    }
                    if (SixnetSessionContext.Current?.User != null && AllowSetInterceptorValue(context, updateUserNameField))
                    {
                        context.SetNewValue(updateUserNameField.PropertyName, SixnetSessionContext.Current.User.Name ?? string.Empty);
                    }
                    if (SixnetSessionContext.Current?.User != null && AllowSetInterceptorValue(context, updateUserDisplayNameField))
                    {
                        context.SetNewValue(updateUserDisplayNameField.PropertyName, SixnetSessionContext.Current.User.DisplayName ?? string.Empty);
                    }

                    // version field
                    if (versionField != null && !context.HasNewValue(versionField.PropertyName))
                    {
                        if (versionField.DataType == typeof(DateTimeOffset))
                        {
                            context.SetNewValue(versionField.PropertyName, DateTimeOffset.Now);
                        }
                        else if (versionField.DataType == typeof(DateTime))
                        {
                            context.SetNewValue(versionField.PropertyName, DateTime.Now);
                        }
                        else
                        {
                            if (context.HasOldValue(versionField.PropertyName))
                            {
                                var currentVersionValue = context.GetOldValue(versionField.PropertyName);
                                context.SetNewValue(versionField.PropertyName, currentVersionValue + 1);
                            }
                            else
                            {
                                var newCalValue = SixnetDataField.Create(versionField.PropertyName, entityType);
                                newCalValue.FormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.ADD, 1);
                                context.SetNewValue(versionField.PropertyName, newCalValue);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Whether allow set interceptor value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityField"></param>
        /// <returns></returns>
        static bool AllowSetInterceptorValue(SixnetInterceptDataContext context, SixnetDataField entityField)
        {
            return entityField != null && context.AllowUpdateNewValue(entityField.DataType, entityField.PropertyName);
        }

        #endregion
    }
}
