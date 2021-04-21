public class MathExtensions
{
    public static double Clamp(double val, double min, double max)
    {
        if (val.CompareTo(min) < 0) return min;
        else if (val.CompareTo(max) > 0) return max;
        else return val;
    }
}
