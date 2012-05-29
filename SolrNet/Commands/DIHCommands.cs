namespace SolrNet.Commands {
    /// <summary>
    ///  The DataImportHandler operations
    /// </summary>
    public enum DIHCommands {
        /// <summary>
        /// To know the status of the current command.
        /// It gives an elaborate statistics on no. of docs created, deleted, queries run, rows fetched, status etc.
        /// </summary>
        Status,

        /// <summary>
        /// Full Import operation
        /// </summary>
        FullImport,

        /// <summary>
        /// For incremental imports and change detection.
        ///  It supports the same clean, commit, optimize and debug parameters.
        /// </summary>
        DeltaImport,

        /// <summary>
        /// Abort an ongoing operation.
        /// </summary>
        Abort,

        /// <summary>
        ///  If the data-config is changed and you wish to reload the file without restarting Solr
        /// </summary>
        ReloadConfig
    }
}