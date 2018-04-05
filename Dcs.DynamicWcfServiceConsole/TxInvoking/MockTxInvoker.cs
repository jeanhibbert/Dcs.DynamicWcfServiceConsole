using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// A mock tx invoker.
    /// </summary>
    public class MockTxInvoker : ITxInvoker
    {
        #region ITxInvoker Members

        /// <summary>
        /// Mock
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Invoke(string txId, object[] parameters)
        {
            Console.WriteLine("Invoking service {0}", txId);
            foreach (object p in parameters)
            {
                Console.WriteLine("   " + p.ToString());
            }
            switch (txId)
            {
                case "Tx1": return "Hallo world";
                case "Tx2": return 17;
                case "Tx3": return new DateTime(1974, 1, 6);
                case "Tx4": return null;
                case "Tx5": return null;
                default: return null;
            }
        }

        #endregion
    }
}
