using System;
using System.Text;

namespace SolrNet {
    public interface ISolrDIHOperations {
        /// <summary>
        /// Full Import operation
        /// </summary>
        /// <param name="options">The data import options</param>
        /// <returns></returns>
        SolrDIHStatus FullImport(DIHOptions options);

        /// <summary>
        /// For incremental imports and change detection.
        /// It supports the same clean, commit, optimize and debug parameters.
        /// </summary>
        /// <param name="options">The data import options</param>
        /// <returns></returns>
        SolrDIHStatus DeltaImport(DIHOptions options);

        /// <summary>
        ///  If the data-config is changed and you wish to reload the file without restarting Solr.
        /// </summary>
        /// <returns></returns>
        SolrDIHStatus ReloadConfig();

        /// <summary>
        /// Abort an ongoing operation.
        /// </summary>
        /// <returns></returns>
        SolrDIHStatus Abort();

        /// <summary>
        /// To know the status of the current command.
        /// It gives an elaborate statistics on no. of docs created, deleted, queries run, rows fetched, status etc.
        /// </summary>
        /// <returns></returns>
        SolrDIHStatus Status();
    }
}