//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Text;
//using EZNEW.Development.Domain.Aggregation;
//using EZNEW.Development.Entity;
//using EZNEW.Development.DataAccess;
//using EZNEW.DependencyInjection;

//namespace EZNEW.Reflection
//{
//    /// <summary>
//    /// Defines dynamic code manager
//    /// </summary>
//    internal class DynamicCodeManager
//    {
//        #region Fields

//        /// <summary>
//        /// Defines module builder
//        /// Key => model type namespace
//        /// </summary>
//        readonly static Dictionary<string, ModuleBuilder> ModuleBuilders = new Dictionary<string, ModuleBuilder>();

//        #endregion

//        #region Generate entity type

//        /// <summary>
//        /// Generate entity type
//        /// </summary>
//        /// <param name="modelType">Model type</param>
//        /// <returns></returns>
//        internal static Type GenerateEntityType(Type modelType)
//        {
//            var modelAttribute = (AggregationModelAttribute)modelType.GetCustomAttributes(typeof(AggregationModelAttribute), false).First();
//            var entityTypeName = $"{modelType.Name}Entity";

//            var moduleBuilder = GetModuleBuilder(modelType);
//            TypeBuilder typeBuilder = moduleBuilder.DefineType(entityTypeName, TypeAttributes.Public | TypeAttributes.Class);
//            Type baseEntityType = typeof(BaseEntity<>).MakeGenericType(typeBuilder);
//            typeBuilder.SetParent(baseEntityType);

//            //Entity attribute
//            var namespaceArray = modelType.Namespace.LSplit(".");
//            EntityAttribute entityAttribute = modelAttribute.EntityInfo ?? new EntityAttribute(modelType.Name, namespaceArray[namespaceArray.Length - 1], string.Empty);
//            Type stringType = typeof(string);
//            Type[] ctorParams = new Type[] { stringType, stringType, stringType };
//            ConstructorInfo classCtorInfo = typeof(EntityAttribute).GetConstructor(ctorParams);
//            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(classCtorInfo, new string[] { entityAttribute.ObjectName, entityAttribute.Group, entityAttribute.Description });
//            typeBuilder.SetCustomAttribute(customAttributeBuilder);

//            //Properties
//            var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
//            foreach (var property in properties)
//            {
//                var nonDataAttribute = property.GetCustomAttribute<NonDataAttribute>();
//                if (nonDataAttribute != null)
//                {
//                    continue;
//                }
//                AddProperty(typeBuilder, property.Name, property.PropertyType);
//            }

//            //Generate type
//            var entityType = typeBuilder.CreateTypeInfo();
//            if (entityType != null)
//            {
//                //Configure entity
//                EntityManager.ConfigureEntity(entityType);

//                //Data access
//                var dataAccessType = typeof(DefaultDataAccess<>).MakeGenericType(entityType);
//                var dataAccess = Activator.CreateInstance(dataAccessType);
//                ContainerManager.AddInternalService(dataAccessType, dataAccess);

//                //Repository
//                //var 
//            }
//            return entityType;
//        }

//        #endregion

//        #region Generate entity data access contract

//        internal static Type GenerateDataAccessInterface(Type modelType, Type entityType)
//        {
//            if (modelType == null || entityType == null)
//            {
//                return null;
//            }
//            var moduleBuilder = GetModuleBuilder(modelType);
//            string interfaceName = $"I{modelType.Name}DataAccess";
//            TypeBuilder interfaceBuilder = moduleBuilder.DefineType(interfaceName, TypeAttributes.Public | TypeAttributes.Interface);
//            Type baseInterfaceType = typeof(IDataAccess<>).MakeGenericType(interfaceBuilder);
//            interfaceBuilder.SetParent(baseInterfaceType);
//            return interfaceBuilder.CreateTypeInfo();
//        }

//        #endregion

//        #region Generate data access

//        internal static Type GenerateDataAccess(Type modelType, Type entityType, Type dataAccessInterface)
//        {
//            if (modelType == null || entityType == null)
//            {
//                return null;
//            }
//            var moduleBuilder = GetModuleBuilder(modelType);
//            string interfaceName = $"I{modelType.Name}DataAccess";
//            TypeBuilder interfaceBuilder = moduleBuilder.DefineType(interfaceName, TypeAttributes.Public | TypeAttributes.Interface);
//            Type baseInterfaceType = typeof(IDataAccess<>).MakeGenericType(interfaceBuilder);
//            interfaceBuilder.SetParent(baseInterfaceType);
//            return interfaceBuilder.CreateTypeInfo();
//        }

//        #endregion

//        #region Get module builder

//        static ModuleBuilder GetModuleBuilder(Type modelType)
//        {
//            if (!ModuleBuilders.TryGetValue(modelType.Namespace, out var moduleBuilder) || moduleBuilder == null)
//            {
//                var assemblyName = new AssemblyName(modelType.Namespace);
//                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
//                moduleBuilder = assemblyBuilder.DefineDynamicModule("EZNEW_DYNAMIC");
//                ModuleBuilders[modelType.Namespace] = moduleBuilder;
//            }
//            return moduleBuilder;
//        }

//        #endregion

//        #region Add property

//        /// <summary>
//        /// Add property
//        /// </summary>
//        /// <param name="typeBuilder">Type builder</param>
//        /// <param name="propertyName">Property name</param>
//        /// <param name="propertyType">Property type</param>
//        private static void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
//        {
//            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
//            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

//            var getMethod = typeBuilder.DefineMethod("get_" + propertyName,
//                MethodAttributes.Public |
//                MethodAttributes.SpecialName |
//                MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
//            var getMethodIL = getMethod.GetILGenerator();
//            getMethodIL.Emit(OpCodes.Ldarg_0);
//            getMethodIL.Emit(OpCodes.Ldfld, fieldBuilder);
//            getMethodIL.Emit(OpCodes.Ret);

//            var setMethod = typeBuilder.DefineMethod("set_" + propertyName,
//                  MethodAttributes.Public |
//                  MethodAttributes.SpecialName |
//                  MethodAttributes.HideBySig,
//                  null, new[] { propertyType });
//            var setMethodIL = setMethod.GetILGenerator();
//            Label modifyProperty = setMethodIL.DefineLabel();
//            Label exitSet = setMethodIL.DefineLabel();

//            setMethodIL.MarkLabel(modifyProperty);
//            setMethodIL.Emit(OpCodes.Ldarg_0);
//            setMethodIL.Emit(OpCodes.Ldarg_1);
//            setMethodIL.Emit(OpCodes.Stfld, fieldBuilder);
//            setMethodIL.Emit(OpCodes.Nop);
//            setMethodIL.MarkLabel(exitSet);
//            setMethodIL.Emit(OpCodes.Ret);

//            propertyBuilder.SetGetMethod(getMethod);
//            propertyBuilder.SetSetMethod(setMethod);
//        }

//        #endregion
//    }
//}
