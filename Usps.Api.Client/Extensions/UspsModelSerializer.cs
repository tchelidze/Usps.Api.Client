using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Usps.Api.Client.Extensions
{
    public static class UspsModelSerializer
    {
        public static XmlSerializerNamespaces EmptyNamespace = new XmlSerializerNamespaces();

        static UspsModelSerializer()
        {
            EmptyNamespace.Add("", "");
        }


        public static string Serialize<T>(this T toSerialize)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType());

            var xws = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8
            };

            using (var ms = new MemoryStream())
            {
                var xtw = XmlWriter.Create(ms, xws);
                xmlSerializer.Serialize(xtw, toSerialize, EmptyNamespace);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static T Deserialize<T>(this string xml) where T : new()
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringReader = new XmlReader(xml);
            var xmlObject = (T)xmlSerializer.Deserialize(stringReader);
            return xmlObject;
        }
    }
}