using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Wumpus
{
    static class Helpers
    {
        static public Vector2 GetHalfSize(this Texture2D tex)
        {
            return new Vector2(tex.Width / 2.0f, tex.Height / 2.0f);
        }

        static public float EaseInOut(float step)
        {
            if (step < 0.5)
                return (float)(Math.Sin(step * MathHelper.Pi - (MathHelper.Pi * 0.5f)) + 1) * 0.5f;
            else
                return (float)(Math.Sin(step * MathHelper.Pi - (MathHelper.Pi * 0.5f)) + 1) * 0.5f;
        }
    }
}
