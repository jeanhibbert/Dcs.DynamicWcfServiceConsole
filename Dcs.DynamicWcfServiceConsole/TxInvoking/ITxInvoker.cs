using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// Strongly types TX invoker
    /// </summary>
    public interface ITxInvoker
    {
        /// <summary>
        /// Invoke the tx, with the given set of parameters.
        /// </summary>
        /// <param name="txId">The TX id to invoke</param>
        /// <param name="parameters">The parameter to the TX</param>
        /// <returns>The deserialised return value</returns>
        object Invoke(string txId, object[] parameters);
    }
}
