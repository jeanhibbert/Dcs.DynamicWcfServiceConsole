using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// Base class for all TX service classes
    /// </summary>
    public abstract class TxServiceBase
    {
        private ITxInvoker _txInvoker = new MockTxInvoker();

        /// <summary>
        /// Initialise
        /// </summary>
        protected TxServiceBase()
        {
        }

        /// <summary>
        /// The invoker to use when invoking the TX
        /// </summary>
        public ITxInvoker TxInvoker { get { return _txInvoker; } set { _txInvoker = value; } }

        /// <summary>
        /// Invoke the specified tx with the specified parameters.
        /// </summary>
        /// <param name="txId">The TX to invoke</param>
        /// <param name="returnType">The expected return type.</param>
        /// <param name="parameters">The parameters to the TX</param>
        /// <returns>The returned value, or null for void returns.</returns>
        protected object Execute(string txId, Type returnType, object[] parameters)
        {
            object r = _txInvoker.Invoke(txId, parameters);
            if ((returnType == null) || (returnType == typeof(void))) return null;
            if (r == null)
            {
                if (returnType.IsValueType) throw new InvalidCastException("The TX returned 'null' and expected a value type (" + returnType.FullName + ").");
            }
            if (returnType.IsAssignableFrom(r.GetType())) return r;
            TypeConverter converter = TypeDescriptor.GetConverter(returnType);
            if (converter.CanConvertFrom(r.GetType())) return converter.ConvertFrom(r);
            throw new InvalidCastException("The TX returned a value (" + r.GetType().FullName + ") that cannot be converted to the expected type (" + returnType.FullName + ").");
        }
    }
}
