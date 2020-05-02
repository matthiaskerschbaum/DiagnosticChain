using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Shared.Interfaces
{
    public abstract class WebSerializable
    {
        public string AsXML()
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

        public string AsJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
