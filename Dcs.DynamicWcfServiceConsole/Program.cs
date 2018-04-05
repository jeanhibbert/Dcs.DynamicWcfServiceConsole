using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace DynamicWcfServiceGeneration
{
    [DataContract]
    public class Person
    {
        [DataMember]
        public string Name;

        [DataMember]
        public int Age;

    }


    class Program
    {
        static void Main(string[] args)
        {
            tTransaction[] txList = new tTransaction[]
            {
                //new tTransaction("Tx1",1,typeof(string),false,
                //    new TxParameter[]
                //    {
                //        new TxParameter("name",typeof(string)),
                //        new TxParameter("dateOfBirth",typeof(DateTime)),
                //    },
                //    new Binding[]
                //    {
                //        new NetTcpBinding(),
                //        new BasicHttpBinding(),
                //    }),
                //new tTransaction("Tx2",1,typeof(int),false,
                //    new TxParameter[]
                //    {
                //        new TxParameter("num",typeof(int))
                //    },
                //    new Binding[]
                //    {
                //        new NetTcpBinding(),
                //        new NetNamedPipeBinding(),
                //    }),
                //new tTransaction("Tx3",2,typeof(DateTime),false,
                //    new TxParameter[]
                //    {
                //        new TxParameter("firstParameter",typeof(int))
                //    },
                //    new Binding[]
                //    {
                //        new NetTcpBinding(),
                //    }),
                new tTransaction("Tx4",1,null,true,
                    new TxParameter[]
                    {
                        new TxParameter("someVAlue",typeof(string))
                    },
                    new Binding[]
                    {
                        new BasicHttpBinding(),
                    }),
                //new tTransaction("Tx5",1,null,false,
                //    new TxParameter[]
                //    {
                //    },
                //    new Binding[]
                //    {
                //        new BasicHttpBinding(),
                //    }),
                //    new tTransaction("Tx6",1,null,false,
                //    new TxParameter[]
                //    {
                //        new TxParameter("Person",typeof(Person))
                //    },
                //    new Binding[]
                //    {
                //        new BasicHttpBinding(),
                //    }),
            };

            using (TxServiceHost host = new TxServiceHost("Dcs.TxHosting.TxHost", "Dcs.TxHosting", txList))
            {
                host.Open();
                Console.WriteLine("Ready");
                Console.ReadLine();
            }
        }
    }
}
