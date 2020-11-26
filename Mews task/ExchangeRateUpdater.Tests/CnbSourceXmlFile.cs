using System.Collections.Generic;
using System.IO;

namespace ExchangeRateUpdater.Tests
{
    class CnbSourceXmlFile : CnbSource
    {
        public ExchangeRatesCnbXmlWrapper Data { get; private set; }

        public void Load(string xmlFilePath)
        {
            using var stream = new FileStream(xmlFilePath, FileMode.Open);
            Data = Deserialize(stream);
        }

        public override IEnumerable<ExchangeRateCnb> GetExchangeRates()
        {           
            return Data.Table.ExchangeRates;
        }
    }
}
