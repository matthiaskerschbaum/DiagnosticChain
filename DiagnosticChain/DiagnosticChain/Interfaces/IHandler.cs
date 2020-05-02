using Blockchain;
using Blockchain.Utilities;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Handler.Interfaces
{
    public abstract class IHandler
    {
        protected Action onShutDown;
        public string username;
        public Chain chain;

        public abstract void StartUp(Action onShutDown, bool registerNew = false, string username = "");
        public abstract void ShutDown();

        public ChainStatistics GetChainStatisics()
        {
            return new ChainStatistics(chain);
        }

        public string HandlerStateAsXML()
        {
            XmlSerializer xsSubmit = new XmlSerializer(this.GetType());
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, this);
                    xml = sww.ToString();
                }
            }

            return xml;
        }
    }
}
