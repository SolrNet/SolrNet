namespace SolrNet.Utils {
	public interface IRNG {
		int Next();
		int Next(int minValue, int maxValue);
		int Next(int maxValue);
		double NextDouble();
		void NextBytes(byte[] buffer);
	}
}