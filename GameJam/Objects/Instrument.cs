using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam.Objects
{
    public class Instrument : Entity
    {
        private Texture2D texture;
        private readonly string texName;
        public Rectangle rectangle;

        public Instrument(Vector2 position, Vector2 size, string dir) : base (position, size, false)
        {
            rectangle = new Rectangle(position.ToPoint(), size.ToPoint());
            texName = dir;
        }

        public override void Dispose()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
            rectangle.Size = texture.Bounds.Size;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            texture = Program.Engine.Content.Load<Texture2D>(texName);
        }
    }
}
