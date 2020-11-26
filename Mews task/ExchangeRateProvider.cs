using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeRateUpdater
{
    public class ExchangeRateProvider
    {
        private readonly CnbSource _source;

        public ExchangeRateProvider()
        {
            _source = new CnbSourceApi();
        }

        public ExchangeRateProvider(CnbSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Should return exchange rates among the specified currencies that are defined by the source. But only those defined
        /// by the source, do not return calculated exchange rates. E.g. if the source contains "CZK/USD" but not "USD/CZK",
        /// do not return exchange rate "USD/CZK" with value calculated as 1 / "CZK/USD". If the source does not provide
        /// some of the currencies, ignore them.
        /// </summary>
        public IEnumerable<ExchangeRate> GetExchangeRates(IEnumerable<Currency> currencies)
        {
            var currenciesArr = currencies != null
                ? currencies.Where(a => !string.IsNullOrEmpty(a?.Code)).ToArray()
                : new Currency[0];

            if (currenciesArr.Count() < 2)
                yield break;

            var cnbExchangeRates = _source.GetExchangeRates().ToDictionary(er => er.Code, StringComparer.OrdinalIgnoreCase);
            cnbExchangeRates.Add("CZK", new ExchangeRateCnb { Code = "CZK", Amount = 1, Value = 1 });

            for (int s = 0; s < currenciesArr.Count(); s++)
            {
                var sourceCurrency = currenciesArr[s];
                if (!cnbExchangeRates.TryGetValue(sourceCurrency.Code, out var sourceCnbExchangeRate))
                    continue; // ignore

                for (int t = s + 1; t < currenciesArr.Count(); t++)
                {
                    var targetCurrency = currenciesArr[t];
                    if (!cnbExchangeRates.TryGetValue(targetCurrency.Code, out var targetCnbExchangeRate))
                        continue; // ignore

                    var exchangeRate =
                        (sourceCnbExchangeRate.Value / sourceCnbExchangeRate.Amount) /
                        (targetCnbExchangeRate.Value / targetCnbExchangeRate.Amount);

                    yield return new ExchangeRate(sourceCurrency, targetCurrency, exchangeRate);
                }
            }
        }
    }
}
