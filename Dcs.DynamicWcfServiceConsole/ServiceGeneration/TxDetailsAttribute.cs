using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicWcfServiceGeneration
{
    /// <summary>
    /// Keep more information on the TX.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class TxDetailsAttribute : Attribute
    {
        private readonly string _txId;

        /// <summary>
        /// Initialise the attribute
        /// </summary>
        /// <param name="txId"></param>
        public TxDetailsAttribute(string txId)
        {
            this._txId = txId;
        }

        /// <summary>
        /// The TX id. Must be a valid identifier
        /// </summary>
        public string TxId { get { return _txId; } }
    }
}
