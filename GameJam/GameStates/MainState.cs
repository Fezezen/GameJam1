using GameJam.Objects;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam.GameStates
{
    public class MainState : GameState // This is the game state that handles the main part of the game where the player has control of their character.
    {
        public List<Entity> entities;
        public int[,] tiles;
        public int tileSize = 32;
        public Point gridSize;

        public Texture2D texture;

        Map map;
        public Camera camera;
        public Rectangle mapRect;

        public override void Initalize()
        {
            map = TiledLoader.LoadMap("Level1.json");

            entities = new List<Entity>
            {
                new Player(new Vector2(64,64))
            };

            gridSize = new Point(map.width,map.height);
            tiles = new int[gridSize.X,gridSize.Y];
            mapRect = new Rectangle(new Point(), new Point(gridSize.X * map.tilesize, gridSize.Y * map.tilesize));

            camera = new Camera();

            if (map != null)
                tiles = map.grid;

            Delay.delays.Clear();
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            foreach (Entity entity in entities)
                entity.LoadContent(graphicsDevice);
        }

        public override void UnloadContent()
        {
            if (unloading) return; // just incase this is called twice
            unloading = true;

            foreach (Entity entity in entities)
                entity.Dispose();

            entities.Clear();
        }

        public override void Update(float deltaTime)
        {
            for (int i = Delay.delays.Count-1; i >= 0; i--)
            {
                Delay.delays[i].Update(deltaTime);
            }

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Update(deltaTime);
            }

            camera.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Draw(spriteBatch);
            }

            /*for (int x = 0; x < gridSize.X; x++)
            {
                for (int y = 0; y < gridSize.Y; y++)
                {
                    switch(tiles[x,y])
                    {
                        case 1:
                            spriteBatch.Draw(texture, new Vector2(x * tileSize, y * tileSize), Color.White);
                            break;
                        case 2:
                            spriteBatch.Draw(texture, new Vector2(x * tileSize, y * tileSize), Color.Blue);
                            break;
                        default:
                            break;
                    }
                }
            }*/

            //spriteBatch.Draw(texture, rect, Color.White);
            if (map != null)
                map.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
