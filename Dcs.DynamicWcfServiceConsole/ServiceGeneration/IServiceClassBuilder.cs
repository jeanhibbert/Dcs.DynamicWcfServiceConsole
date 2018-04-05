using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// Interface to WCF service class builders.
    /// </summary>
    public interface IServiceClassBuilder
    {
        /// <summary>
        /// Build a service class to host the TX contracts given.
        /// </summary>
        /// <param name="className">The full class name to use, i.e.: Dcs.SomeNamespace.SomeClassName</param>
        /// <param name="contracts">The contracts to host</param>
        /// <returns>The service type</returns>
        Type BuildService(string className, Type[] contracts);
    }
}
