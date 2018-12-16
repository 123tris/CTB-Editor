using System;

[Flags]
public enum HitObjectType : byte
{
    Fruit = 1,
    Slider = 2,
    NewCombo = 4,
    Spinner = 8
}