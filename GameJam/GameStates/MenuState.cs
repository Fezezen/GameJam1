using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam.GameStates
{
    class MenuState : GameState
    {
        List<Button> buttons;
        Texture2D background;
        Texture2D backgroundFar;

        Point bkPos;

        public override void Initalize()
        {
            Program.Engine.IsMouseVisible = true;
            buttons = new List<Button>
            {
                new Button("Menu/Play", Program.Engine.GraphicsDevice.Viewport.Width / 2, 150, true, Play)
            };

            bkPos = new Point();
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            background = Program.Engine.Content.Load<Texture2D>("Background");
            backgroundFar = Program.Engine.Content.Load<Texture2D>("BackgroundFar");
        }

        public override void UnloadContent()
        {
            foreach (Button button in buttons)
                button.texture.Dispose();
        }

        public override void Update(float deltaTime)
        {
            foreach (Button button in buttons)
            {
                button.hover = button.rect.Contains(InputManager.GetMousePos());

                if (button.hover && InputManager.MouseClick(1))
                {
                    button.callback();
                }
            }

            bkPos.X += 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            spriteBatch.Draw(backgroundFar, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Matrix.CreateTranslation(new Vector3(-bkPos.X, 0, 0)));
            spriteBatch.Draw(background, bkPos.ToVector2(), new Rectangle(bkPos, new Point(Program.Engine.GraphicsDevice.Viewport.Width, Program.Engine.GraphicsDevice.Viewport.Height)), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();

            foreach (Button button in buttons)
                if (button.hover)
                    spriteBatch.Draw(button.texture, button.rect, Color.LightGray);
                else
                    spriteBatch.Draw(button.texture, button.rect, Color.White);

            spriteBatch.End();
        }

        public void Play()
        {
            Program.Engine.ChangeGameState("MainState");
        }
    }

    class Button
    {
        public Button(string dir, int x, int y, bool centered, Action callback)
        {
            texture = Program.Engine.Content.Load<Texture2D>(dir);

            if (centered)
                rect = new Rectangle(x - texture.Width/2, y - texture.Height / 2, texture.Width, texture.Height);
            else
                rect = new Rectangle(x, y, texture.Width, texture.Height);

            hover = false;
            this.callback = callback;
        }

        public Texture2D texture;
        public Rectangle rect;
        public bool hover;
        public Action callback;
    }
}
