using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ExchangeRateUpdater.Tests
{
    public class Tests
    {
        public static TestCaseData[] CurrenciesSource = new[]
        {
            new TestCaseData { Codes = null },
            new TestCaseData { Codes = new string[0] },
            new TestCaseData { Codes = new string[0], IncludeNullCurrency = true },
            new TestCaseData { Codes = new string[] { "", }},
            new TestCaseData { Codes = new string[] { null }},
            new TestCaseData { Codes = new string[] { "", null }},
            new TestCaseData 
            { 
                Codes = new string[] {"USD"}, 
                ExpectedTesults = new string[0]
            },
            new TestCaseData
            {
                Codes = new string[] {"USD", "CZK"}, 
                ExpectedTesults = new string[] { "USD/CZK/22.128" }
            },
            new TestCaseData
            {
                Codes = new string[] {"usd", "czk", "RUB", "IDR", "xxx", "", null},
                ExpectedTesults = new string[]
                { 
                    "usd/czk/22.128", 
                    "usd/RUB/75.929039563531551315924921937",
                    "usd/IDR/14157.389635316698656429942418", 
                    "czk/RUB/3.4313557286483889784853995814",
                    "czk/IDR/639.79526551503518873960332694",
                    "RUB/IDR/186.45553422904670505438259757",
                }
            }
        };

        private CnbSourceXmlFile _source;

        [SetUp]
        public void Setup()
        {
            var testXmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "daily_rate.xml");
            _source = new CnbSourceXmlFile();
            _source.Load(testXmlPath);
        }

        [Test]
        public void DeserializeCnbApiResultTest()
        {
            var cnbResult = _source.Data;
            Assert.That(cnbResult.Bank, Is.EqualTo("CNB"));
            Assert.That(cnbResult.DateUtc, Is.EqualTo(new DateTime(2020, 11, 24, 0, 0, 0)));
            Assert.That(cnbResult.DateUtc.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(cnbResult.Order, Is.EqualTo(226));
            Assert.That(cnbResult.Table, Is.Not.Null);
            Assert.That(cnbResult.Table.Type, Is.EqualTo("XML_TYP_CNB_KURZY_DEVIZOVEHO_TRHU"));
            Assert.That(cnbResult.Table.ExchangeRates.Count(), Is.EqualTo(33));
            var cnbExchangeRate = cnbResult.Table.ExchangeRates.First();
            Assert.That(cnbExchangeRate.Code, Is.EqualTo("AUD"));
            Assert.That(cnbExchangeRate.Currency, Is.EqualTo("dolar"));
            Assert.That(cnbExchangeRate.Value, Is.EqualTo(16.234));
            Assert.That(cnbExchangeRate.Country, Is.EqualTo("Austrálie"));
        }

        [Test]
        public void CnbSourceApiTest()
        {
            var source = new CnbSourceApi();
            Assert.That(source.GetExchangeRates(), Is.Not.Empty);
        }

        [Test]
        public void DefaultExchangeRateProviderTest()
        {
            var currencies = new[]
            {
                new Currency("USD"),
                new Currency("EUR"),
            };
            var provider = new ExchangeRateProvider();
            var exchangeRates = provider.GetExchangeRates(currencies);
            Assert.That(exchangeRates.Count(), Is.EqualTo(1));
        }

        [Test, TestCaseSource("CurrenciesSource")]
        public void ExchangeRateProviderTest(TestCaseData testData)
        {
            var provider = new ExchangeRateProvider(_source);
            var exchangeRates = provider.GetExchangeRates(testData.Currencies).ToArray();
            Assert.That(exchangeRates.Count(), Is.EqualTo(testData.ExpectedTesults.Count()));
            
            for (var i = 0; i < testData.ExpectedTesults.Count(); i++)
            {
                var exchangeRate = exchangeRates[i];
                var expectedResult = testData.ExpectedTesults[i].Split('/');
                Assert.That(exchangeRate.SourceCurrency.Code, Is.EqualTo(expectedResult[0]));
                Assert.That(exchangeRate.TargetCurrency.Code, Is.EqualTo(expectedResult[1]));
                Assert.That(exchangeRate.Value, Is.EqualTo(decimal.Parse(expectedResult[2])));
            }
        }

        public class TestCaseData
        {
            public IEnumerable<string> Codes { get; set; }
            
            public string[] ExpectedTesults { get; set; } = new string[0];
            
            public bool IncludeNullCurrency { get; set; }

            public IEnumerable<Currency> Currencies
            {
                get
                {
                    var currencies = Codes?.Select(code => new Currency(code)).ToList();
                    if (IncludeNullCurrency)
                        currencies.Add(null);
                    return currencies;
                }
            }
        }
    }
}