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

        public override void Initalize()
        {
            entities = new List<Entity>
            {
                new Player(new Vector2(64,64))
            };

            gridSize = new Point(32, 18);
            tiles = new int[gridSize.X,gridSize.Y];
            for (int i = 0; i < gridSize.X; i++)
                tiles[i, gridSize.Y-1] = 1;

            tiles[6, 16] = 1;

            tiles[20, 16] = 1;
            tiles[20, 15] = 1;

            tiles[15, 15] = 2;
            tiles[16, 15] = 2;
            tiles[14, 15] = 2;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            foreach (Entity entity in entities)
                entity.LoadContent(graphicsDevice);

            texture = new Texture2D(graphicsDevice, tileSize, tileSize);
            Color[] data = new Color[tileSize * tileSize];
            for (int i = 0; i < tileSize * tileSize; i++)
            {
                int x = i % tileSize;
                int y = i / tileSize;
                if ((x == 0 || x == tileSize - 1) || (y == 0 || y == tileSize - 1))
                    data[i] = Color.Black;
                else
                    data[i] = Color.White;
            }
            
            texture.SetData(data);
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
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Update(deltaTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Draw(spriteBatch);
            }

            for (int x = 0; x < gridSize.X; x++)
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
            }

            //spriteBatch.Draw(texture, rect, Color.White);

            spriteBatch.End();
        }
    }
}
