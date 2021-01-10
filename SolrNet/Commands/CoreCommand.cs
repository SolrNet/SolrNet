using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolrNet.Commands {
    public class CoreCommand : ISolrCommand {
        /// <summary>
        /// List of Parameters that will be sent to the /admin/cores handler.
        /// </summary>
        protected readonly List<KeyValuePair<string, string>> Parameters = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Executes a Core command
        /// </summary>
        /// <param name="connection">The SolrConnection to use.</param>
        /// <returns>The results of the Command.</returns>
        public string Execute(ISolrConnection connection) {
            return connection.Get("/admin/cores", Parameters.ToArray());
        }

        /// <inheritdoc />
        public Task<string> ExecuteAsync(ISolrConnection connection)
        {
            return connection.GetAsync("/admin/cores", Parameters.ToArray());
        }

        /// <summary>
        /// Adds the specified parameter to the current command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void AddParameter(string key, string value) {
            Parameters.Add(new KeyValuePair<string, string>(key, value));
        }

        public IEnumerable<KeyValuePair<string, string>> GetParameters() {
            return Parameters;
        }
    }
}
