namespace OMEGA
{
    public static class MathUtil
    {
        // Maps a value N from [min, max] to [newMin, newMax]
        public static float Map(float n, float min, float max, float newMin, float newMax)
        {
            return ((n - min) / (max - min)) * (newMax - newMin) + newMin;
        }
    }
}