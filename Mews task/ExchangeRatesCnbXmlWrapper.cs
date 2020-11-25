using System;
using System.Globalization;
using System.Xml.Serialization;

namespace ExchangeRateUpdater
{
    /// <summary>
    /// Represents an exchange rates XML result from Czech National Bank API.
    /// </summary>
    [Serializable, XmlRoot("kurzy")]
    public class ExchangeRatesCnbXmlWrapper
    {
        [XmlElement(ElementName = "tabulka")]
        public Table Table { get; set; }

        [XmlAttribute(AttributeName = "banka")]
        public string Bank { get; set; }

        [XmlAttribute(AttributeName = "datum")]
        public string DateStr
        {
            get => DateUtc.ToString(CultureInfo.CurrentCulture);
            set
            {
                DateUtc = DateTime.SpecifyKind(DateTime.Parse(value, CultureInfo.GetCultureInfo("cs-CZ")), DateTimeKind.Utc);
            }
        }

        public DateTime DateUtc { get; set; }

        [XmlAttribute(AttributeName = "poradi")]
        public int Order { get; set; }
    }

    public class Table
    {
        [XmlElement(ElementName = "radek")]
        public ExchangeRateCnb[] ExchangeRates { get; set; }

        [XmlAttribute(AttributeName = "typ")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Represents an specific exchange rate against Czech Crown from Czech National Bank API.
    /// </summary>
    public class ExchangeRateCnb
    {
        [XmlAttribute(AttributeName = "kod")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "mena")]
        public string Currency { get; set; }

        [XmlAttribute(AttributeName = "mnozstvi")]
        public int Amount { get; set; }

        [XmlAttribute(AttributeName = "zeme")]
        public string Country { get; set; }

        [XmlAttribute(AttributeName = "kurz")]
        public string ValueStr
        {
            get => Value.ToString(CultureInfo.CurrentCulture);
            set
            {
                Value = decimal.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);
            }
        }

        public decimal Value { get; set; }
    }
}
