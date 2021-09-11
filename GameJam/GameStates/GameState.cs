using Microsoft.Xna.Framework.Graphics;

namespace GameJam.GameStates
{
    public abstract class GameState
    {
        // This class is the abstract class for Game States.
        // Game states are states that the game is in such as the Menu, or Level select screen, or Overworld and alike.
        protected bool unloading = false;

        public abstract void Initalize();
        public abstract void LoadContent(GraphicsDevice graphicsDevice);
        public abstract void UnloadContent();
        public abstract void Update(float deltaTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
