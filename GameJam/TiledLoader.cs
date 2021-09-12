using Microsoft.Xna.Framework;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam
{
    public static class TiledLoader
    {
        public static Map LoadMap(string dir)
        {
            string curPath = Directory.GetCurrentDirectory();
            string path = curPath + @"\Content\Levels\" + dir;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);

                D_Map d_Map = JsonConvert.DeserializeObject<D_Map>(json);

                Texture2D[] tileTextures = LoadTiles(d_Map);

                Map map = new Map(d_Map.width, d_Map.height);

                foreach (D_Layer layer in d_Map.layers)
                {
                    Texture2D texture = new Texture2D(Program.Engine.GraphicsDevice, d_Map.width * d_Map.tileWidth, d_Map.height * d_Map.tileHeight);
                    Color[] colorData = new Color[d_Map.width * d_Map.tileWidth * d_Map.height * d_Map.tileHeight];

                    for (int x = 0; x < layer.width; x++)
                    {
                        for (int y = 0; y < layer.height; y++)
                        {
                            int i = layer.data[x + layer.width * y];
                            if (i != 0)
                            {
                                Texture2D tile = tileTextures[i - 1]; // subtract given index by 1 because 0 is empty space
                                PlaceTileOnColorData(ref tile, ref colorData, x, y, texture.Width, texture.Height);
                            }
                        }
                    }

                    texture.SetData(colorData);
                    map.AddLayerImage(texture);
                }
                
                return map;
            } else
            {
                System.Console.WriteLine("Map doesn't exist at " + path);
            }

            return null;
        }

        private static Texture2D[] LoadTiles(D_Map d_Map)
        {
            int n_tiles = 0;
            foreach (D_Tileset d_Tileset in d_Map.tilesets)
                n_tiles += d_Tileset.tilecount;

            Texture2D[] tileTextures = new Texture2D[n_tiles];

            foreach (D_Tileset d_Tileset in d_Map.tilesets)
            {
                string t_dir = d_Tileset.image.Substring(3, d_Tileset.image.Length-7);
                System.Console.WriteLine(t_dir);
                Texture2D tilesetImage = Program.Engine.Content.Load<Texture2D>(t_dir);                
                Color[] tilesetColor = new Color[tilesetImage.Width* tilesetImage.Height];
                tilesetImage.GetData(tilesetColor);
                int rows = (d_Tileset.tilecount/d_Tileset.columns); // number of rows in the texture

                int i = d_Tileset.firstgid - 1; //-1 because ID 0 is always clear space, and therefore not needed;

                for (int y = 0; y < rows; y++) // run across row by row, not column by column. 
                {
                    for (int x = 0; x < d_Tileset.columns; x++)
                    {
                        Texture2D tileTexture = new Texture2D(Program.Engine.GraphicsDevice,d_Map.tileWidth,d_Map.tileHeight);
                        Color[] tColor = new Color[d_Map.tileWidth * d_Map.tileHeight];

                        for (int tx = 0; tx < d_Map.tileWidth; tx++)
                        {
                            for (int ty = 0; ty < d_Map.tileWidth; ty++)
                            {
                                int cx = x * d_Map.tileWidth + tx;
                                int cy = y * d_Map.tileHeight + ty;
                                tColor[tx + d_Map.tileWidth * ty] = tilesetColor[cx + tilesetImage.Width * cy];
                            }
                        }
                        tileTexture.SetData(tColor);
                        tileTextures[i] = tileTexture;
                        i++;
                    }
                }
            }

            return tileTextures;
        }

        private static void PlaceTileOnColorData(ref Texture2D tile, ref Color[] color, int px, int py, int imageWidth, int imageHeight)
        {
            Color[] tileColor = new Color[tile.Width * tile.Height];
            tile.GetData(tileColor);

            for (int x = 0; x < tile.Width; x++)
            {
                for (int y = 0; y < tile.Height; y++)
                {
                    int cx = px * tile.Width + x;
                    int cy = py * tile.Height + y;

                    color[cx + imageWidth * cy] = tileColor[x + tile.Width * y];
                }
            }
        }
    }

    class D_Map
    {
        public int width;
        public int height;
        public int tileWidth;
        public int tileHeight;

        public List<D_Tileset> tilesets;
        public List<D_Layer> layers;
    } 

    class D_Tileset
    {
        public int columns;
        public int firstgid;
        public string image;
        public int tilecount;
    }

    class D_Layer
    {
        public int[] data;
        public int width;
        public int height;
        public string name;
    }
}
