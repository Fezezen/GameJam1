using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameJam
{
    public class Map
    {
        public Matrix Transform { get; private set; }
        public readonly int width;
        public readonly int height;
        public int tilesize;
        private List<Texture2D> layerImages;
        public int[,] grid;

        public int playerX;
        public int playerY;

        public string music;
        public string instrument;
        public Vector2 instruPos;

        public bool isFinale;
        public string nextLevel;

        public Map(int w, int h, int tileSize)
        {
            width = w;
            height = h;

            layerImages = new List<Texture2D>();
            grid = new int[w, h];
            tilesize = tileSize;
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
