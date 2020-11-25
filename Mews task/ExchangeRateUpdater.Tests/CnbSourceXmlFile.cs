using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ExchangeRateUpdater.Tests
{
    class CnbSourceXmlFile : ICnbSource
    {
        public ExchangeRatesCnbXmlWrapper Data { get; private set; }

        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(ExchangeRatesCnbXmlWrapper), defaultNamespace: "");

        public void Load(string xmlFilePath)
        {
            using var stream = new StreamReader(xmlFilePath);
            Data = (ExchangeRatesCnbXmlWrapper)_serializer.Deserialize(stream);
        }

        public IEnumerable<ExchangeRateCnb> GetExchangeRates()
        {           
            return Data.Table.ExchangeRates;
        }
    }
}
