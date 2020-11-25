using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace ExchangeRateUpdater
{
    /// <summary>
    /// Provides communication with Czech National Bank API.
    /// </summary>
    public class CnbSourceApi : ICnbSource
    {
        // TODO: would be nice to move CNB API URL to App.config file
        private const string CnbApiUrl = "https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.xml?ref=public-apis";

        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(ExchangeRatesCnbXmlWrapper));

        /// <summary>
        /// Gets current exchange rates.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExchangeRateCnb> GetExchangeRates()
        {
            var request = WebRequest.CreateHttp(CnbApiUrl);
            request.Method = "GET";

            // TODO: here should be handling transient faults by using Re-Try policy

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return ((ExchangeRatesCnbXmlWrapper)_serializer.Deserialize(stream)).Table.ExchangeRates;
            };
        }
    }
}
