using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// Describes a TX. tTransaction table information.
    /// </summary>
    public class tTransaction
    {
        /// <summary>
        /// Initialise the TX description
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="version"></param>
        /// <param name="returnType"></param>
        /// <param name="isOneWay"></param>
        /// <param name="parameters"></param>
        /// <param name="bindings"></param>
        public tTransaction(string txId, int version, Type returnType, bool isOneWay, IEnumerable<TxParameter> parameters, IEnumerable<Binding> bindings)
        {
            this.TxId = txId;
            this.Version = version;
            this.ReturnType = returnType;
            this.IsOneWay = isOneWay;
            this.Parameters = parameters;
            this.Bindings = bindings;
        }

        /// <summary>
        /// The TX id. Must be a valid identifier.
        /// </summary>
        public string TxId { get; private set; }

        /// <summary>
        /// Version of the TX
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// The return type of the TX
        /// </summary>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// Whether the TX is a one-way (does not return anything)
        /// </summary>
        public bool IsOneWay { get; private set; }

        /// <summary>
        /// The parameters to the TX
        /// </summary>
        public IEnumerable<TxParameter> Parameters { get; private set; }

        /// <summary>
        /// The bindings this TX must be hosted on.
        /// </summary>
        public IEnumerable<Binding> Bindings { get; private set; }
    }
}
