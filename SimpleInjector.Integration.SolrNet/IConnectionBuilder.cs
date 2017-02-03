using System;

namespace SimpleInjector.Integration.SolrNet
{
    /// <summary>
    /// SolrNet connection URL builder interface.
    /// </summary>
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Builds connection url based on entity type.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <returns>Connection string</returns>
        string Build<TEntity>();

        /// <summary>
        /// Builds connection url based on entity type.
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Connection string</returns>
        string Build(Type entityType);
    }
}
