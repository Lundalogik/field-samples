using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;


namespace CaseExportAndCommands
{
    class Program
    {
        private string _serviceUri = ConfigurationManager.AppSettings["serviceURI"];
        private string _username = ConfigurationManager.AppSettings["username"];
        private string _password = ConfigurationManager.AppSettings["password"];

        static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            XmlDocument xml = GetCasesForExport();
            XmlNodeList casesForExport = xml.SelectNodes(@"//*[local-name()='Case']");
            var faultyCases = new List<string>();
            var validCases = new List<string>();

            foreach (XmlNode caseForExport in casesForExport)
            {
                var caseId = caseForExport.SelectSingleNode(@"*[local-name()='Id']").InnerText;
                //Do some validation
                //Example of validation All cases missing zipcode is faulty and will be updated with Error in ExternalNote, No CrmsystemId and a error description in ExternalComment
                var zipcodeElement =
                    caseForExport.SelectSingleNode(@"//*[local-name()='BillingAddress']/*[local-name()='Zip']");

                if (zipcodeElement == null || String.IsNullOrEmpty(zipcodeElement.InnerText))
                {
                    faultyCases.Add(caseId);
                }
                else
                {
                    validCases.Add(caseId);
                }
            }
            UpdateCases(validCases, faultyCases);
        }

        private void UpdateCases(List<string> validCases, List<string> faultyCases)
        {
            var batchStart = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                <CommandBatch xmlns=""http://schemas.remotex.net/Apps/201207/Commands"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                                <Id></Id>
                                <ContinueOnError>true</ContinueOnError>";

            var builder = new StringBuilder(batchStart);

            foreach (var validCaseId in validCases)
            {
                builder.Append("<Command>");
                builder.Append("<Name>UpdateCase</Name>");
                builder.AppendFormat("<Target>cases/{0}</Target>", validCaseId);

                AddParameter(builder, "CrmSystemId", GetCRMSystemId());
                AddParameter(builder, "ExternalNote", "OK");
               

                builder.Append("</Command>");
            }

            foreach (var faultyCaseId in faultyCases)
            {
                builder.Append("<Command>");
                builder.Append("<Name>UpdateCase</Name>");
                builder.AppendFormat("<Target>cases/{0}</Target>", faultyCaseId);

                AddParameter(builder, "ExternalNote", "Error");
                AddParameter(builder, "ExternalComment", "Zipcode error");
               

                builder.Append("</Command>");
            }

            builder.Append("</CommandBatch>");

            var commandBatch = new XmlDocument();
            commandBatch.LoadXml(builder.ToString());
            XmlDocument respons = SendCommands(commandBatch);
        }

        private XmlDocument SendCommands(XmlDocument commandBatch)
        {
            var request = (HttpWebRequest)WebRequest.Create(_serviceUri + @"/commands");
            request.Accept = "application/xml";
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.Credentials = new NetworkCredential(_username, _password);
            request.PreAuthenticate = true;

            var requestStream = request.GetRequestStream();

            using (var writer = XmlWriter.Create(requestStream))
            {
                commandBatch.WriteTo(writer);
            }
            //Request message 2UpdateCaseRequest.txt
            //Respons message 3AfterUpdateResponse.txt
            var response = request.GetResponse();

            using (var responseStream = response.GetResponseStream())
            {
                var doc = new XmlDocument();
                doc.Load(responseStream);

                return doc;
            }
        }

        private string GetCRMSystemId()
        {
            //Generates fake CRMsystemId, should be replaced to external Id
            return DateTime.UtcNow.Ticks.ToString();
        }

        private static void AddParameter(StringBuilder builder, string name, string value)
        {
            builder.AppendFormat("<Parameter><Name>{0}</Name><Value>{1}</Value></Parameter>", name, value);
        }

        private XmlDocument GetCasesForExport()
        {
            var request = (HttpWebRequest)WebRequest.Create(_serviceUri + @"/caseexport/rmx");
            request.Accept = "application/xml";
            request.Credentials = new NetworkCredential(_username, _password);
            request.PreAuthenticate = true;

            //Response message 1CaseExportRespons.txt
            var response = request.GetResponse();

            using (var responseStream = response.GetResponseStream())
            {
                var doc = new XmlDocument();
                doc.Load(responseStream);

                return doc;
            }
        }
    }
}
