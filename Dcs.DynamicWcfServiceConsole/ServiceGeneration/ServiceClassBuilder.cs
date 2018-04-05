using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// WCF service builder.
    /// </summary>
    public class ServiceClassBuilder : IServiceClassBuilder
    {
        private ModuleBuilder _moduleBuilder;

        /// <summary>
        /// Initialise the builder
        /// </summary>
        /// <param name="moduleBuilder">The module to add the type to</param>
        public ServiceClassBuilder(ModuleBuilder moduleBuilder)
        {
            this._moduleBuilder = moduleBuilder;
        }

        #region IServiceClassBuilder Members

        /// <summary>
        /// Build a service class to host the TX contracts given.
        /// </summary>
        /// <param name="className">The full class name to use, i.e.: Dcs.SomeNamespace.SomeClassName</param>
        /// <param name="contracts">The contracts to host</param>
        /// <returns>The service type</returns>
        public Type BuildService(string className, Type[] contracts)
        {
            // Get a type builder
            TypeBuilder typeBuilder = _moduleBuilder.DefineType(className
                , TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit
                , typeof(TxServiceBase), contracts);

            // Create a default constructor
            CreateDefaultConstructor(typeBuilder);
            foreach (Type contract in contracts)
            {
                CreateContractImplementation(typeBuilder, contract, GetTxId(contract));
            }

            return typeBuilder.CreateType();
        }

        #endregion

        private string GetTxId(Type contract)
        {
            foreach (TxDetailsAttribute attr in contract.GetCustomAttributes(typeof(TxDetailsAttribute), true))
            {
                return attr.TxId;
            }
            return contract.Name;
        }

        private static void Emit_Ldc_I4_x(ILGenerator il, int x)
        {
            switch (x)
            {
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default: il.Emit(OpCodes.Ldc_I4, x); break;
            }
        }

        private static void Emit_Ldarg_x(ILGenerator il, int x)
        {
            switch (x)
            {
                case 0: il.Emit(OpCodes.Ldarg_0); break;
                case 1: il.Emit(OpCodes.Ldarg_1); break;
                case 2: il.Emit(OpCodes.Ldarg_2); break;
                case 3: il.Emit(OpCodes.Ldarg_3); break;
                default: il.Emit(OpCodes.Ldarg, x); break;
            }
        }

        private void CreateContractImplementation(TypeBuilder typeBuilder, Type contract, string txId)
        {
            typeBuilder.AddInterfaceImplementation(contract);
            MethodInfo baseMethod = typeof(TxServiceBase).GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo minfo in contract.GetMethods())
            {
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(contract.FullName + "." + minfo.Name
                    , MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot
                    , CallingConventions.Standard | CallingConventions.HasThis
                    , minfo.ReturnType, (from p in minfo.GetParameters()
                                         select p.ParameterType).ToArray());

                ParameterInfo[] parameters = minfo.GetParameters();
                int paramCount = parameters.Length;

                ILGenerator il = methodBuilder.GetILGenerator();
                if (paramCount != 0)
                {
                    if ((minfo.ReturnType != null) && (minfo.ReturnType != typeof(void)))
                    {
                        il.DeclareLocal(minfo.ReturnType);
                    }
                    else
                    {
                        il.DeclareLocal(typeof(object));
                    }
                    il.DeclareLocal(typeof(object[]));
                }
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldarg_0);
                // Push txid onto stack (push txid)
                il.Emit(OpCodes.Ldstr, txId);
                // Get and push return type onto stack (push typeof(minfo.ReturnType))
                il.Emit(OpCodes.Ldtoken, minfo.ReturnType);
                il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(RuntimeTypeHandle) }, null));
                // Build and push object array onto stack (push new object[] { arg1, arg2, arg3 ...})
                Emit_Ldc_I4_x(il, paramCount);
                il.Emit(OpCodes.Newarr, typeof(object));
                if (paramCount != 0)
                {
                    il.Emit(OpCodes.Stloc_1);
                    il.Emit(OpCodes.Ldloc_1);
                }
                for (int i = 0; i < paramCount; i++)
                {
                    methodBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
                    Emit_Ldc_I4_x(il, i);
                    Emit_Ldarg_x(il, i + 1);
                    if (parameters[i].ParameterType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, parameters[i].ParameterType);
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                    il.Emit(OpCodes.Ldloc_1);
                }
                // Call base class method (Execute(txid, typeof(minfo.ReturnType), new object[] {arg1, arg2, ...}))
                il.Emit(OpCodes.Call, baseMethod);
                // Get return value
                if ((minfo.ReturnType == null) || (minfo.ReturnType == typeof(void)))
                {
                    il.Emit(OpCodes.Pop);
                }
                else if (minfo.ReturnType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, minfo.ReturnType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, minfo.ReturnType);
                }
                // Return
                il.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(methodBuilder, minfo);
            }
        }

        private void CreateDefaultConstructor(TypeBuilder typeBuilder)
        {
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard | CallingConventions.HasThis, new Type[] { });
            ILGenerator il = constructorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(TxServiceBase).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null));
            il.Emit(OpCodes.Ret);
        }
    }
}
