using System.Security.Cryptography;

namespace SortLab.Core;

public static class RandomUtil
{
    [ThreadStatic]
    private static Random random;

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
    }

    public static Func<Random> RandomFactory { get; set; }

    public static Random ThreadRandom
    {
        get
        {
            random ??= RandomFactory();
            return random;
        }
    }
}
