using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// Interface to TX service contract builders
    /// </summary>
    public interface IServiceContractBuilder
    {
        /// <summary>
        /// Build a contract for the specified TX
        /// </summary>
        /// <param name="className">The full class name to use, i.e.: Dcs.SomeNamespace.ITx1</param>
        /// <param name="tx">The TX to build a contract for</param>
        /// <returns>The interface type for the contract</returns>
        Type BuildContract(string className, tTransaction tx);
    }
}
