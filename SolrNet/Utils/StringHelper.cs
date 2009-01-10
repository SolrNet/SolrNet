namespace SolrNet.Utils {
    public class StringHelper {
        /// <summary>
        /// Converts an object to null string if null or executes ToString() on it.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToNullOrString(object o) {
            if (o == null)
                return null;
            return o.ToString();
        }
    }
}