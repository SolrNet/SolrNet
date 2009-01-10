namespace SolrNet.Utils {
    public class StringHelper {
        public static string ToNullOrString(object o) {
            if (o == null)
                return null;
            return o.ToString();
        }
    }
}