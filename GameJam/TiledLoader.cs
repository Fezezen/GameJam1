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

                //Texture2D[] tileTextures = LoadTiles(d_Map);
                Dictionary<int, Color[]> tileTextures = LoadTiles(d_Map);

                Map map = new Map(d_Map.width, d_Map.height, d_Map.tileWidth)
                {
                    music = d_Map.properties.Find(e => e.name == "Music").value,
                    instrument = d_Map.properties.Find(e => e.name == "Instrument").value
                };

                if (d_Map.properties.Exists(p => p.name == "PlayerX"))
                    map.playerX = int.Parse(d_Map.properties.Find(p => p.name == "PlayerX").value);

                if (d_Map.properties.Exists(p => p.name == "PlayerY"))
                    map.playerY = int.Parse(d_Map.properties.Find(p => p.name == "PlayerY").value);

                map.instruPos = new Vector2(float.Parse(d_Map.properties.Find(p => p.name == "InstrumentX").value), float.Parse(d_Map.properties.Find(p => p.name == "InstrumentY").value));
                map.isFinale = d_Map.properties.Exists(e => e.name == "Finale");

                if (d_Map.properties.Exists(p => p.name == "NextLevel"))
                    map.nextLevel = d_Map.properties.Find(p => p.name == "NextLevel").value;

                foreach (D_Layer layer in d_Map.layers)
                {
                    switch (layer.type)
                    {
                        case "tilelayer":
                            if (layer.name != "_Collision")
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
                                            //Texture2D tile = tileTextures[i - 1]; // subtract given index by 1 because 0 is empty space
                                            if (tileTextures.ContainsKey(i-1))
                                                PlaceTileOnColorData(tileTextures[i-1], d_Map.tileWidth, d_Map.tileHeight, ref colorData, x, y, texture.Width, texture.Height);

                                        }
                                    }
                                }

                                texture.SetData(colorData);
                                map.AddLayerImage(texture);
                            } else
                            {                                
                                SetUpCollision(ref d_Map, layer, ref map);
                            }
                            break;
                        default:
                            break;
                    }
                }
                
                return map;
            } else
            {
                System.Console.WriteLine("Map doesn't exist at " + path);
            }

            return null;
        }

        private static Dictionary<int, Color[]> LoadTiles(D_Map d_Map)
        {
            //int n_tiles = 0;
            /*foreach (D_Tileset d_Tileset in d_Map.tilesets)
                if (d_Tileset.name != "CollisionSet")
                    n_tiles += (d_Tileset.tilecount+d_Tileset.firstgid)-1;*/

            //Color[][] tileTextures = new Color[n_tiles][];
            Dictionary<int, Color[]> tileTextures = new Dictionary<int, Color[]>();

            foreach (D_Tileset d_Tileset in d_Map.tilesets)
            {
                if (d_Tileset.name != "CollisionSet")
                {
                    string t_dir = d_Tileset.image.Substring(3, d_Tileset.image.Length - 7);
                    Texture2D tilesetImage = Program.Engine.Content.Load<Texture2D>(t_dir);
                    Color[] tilesetColor = new Color[tilesetImage.Width * tilesetImage.Height];
                    tilesetImage.GetData(tilesetColor);
                    int rows = (d_Tileset.tilecount / d_Tileset.columns); // number of rows in the texture

                    int i = d_Tileset.firstgid - 1; //-1 because ID 0 is always clear space, and therefore not needed;

                    for (int y = 0; y < rows; y++) // run across row by row, not column by column. 
                    {
                        for (int x = 0; x < d_Tileset.columns; x++)
                        {
                            //Texture2D tileTexture = new Texture2D(Program.Engine.GraphicsDevice, d_Map.tileWidth, d_Map.tileHeight);
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

                            tileTextures[i] = tColor;
                            //tileTexture.SetData(tColor);
                            //tileTextures[i] = tileTexture;
                            i++;
                        }
                    }
                }
            }

            return tileTextures;
        }

        private static void PlaceTileOnColorData(Color[] tile, int tileWidth, int tileHeight, ref Color[] color, int px, int py, int imageWidth, int imageHeight)
        {
            //Color[] tileColor = new Color[tileWidth * tileHeight];
            //tile.GetData(tileColor);

            for (int x = 0; x < tileWidth; x++)
            {
                for (int y = 0; y < tileHeight; y++)
                {
                    int cx = px * tileWidth + x;
                    int cy = py * tileHeight + y;

                    color[cx + imageWidth * cy] = tile[x + tileWidth * y];
                }
            }
        }

        private static void SetUpCollision(ref D_Map d_Map, D_Layer layer, ref Map map)
        {            
            if (d_Map.tilesets.Exists(t => t.name == "CollisionSet"))
            {
                D_Tileset colSet = d_Map.tilesets.Find(t => t.name == "CollisionSet");
                int max = colSet.firstgid-1;
                
                for (int x = 0; x < layer.width; x++)
                {
                    for (int y = 0; y < layer.height; y++)
                    {
                        int i = layer.data[x + layer.width * y] - max;
                        map.grid[x, y] = i;
                    }
                }
            }
        }
    }

    struct D_Map
    {
        public int width;
        public int height;
        public int tileWidth;
        public int tileHeight;

        public List<D_Tileset> tilesets;
        public List<D_Layer> layers;
        public List<D_CustomProperty> properties;
    }

    struct D_Tileset
    {
        public int columns;
        public int firstgid;
        public string image;
        public int tilecount;
        public string name;
    }

    struct D_Layer
    {
        public int[] data;
        public int width;
        public int height;
        public string name;
        public string type;
    }

    struct D_CustomProperty
    {
        public string name;
        public string value; 
    }
}
