using System.IO;
using System.IO.Compression;
using System.Text;

namespace SolrNet.Tests.Utils
{
    public class CompressionUtils
    {
        public static Stream GzipCompressStream(string textToCompress)
        {
            var data = Encoding.GetEncoding("iso-8859-1").GetBytes(textToCompress);

			using (var ms = new MemoryStream()) {

				using (var zip = new GZipStream(ms, CompressionMode.Compress)) {
					zip.Write(data, 0, data.Length);
				}

				return new MemoryStream(ms.ToArray());
			}
        }

        public static Stream DeflateCompressStream(string textToCompress)
        {
            var data = Encoding.GetEncoding("iso-8859-1").GetBytes(textToCompress);

			using (var ms = new MemoryStream()) {

				using (var zip = new DeflateStream(ms, CompressionMode.Compress)) {
					zip.Write(data, 0, data.Length);
				}

				return new MemoryStream(ms.ToArray());
			}
        }
    }
}