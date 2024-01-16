using System;
using System.Collections.Generic;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using Sixnet.Model;
using Sixnet.Session;

namespace Sixnet.Development.Data.Interceptor
{
    /// <summary>
    /// Intercept data manager
    /// </summary>
    internal static class InterceptDataManager
    {
        #region Fields

        /// <summary>
        /// Data interceptor
        /// </summary>
        static Action<InterceptDataContext> dataInterceptor;

        /// <summary>
        /// default interceptor field role
        /// </summary>
        static FieldRole defaultInterceptorFieldRole = FieldRole.CreatedDate | FieldRole.CreatedUserId
            | FieldRole.UpdatedDate | FieldRole.UpdatedUserId | FieldRole.Revision | FieldRole.Isolation;

        #endregion

        #region Intercept data

        /// <summary>
        /// Intercept datas
        /// </summary>
        /// <param name="dataCommand">Data command</param>
        /// <returns>Finally values</returns>
        internal static DataCommand InterceptData(DataCommand dataCommand)
        {
            var interceptContext = new InterceptDataContext()
            {
                DataCommand = dataCommand
            };

            // default interceptor
            if (defaultInterceptorFieldRole != FieldRole.None)
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
        public static void ConfigDataInterceptor(Action<InterceptDataContext> interceptor)
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
            defaultInterceptorFieldRole = FieldRole.None;
        }

        /// <summary>
        /// Remove default interceptor
        /// </summary>
        /// <param name="fieldRoles">Field roles</param>
        public static void RemoveDefaultDataInterceptor(params FieldRole[] fieldRoles)
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
        static void DefaultDataInterceptor(InterceptDataContext context)
        {
            var entityType = context.GetEntityType();
            var operationType = context.GetDataOperationType();
            var createDateField = EntityManager.GetRoleField(entityType, defaultInterceptorFieldRole & FieldRole.CreatedDate);
            var updateDateField = EntityManager.GetRoleField(entityType, defaultInterceptorFieldRole & FieldRole.UpdatedDate);
            var createUserField = EntityManager.GetRoleField(entityType, defaultInterceptorFieldRole & FieldRole.CreatedUserId);
            var updateUserField = EntityManager.GetRoleField(entityType, defaultInterceptorFieldRole & FieldRole.UpdatedUserId);
            var versionField = EntityManager.GetRoleField(entityType, defaultInterceptorFieldRole & FieldRole.Revision);
            var isolationField = EntityManager.GetRoleField(entityType, defaultInterceptorFieldRole & FieldRole.Isolation);
            switch (operationType)
            {
                case DataOperationType.Insert:
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
                    if (SessionContext.Current?.User != null)
                    {
                        if (AllowSetInterceptorValue(context, createUserField))
                        {
                            context.SetNewValue(createUserField.PropertyName, SessionContext.Current.User.Id.ConvertTo(createUserField.DataType));
                        }
                        if (AllowSetInterceptorValue(context, updateUserField))
                        {
                            context.SetNewValue(updateUserField.PropertyName, SessionContext.Current.User.Id.ConvertTo(updateUserField.DataType));
                        }
                    }
                    if (SessionContext.Current?.IsolationData != null && AllowSetInterceptorValue(context, isolationField))
                    {
                        context.SetNewValue(isolationField.PropertyName, SessionContext.Current.IsolationData.Id.ConvertTo(isolationField.DataType));
                    }
                    break;
                case DataOperationType.Update:
                    // update field
                    if (AllowSetInterceptorValue(context, updateDateField))
                    {
                        context.SetNewValue(updateDateField.PropertyName, updateDateField.DataType.GetNowDateTime());
                    }
                    if (SessionContext.Current?.User != null && AllowSetInterceptorValue(context, updateUserField))
                    {
                        context.SetNewValue(updateUserField.PropertyName, SessionContext.Current.User.Id.ConvertTo(updateUserField.DataType));
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
                                var newCalValue = PropertyField.Create(versionField.PropertyName, entityType);
                                newCalValue.FormatOption = FieldFormatOption.Create(FieldFormatterNames.ADD, 1);
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
        static bool AllowSetInterceptorValue(InterceptDataContext context, EntityField entityField)
        {
            return entityField != null && context.AllowUpdateNewValue(entityField.DataType, entityField.PropertyName);
        }

        #endregion
    }
}
