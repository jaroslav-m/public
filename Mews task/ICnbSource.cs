using System.Collections.Generic;

namespace ExchangeRateUpdater
{
    public interface ICnbSource
    {
        IEnumerable<ExchangeRateCnb> GetExchangeRates();
    }
}
