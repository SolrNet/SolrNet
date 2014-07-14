using System;

namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// </summary>
    public class CursorMarkOption
    {
        /// <summary>
        ///     Use deep pagination via Solr CursorMark
        ///     IMPORTANT. <see cref="CommonQueryOptions.Start" /> must have a value of 0.
        ///     IMPROTANT. <see cref="QueryOptions.OrderBy" /> must contain at least one UNIQUE field
        ///     and optionally any other fields.
        ///     In order to use cursorMark in SOLR there must be sorting on at least one unique field.
        ///     In addition other non uniques fields can also be used for sorting.
        /// </summary>
        /// <param name="cursorMark">
        ///     <see cref="CursorMarkOption.CursorMark" />
        /// </param>
        public CursorMarkOption(string cursorMark = "*")
        {
            CursorMark = cursorMark;
        }

        /// <summary>
        ///     The pagination cursorMark token used to paginate or the default startvalue '*'
        /// </summary>
        public string CursorMark { get; set; }
    }
}