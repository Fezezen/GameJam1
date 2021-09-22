using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameJam.Objects;
using System.Collections.Generic;
using GameJam.GameStates;
using System;
using System.Linq;

namespace GameJam
{
    public class Engine : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public GameState gameState;

        public float gravity = 700.0f; // graivty all entities use.

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 576,
            };
            Content.RootDirectory = "Content";

            gameState = new MenuState();
        }

        protected override void Initialize()
        {
            InputManager.Initalize();

            if (gameState != null)
                gameState.Initalize(new object[0]);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (gameState != null)
                gameState.LoadContent(GraphicsDevice); // load current game state
        }

        protected override void UnloadContent()
        {
            if (gameState != null)
                gameState.UnloadContent();

            Content.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            InputManager.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameState != null)
                gameState.Update(deltaTime);

            base.Update(gameTime);
            InputManager.LateUpdate();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (gameState != null)
                gameState.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        public void ChangeGameState(string stateName, params object[] list) // swap between game states
        {
            var type = (from assembly in AppDomain.CurrentDomain.GetAssemblies() // determine if the state class exists
                        from t in assembly.GetTypes()
                        where t.Name == stateName 
                        select t).FirstOrDefault();

            if (type != null)
            {
                if (gameState != null) // dispose of last state. We don't need data leaks :)
                {
                    gameState.UnloadContent();
                }

                gameState = (GameState)Activator.CreateInstance(type);        
                gameState.Initalize(list);
                gameState.LoadContent(GraphicsDevice);
            }
        }
    }
}
