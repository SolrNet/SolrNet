using System;
using System.Globalization;

namespace SolrNet {
    /// <summary>
    /// Money type, for use with Solr CurrencyField
    /// </summary>
    [Serializable]    
    public class Money {
        /// <summary>
        /// Nominal value
        /// </summary>
        public readonly decimal Value;

        /// <summary>
        /// Currency code (e.g. USD)
        /// </summary>
        public readonly string Currency;

        /// <summary>
        /// Money type, for use with Solr CurrencyField
        /// </summary>
        /// <param name="value">Nominal value</param>
        /// <param name="currency">Currency code (e.g. USD)</param>
        public Money(decimal value, string currency) {
            Value = value;
            Currency = currency;
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Value, string.IsNullOrEmpty(Currency) ? "" : ",", Currency);
        }
    }
}
