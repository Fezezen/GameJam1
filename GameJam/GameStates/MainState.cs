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
        public int tileSize = 64;
        public Point gridSize;

        public Texture2D texture;
        public Rectangle rect;

        public override void Initalize()
        {
            entities = new List<Entity>
            {
                new Player(new Vector2(64,64))
            };

            gridSize = new Point(16, 9);
            tiles = new int[gridSize.X,gridSize.Y];
            for (int i = 0; i < 16; i++)
                tiles[i, 8] = 1;

            tiles[6, 7] = 1;
            tiles[6, 4] = 1;

            tiles[15, 7] = 1;
            tiles[15, 6] = 1;
            tiles[15, 5] = 1;
            tiles[15, 4] = 1;

            rect = new Rectangle(0, 300, 500, 3000);
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

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (tiles[x,y] == 1)
                    {
                        spriteBatch.Draw(texture, new Vector2(x * tileSize, y * tileSize), Color.White);
                    }
                }
            }

            //spriteBatch.Draw(texture, rect, Color.White);

            spriteBatch.End();
        }
    }
}
