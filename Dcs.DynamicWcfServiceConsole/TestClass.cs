using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicWcfServiceGeneration
{
    public interface ITestInterface {
        int CallSomeMethod(string name, int value);
    }

    public class TestClass : TxServiceBase, ITestInterface
    {
        #region ITestInterface Members

        public int CallSomeMethod(string name, int value)
        {
            return (int)base.Execute("txid", typeof(string), new object[] { name, value });
        }

        #endregion
    }
}
