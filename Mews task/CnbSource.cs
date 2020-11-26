using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ExchangeRateUpdater
{
    /// <summary>
    /// Defines the behavior of Czech National Bank source and provides a base for derived classes.
    /// </summary>
    public abstract class CnbSource
    {
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(ExchangeRatesCnbXmlWrapper), defaultNamespace: "");

        public abstract IEnumerable<ExchangeRateCnb> GetExchangeRates();

        protected ExchangeRatesCnbXmlWrapper Deserialize(Stream xmlStream)
        {
            return (ExchangeRatesCnbXmlWrapper)_serializer.Deserialize(xmlStream);
        }
    }
}
