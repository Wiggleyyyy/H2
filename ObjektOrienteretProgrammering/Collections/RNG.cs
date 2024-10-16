namespace Collections;
internal static class RNG
{
    private static Random s_random = new Random();
    public static int Range(int lower, int higher) { return s_random.Next(lower, higher); }
}