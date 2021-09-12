using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameJam
{
    public class Map
    {
        public readonly int width;
        public readonly int height;
        private List<Texture2D> layerImages;
        public int[,] grid;

        public Map(int w, int h)
        {
            width = w;
            height = h;

            layerImages = new List<Texture2D>();
            grid = new int[w, h];
        }

        public void AddLayerImage(Texture2D image)
        {
            layerImages.Add(image);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Texture2D layer in layerImages)
            {
                spriteBatch.Draw(layer, Vector2.Zero, Color.White);
            }
        }
    }
}
