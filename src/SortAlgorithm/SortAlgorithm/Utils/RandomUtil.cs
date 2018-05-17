using System;
using System.Collections.Generic;
using System.Text;

namespace SortAlgorithm
{
    public static class RandomUtil
    {
        [ThreadStatic]
        private static Random random;

        static RandomUtil()
        {
            RandomFactory = () =>
            {
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    var buffer = new byte[sizeof(int)];
                    rng.GetBytes(buffer);
                    var seed = BitConverter.ToInt32(buffer, 0);
                    return new Random(seed);
                }
            };
        }

        public static Func<Random> RandomFactory { get; set; }

        public static Random ThreadRandom
        {
            get
            {
                if (random == null)
                {
                    random = RandomFactory();
                }

                return random;
            }
        }
    }
}
