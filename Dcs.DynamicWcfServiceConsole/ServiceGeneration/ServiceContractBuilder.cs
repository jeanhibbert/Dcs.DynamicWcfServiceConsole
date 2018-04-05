using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.ServiceModel;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// Contract builder
    /// </summary>
    public class ServiceContractBuilder : IServiceContractBuilder
    {
        private ModuleBuilder _moduleBuilder;
        private readonly string _soapNamespaceFormat;

        /// <summary>
        /// Initialise the builder
        /// </summary>
        /// <param name="moduleBuilder">The module to add the type to</param>
        /// <param name="soapNamespaceFormat">The format to use for building the namespace. Use the {0} and {1} for the TX id and TX version.</param>
        public ServiceContractBuilder(ModuleBuilder moduleBuilder, string soapNamespaceFormat)
        {
            this._moduleBuilder = moduleBuilder;
            this._soapNamespaceFormat = soapNamespaceFormat;
        }

        #region IServiceContractBuilder Members

        /// <summary>
        /// Build a contract for the specified TX
        /// </summary>
        /// <param name="className">The full class name to use, i.e.: Dcs.SomeNamespace.ITx1</param>
        /// <param name="tx">The TX to build a contract for</param>
        /// <returns>The interface type for the contract</returns>
        public Type BuildContract(string className, tTransaction tx)
        {
            // Get a type builder
            TypeBuilder typeBuilder = _moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.AnsiClass);

            // Add the ServiceContract attribute
            PropertyInfo namespaceProperty = typeof(ServiceContractAttribute).GetProperty("Namespace", BindingFlags.Instance | BindingFlags.Public);
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(ServiceContractAttribute).GetConstructor(Type.EmptyTypes), new object[0],
                new PropertyInfo[] { namespaceProperty }, new object[] { string.Format(this._soapNamespaceFormat, tx.TxId, tx.Version) }));
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(TxDetailsAttribute).GetConstructor(new Type[] { typeof(string) }), new object[] { tx.TxId }));

            // Build the execute method
            AddExecuteMethod(tx, typeBuilder);

            return typeBuilder.CreateType();
        }

        #endregion

        private static void AddExecuteMethod(tTransaction tx, TypeBuilder typeBuilder)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Execute", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract,
                tx.ReturnType, (from p in tx.Parameters
                                select p.ParameterType).ToArray());
            // Add OperationContract attribute
            PropertyInfo isOneWayProperty = typeof(OperationContractAttribute).GetProperty("IsOneWay", BindingFlags.Instance | BindingFlags.Public);
            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(OperationContractAttribute).GetConstructor(Type.EmptyTypes), new object[0],
                new PropertyInfo[] { isOneWayProperty }, new object[] { tx.IsOneWay }));
            int pos = 1;
            foreach (var parameter in tx.Parameters)
            {
                ParameterBuilder parameterBuilder = methodBuilder.DefineParameter(pos, ParameterAttributes.In, parameter.Name);
                pos++;
            }
        }
    }
}
