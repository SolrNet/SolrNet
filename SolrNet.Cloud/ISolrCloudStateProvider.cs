using System;
using System.Threading.Tasks;

namespace SolrNet.Cloud
{
    /// <summary>
    /// Solr cloud state provider interface
    /// </summary>
    public interface ISolrCloudStateProvider : IDisposable {
        
        /// <summary>
        /// Provider key
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Returns actual solr cloud state
        /// </summary>
        SolrCloudState GetCloudState();

        /// <summary>
        /// Reinitialize connection and get fresh cloud state.
        /// </summary>
        /// <remarks>Attention: This causes reloading all cloud data and maybe slow!</remarks>
        /// <returns>Refreshed Solr Cloud state</returns>
        Task<SolrCloudState> GetFreshCloudStateAsync();

        /// <summary>
        /// Provider initialization
        /// </summary>
        Task InitAsync();

        Task DisposeAsync();
    }
}
