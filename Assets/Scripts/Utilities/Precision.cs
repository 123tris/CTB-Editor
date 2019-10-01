﻿using System;
using UnityEngine;

public static class Precision
{
    public const float FLOAT_EPSILON = 1e-3f;
    public const double DOUBLE_EPSILON = 1e-7;

    public static bool AlmostEquals(float value1, float value2, float acceptableDifference = FLOAT_EPSILON)
    {
        return Math.Abs(value1 - value2) <= acceptableDifference;
    }

    public static bool AlmostEquals(Vector2 value1, Vector2 value2, float acceptableDifference = FLOAT_EPSILON)
    {
        return AlmostEquals(value1.x, value2.x, acceptableDifference) && AlmostEquals(value1.y, value2.y, acceptableDifference);
    }

    public static bool AlmostEquals(double value1, double value2, double acceptableDifference = DOUBLE_EPSILON)
    {
        return Math.Abs(value1 - value2) <= acceptableDifference;
    }
}