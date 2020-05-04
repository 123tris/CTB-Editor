using System;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 GetDeviationScore(this Vector3 vector3, Vector3 mean, Vector3 stdDev)
    {
        return new Vector3(
            Mathf.RoundToInt((vector3.x - mean.x) / stdDev.x),
            Mathf.RoundToInt((vector3.y - mean.y) / stdDev.y),
            Mathf.RoundToInt((vector3.z - mean.z) / stdDev.z));
    }

    public static Vector3 Parse(string s, IFormatProvider provider)
    {
        string[] parts = s.Split(',');

        if (parts.Length != 3)
        {
            throw new ArgumentException();
        }

        parts[0] = parts[0].Remove(0, 1);
        parts[2] = parts[2].Remove(parts[2].Length - 1, 1);

        return new Vector3(
            float.Parse(parts[0].Trim(), provider),
            float.Parse(parts[1].Trim(), provider),
            float.Parse(parts[2].Trim(), provider));
    }

    public static string ToString(this Vector3 vector, string format, IFormatProvider provider)
    {
        return String.Format("({0}, {1}, {2})",
            vector.x.ToString(format, provider),
            vector.y.ToString(format, provider),
            vector.z.ToString(format, provider));
    }

    public static string ToString(this Vector3 vector, IFormatProvider provider)
    {
        return String.Format("({0}, {1}, {2})",
            vector.x.ToString(provider),
            vector.y.ToString(provider),
            vector.z.ToString(provider));
    }

    public static Vector3 FlattenX(this Vector3 vector)
    {
        return new Vector3(0, vector.y, vector.z);
    }

    public static Vector3 FlattenY(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static Vector3 FlattenZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector3 ToInt(this Vector3 vector)
    {
        vector.x = Mathf.Round(vector.x);
        vector.y = Mathf.Round(vector.y);
        vector.z = Mathf.Round(vector.z);
        return vector;
    }

    //TODO: Create own extension classes for vector2int and vector2

    public static Vector2 ToVector2(this Vector3 vector)
    {
        Vector2 output = new Vector2
        {
            x = vector.x,
            y = vector.y
        };
        return output;
    }

    public static Vector2Int ToVector2Int(this Vector3 vector)
    {
        Vector2Int output = new Vector2Int();
        output.x = (int) Mathf.Round(vector.x);
        output.y = (int) Mathf.Round(vector.y);
        return output;
    }

    public static System.Numerics.Vector2 ToNumerical(this Vector2Int vector)
    {
        return new System.Numerics.Vector2(vector.x, vector.y);
    }

    public static Vector2 ToUnityVector(this System.Numerics.Vector2 vector)
    {
        return new Vector2(vector.X,vector.Y);
    }

    public static Vector2Int ToVector2Int(this Vector2 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x),Mathf.RoundToInt(vector.y));
    }

    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x,vector.y);
    }
}
