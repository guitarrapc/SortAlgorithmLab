using System.Security.Cryptography;

namespace SortLab.Core;

public static class RandomUtil
{
    [ThreadStatic]
    private static Random random;

    public static Func<Random> RandomFactory { get; }
    public static Random ThreadRandom => random;

    static RandomUtil()
    {
        RandomFactory = () =>
        {
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[sizeof(int)];
            rng.GetBytes(buffer);
            var seed = BitConverter.ToInt32(buffer, 0);
            return new Random(seed);
        };
        random = RandomFactory();
    }
}
