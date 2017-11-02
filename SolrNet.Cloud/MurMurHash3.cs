using System.IO;

// https://gist.github.com/automatonic/3725443
namespace SolrNet.Cloud {
    public static class MurMurHash3 {
        const uint Seed = 144;

        public static int Compute(Stream stream)
        {
            const uint C1 = 0xcc9e2d51;
            const uint C2 = 0x1b873593;

            var h1 = Seed;
            uint streamLength = 0;

            using (var reader = new BinaryReader(stream))
            {
                var chunk = reader.ReadBytes(4);
                while (chunk.Length > 0)
                {
                    streamLength += (uint)chunk.Length;
                    uint k1;
                    switch (chunk.Length)
                    {
                        case 4:
                            /* Get four bytes from the input into an uint */
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8
                              | chunk[2] << 16
                              | chunk[3] << 24);

                            /* bitmagic hash */
                            k1 *= C1;
                            k1 = Rotl32(k1, 15);
                            k1 *= C2;

                            h1 ^= k1;
                            h1 = Rotl32(h1, 13);
                            h1 = h1 * 5 + 0xe6546b64;
                            break;
                        case 3:
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8
                              | chunk[2] << 16);
                            k1 *= C1;
                            k1 = Rotl32(k1, 15);
                            k1 *= C2;
                            h1 ^= k1;
                            break;
                        case 2:
                            k1 = (uint)
                               (chunk[0]
                              | chunk[1] << 8);
                            k1 *= C1;
                            k1 = Rotl32(k1, 15);
                            k1 *= C2;
                            h1 ^= k1;
                            break;
                        case 1:
                            k1 = chunk[0];
                            k1 *= C1;
                            k1 = Rotl32(k1, 15);
                            k1 *= C2;
                            h1 ^= k1;
                            break;

                    }
                    chunk = reader.ReadBytes(4);
                }
            }

            // finalization, magic chants to wrap it all up
            h1 ^= streamLength;
            h1 = Fmix(h1);

            unchecked //ignore overflow
            {
                return (int)h1;
            }
        }

        private static uint Rotl32(uint x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }

        private static uint Fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }
    }
}
