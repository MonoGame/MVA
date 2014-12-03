using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Wumpus
{
    static class Extensions
    {
        static public Vector2 GetHalfSize(this Texture2D tex)
        {
            return new Vector2(tex.Width / 2.0f, tex.Height / 2.0f);
        }
    }
}
