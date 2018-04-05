using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Reflection.Emit;
using System.Reflection;

namespace DynamicWcfServiceGeneration
{
    public class TxServiceHost : ServiceHost
    {
        private static string GetTxServiceContractName(string contractsNamespace, tTransaction tx)
        {
            return string.Format("{0}.I{1}", contractsNamespace, tx.TxId);
        }

        private static Type GenerateService(string serviceName, string contractsNamespace, tTransaction[] txList)
        {
            AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("test.dll"), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("TestModule", "test.dll");

            IServiceContractBuilder contractBuilder = new ServiceContractBuilder(moduleBuilder, "http://Dcs/tx/services/{0}/{1}");
            List<Type> contracts = new List<Type>();
            foreach (var tx in txList)
            {
                contracts.Add(contractBuilder.BuildContract(GetTxServiceContractName(contractsNamespace, tx), tx));
            }

            IServiceClassBuilder serviceBuilder = new ServiceClassBuilder(moduleBuilder);
            return serviceBuilder.BuildService(serviceName, contracts.ToArray());
        }

        public TxServiceHost(string serviceName, string contractsNamespace, tTransaction[] txList, params Uri[] baseAddresses)
            : base(GenerateService(serviceName, contractsNamespace, txList), baseAddresses)
        {
            foreach (Type serviceContract in this.Description.ServiceType.GetInterfaces())
            {
                TxDetailsAttribute attr = (TxDetailsAttribute)serviceContract.GetCustomAttributes(typeof(TxDetailsAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    // Contract is hosted
                    tTransaction tx = (from t in txList
                                        where t.TxId == attr.TxId
                                        select t).FirstOrDefault();
                    if (tx != null)
                    {
                        foreach (var binding in tx.Bindings)
                        {
                            this.AddServiceEndpoint(serviceContract, binding, string.Format("{0}/{1}", tx.TxId, tx.Version));
                        }
                    }
                }
            }
        }
    }
}
