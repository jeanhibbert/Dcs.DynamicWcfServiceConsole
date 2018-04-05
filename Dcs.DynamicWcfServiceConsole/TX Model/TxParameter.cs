using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// Information required for a parameter
    /// </summary>
    public class TxParameter
    {
        /// <summary>
        /// Initialise the parameter information
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public TxParameter(string name, Type type)
        {
            this.Name = name;
            this.ParameterType = type;
        }

        /// <summary>
        /// The name of the parameters. Must be a valid identifier
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type of the parameter
        /// </summary>
        public Type ParameterType { get; private set; }
    }
}
