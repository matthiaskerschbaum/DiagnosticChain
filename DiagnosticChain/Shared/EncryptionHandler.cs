using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Shared
{
    public static class EncryptionHandler
    {
        public class KeyPair
        {
            public RSAParameters PublicKey { get; set; }
            public RSAParameters PrivateKey { get; set; }
        }

        public static KeyPair GenerateNewKeys()
        {
            var csp = new RSACryptoServiceProvider(2048);

            return new KeyPair()
            {
                PublicKey = csp.ExportParameters(false)
                ,
                PrivateKey = csp.ExportParameters(true)
            };
        }

        public static string Key2String(RSAParameters parameters)
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, parameters);
            return sw.ToString();
        }

        public static RSAParameters String2Key(string key)
        {
            var sr = new StringReader(key);
            var xs = new XmlSerializer(typeof(RSAParameters));
            return (RSAParameters)xs.Deserialize(sr);
        }

        public static string Encrypt(string message, RSAParameters key)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(key);

            var byteMessage = Encoding.Unicode.GetBytes(message);
            var cypher = csp.Encrypt(byteMessage, false);
            return Convert.ToBase64String(cypher);
        }

        public static string Sign(string data, RSAParameters key)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(key);

            var byteData = Encoding.Unicode.GetBytes(data);
            var cypher = csp.SignData(byteData, CryptoConfig.MapNameToOID("SHA256"));
            return Convert.ToBase64String(cypher);
        }

        public static string Decrypt(string cypher, RSAParameters key)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(key);
            var byteCypher = Convert.FromBase64String(cypher);
            var message = csp.Decrypt(byteCypher, false);
            return Encoding.Unicode.GetString(message);
        }

        public static bool VerifiySignature(string data, string signature, RSAParameters key)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(key);
            var byteData = Encoding.Unicode.GetBytes(data);
            var byteSiganture = Convert.FromBase64String(signature);
            return csp.VerifyData(byteData, CryptoConfig.MapNameToOID("SHA256"), byteSiganture);
        }
    }
}
