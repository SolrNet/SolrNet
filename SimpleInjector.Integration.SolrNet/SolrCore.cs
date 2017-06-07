using System;

namespace SimpleInjector.Integration.SolrNet
{
    /// <summary>
    /// SolR core
    /// </summary>
    class SolrCore
    {
        /// <summary>
        /// Core connection URL
        /// </summary>
        public string ConnectionUrl { get; set; }

        /// <summary>
        /// Document type for this core
        /// </summary>
        public Type DocumentType { get; set; }
    }
}
