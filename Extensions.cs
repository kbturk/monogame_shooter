using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace shooter;

public static class Extensions
{
    public static float ToAngle(this Vector2 vector)
    {
        return (float)Math.Atan2(vector.Y, vector.X);
    }

    public static float NextFloat(this Random rand, float minValue, float maxValue) {
        return (float)rand.NextDouble() * (maxValue - minValue) + minValue;
    }
}

public record class MathUtil {

    //Math Utility Functions
    public static Vector2 FromPolar(float angle, float magnitude)
    {
        return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }

}
