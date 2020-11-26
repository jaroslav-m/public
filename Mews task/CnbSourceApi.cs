using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ExchangeRateUpdater
{
    /// <summary>
    /// Provides communication with Czech National Bank API.
    /// </summary>
    public class CnbSourceApi : CnbSource
    {
        // TODO: would be nice to move CNB API URL to App.config file
        private const string CnbApiUrl = "https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.xml?ref=public-apis";

        /// <summary>
        /// Gets current exchange rates.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ExchangeRateCnb> GetExchangeRates()
        {
            var request = WebRequest.CreateHttp(CnbApiUrl);
            request.Method = "GET";

            // TODO: here should be handling transient faults by using Re-Try policy

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return Deserialize(stream).Table.ExchangeRates;
            };
        }
    }
}
