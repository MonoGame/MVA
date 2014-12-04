using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Wumpus
{
    static class Helpers
    {
        static public float EaseInOut(float step)
        {
            if (step < 0.5)
                return (float)(Math.Sin(step * MathHelper.Pi - (MathHelper.Pi * 0.5f)) + 1) * 0.5f;
            else
                return (float)(Math.Sin(step * MathHelper.Pi - (MathHelper.Pi * 0.5f)) + 1) * 0.5f;
        }
    }
}
