using System;
using GameJam.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam.Objects
{
    public class Camera
    {
        public Matrix Transform { get; private set; }
        public Vector2 position;
        public Vector2 target;
        public Vector2 size;
        private Player player;

        private MainState gameState;

        public Camera()
        {
            position = Vector2.Zero;
            size = new Vector2(Program.Engine.GraphicsDevice.Viewport.Width, Program.Engine.GraphicsDevice.Viewport.Height);
            gameState = (MainState)Program.Engine.gameState;
            player = (Player)gameState.entities.Find(p => p is Player);
        }

        public virtual void Update(float deltaTime)
        {
            Vector2 centerX = player.position + player.size / 2;
            centerX.Y = gameState.mapRect.Center.Y;
            Vector2 centerY = player.position + player.size / 2;
            centerY.X = gameState.mapRect.Center.X;

            Vector2 projectedX = centerX + Vector2.UnitX*size.X/2;
            Vector2 projectedXL = centerX - Vector2.UnitX * size.X/2;
            Vector2 projectedY = centerY + Vector2.UnitY * size.Y/2;
            Vector2 projectedYU = centerY - Vector2.UnitY * size.Y/2;

            if (gameState.mapRect.Contains(projectedX) && gameState.mapRect.Contains(projectedXL))
                target.X = centerX.X- size.X / 2;

            if (gameState.mapRect.Contains(projectedY) && gameState.mapRect.Contains(projectedYU))
                target.Y = centerY.Y - size.Y / 2;

            position = Vector2.Lerp(position, target, deltaTime * 10);

            Transform = Matrix.CreateTranslation(
            -position.X,
            -position.Y,
            0
            );
        }
    }
}
