using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GameJam.Objects
{
    public class Player : Entity
    {
        private new static readonly Vector2 size = new Vector2(40, 40);

        private readonly float maxSpeed = 200f; // max speed the player moves at
        private readonly float accel = 1600; // acceleration
        private readonly float friction = 600f; // friction applied to player
        private readonly float airScaler = .65f;
        private readonly float airSpeedDebuff = 160f; //when the player goes faster than this speed, it will apply the airScalar debuff to air friction instead of the normal scalar
        private readonly float airScalarDebuff = .4f;
        private readonly float jumpForce = 400f; // the force applied when jumping

        private Texture2D texture;
        private float moveX = 0;

        public Player(Vector2 _position) : base(_position, size, true)
        {

        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            texture = new Texture2D(graphicsDevice, (int)size.X, (int)size.Y);
            Color[] color = new Color[texture.Width * texture.Height];

            for (int i = 0; i < texture.Width * texture.Height; i++)
                color[i] = Color.Red;

            texture.SetData(color);
        }

        private float Approach(float current, float goal, float a) // simulated acceleration
        {
            if (goal < 0)
            {
                if (current > goal)
                    return current -= a;
                else
                    return current += a;
            }
            else if (goal > 0)
            {
                if (current < goal)
                    return current += a;
                else
                    return current -= a;
            }
            else
            {
                if (current > 0)
                    return Math.Max(current -= a, 0);
                else if (current < 0)
                    return Math.Min(current += a, 0);
            }

            return current;
        }

        public override void Update(float deltaTime)
        {
            HandleInput(deltaTime);

            float mul = isGrounded ? 1 : ((Math.Abs(velocity.X) < airSpeedDebuff) ? airScaler : airScalarDebuff);

            if (Math.Abs(velocity.X) > maxSpeed && Math.Sign(velocity.X) == moveX)
                velocity.X = Approach(velocity.X, maxSpeed * moveX, friction * deltaTime * mul);
            else
                velocity.X = Approach(velocity.X, maxSpeed * moveX, accel * deltaTime * mul);

            base.Update(deltaTime);
        }

        private void HandleInput(float deltaTime)
        {
            if (InputManager.IsKeyDown(Keys.Left))
                moveX = -1;
            else if (InputManager.IsKeyDown(Keys.Right))
                moveX = 1;
            else
                moveX = 0;

            if (InputManager.KeyPushed(Keys.Z) && isGrounded)
                Jump();
        }

        public void Jump()
        {
            isGrounded = false;
            position.Y--; // avoids weird bug, ik it's not a good fix
            velocity.Y -= jumpForce;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public override void Dispose()
        {
            texture.Dispose();
        }
    }
}
