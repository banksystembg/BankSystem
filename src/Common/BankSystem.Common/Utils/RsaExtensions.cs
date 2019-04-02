namespace BankSystem.Common.Utils
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;

    public static class RsaExtensions
    {
        public static void FromXmlString(this RSA rsa, string xmlString)
        {
            var parameters = new RSAParameters();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus":
                            parameters.Modulus = Convert.FromBase64String(node.InnerText);

                            break;
                        case "Exponent":
                            parameters.Exponent = Convert.FromBase64String(node.InnerText);

                            break;
                        case "P":
                            parameters.P = Convert.FromBase64String(node.InnerText);

                            break;
                        case "Q":
                            parameters.Q = Convert.FromBase64String(node.InnerText);

                            break;
                        case "DP":
                            parameters.DP = Convert.FromBase64String(node.InnerText);

                            break;
                        case "DQ":
                            parameters.DQ = Convert.FromBase64String(node.InnerText);

                            break;
                        case "InverseQ":
                            parameters.InverseQ = Convert.FromBase64String(node.InnerText);

                            break;
                        case "D":
                            parameters.D = Convert.FromBase64String(node.InnerText);

                            break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        public static string ToXmlString(this RSA rsa, bool includePrivateParameters)
        {
            var rsaParams = rsa.ExportParameters(includePrivateParameters);
            var sb = new StringBuilder();

            sb.Append("<RSAKeyValue>");
            sb.Append("<Modulus>" + Convert.ToBase64String(rsaParams.Modulus) + "</Modulus>");
            sb.Append("<Exponent>" + Convert.ToBase64String(rsaParams.Exponent) + "</Exponent>");

            if (includePrivateParameters)
            {
                sb.Append("<P>" + Convert.ToBase64String(rsaParams.P) + "</P>");
                sb.Append("<Q>" + Convert.ToBase64String(rsaParams.Q) + "</Q>");
                sb.Append("<DP>" + Convert.ToBase64String(rsaParams.DP) + "</DP>");
                sb.Append("<DQ>" + Convert.ToBase64String(rsaParams.DQ) + "</DQ>");
                sb.Append("<InverseQ>" + Convert.ToBase64String(rsaParams.InverseQ) + "</InverseQ>");
                sb.Append("<D>" + Convert.ToBase64String(rsaParams.D) + "</D>");
            }

            sb.Append("</RSAKeyValue>");

            return sb.ToString();
        }
    }
}