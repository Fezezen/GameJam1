using GameJam.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GameJam.Objects
{
    public class Player : Entity
    {
        private new static readonly Vector2 size = new Vector2(19, 50);
        private readonly Vector2 startPosition;

        private readonly float maxSpeed = 200f; // max speed the player moves at
        private readonly float accel = 1600; // acceleration
        private readonly float friction = 1300f; // friction applied to player
        private readonly float airScaler = .65f;
        private readonly float airSpeedDebuff = 160f; //when the player goes faster than this speed, it will apply the airScalar debuff to air friction instead of the normal scalar
        private readonly float airScalarDebuff = .4f;
        private readonly float jumpForce = 400f; // the force applied when jumping
        
        private readonly float coyoteTimeAmount = .1f;
        private float coyoteTime = 0f; // if the player presses the jump button only like 2 or 3 frames after they start falling they may feel like the game is broken. So we let them jump a little after
        private readonly float jumpBufferAmount = .1f;
        private float jumpBuffer = 0f; // if the player presses the jump button only a couple frames before they land, we still let them jump.

        bool isDead = false;
        bool isCollecting = false;

        private Texture2D texture;
        private float moveX = 0;
        private bool flip = false;

        private SFX jumpSound;
        private SFX deathSound;
        private SFX collectIntrument;
        private SFX levelCompleteSFX;

        public Player(Vector2 _position) : base(_position, size, true)
        {
            position -= Vector2.UnitY * size.Y;
            startPosition = position;
            collisionCallbacks.Add(3, SpikeHit);
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            texture = Program.Engine.Content.Load<Texture2D>("Player/Player");

            jumpSound = new SFX("Sounds/Player/jump")
            {
                volume = 0.1f
            };
            deathSound = new SFX("Sounds/Player/death")
            {
                volume = 0.2f
            };
            collectIntrument = new SFX("Sounds/collecting_instruments_sound")
            {
                volume = 0.2f
            };
            levelCompleteSFX = new SFX("Sounds/level_completed_sound")
            {
                volume = 0.2f
            };
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
            if (gameState.instrument.rectangle.Contains((position + size / 2).ToPoint()))
                GrabInstrument();

            if (isCollecting)
                return;

            HandleInput(deltaTime);

            bool wasGrounded = isGrounded;

            float mul = isGrounded ? 1 : ((Math.Abs(velocity.X) < airSpeedDebuff) ? airScaler : airScalarDebuff);

            if (Math.Abs(velocity.X) > maxSpeed && Math.Sign(velocity.X) == moveX)
                velocity.X = Approach(velocity.X, maxSpeed * moveX, friction * deltaTime * mul);
            else
                velocity.X = Approach(velocity.X, maxSpeed * moveX, accel * deltaTime * mul);

            if (position.Y > gameState.mapRect.Bottom)
            {
                Died();
            } 

            base.Update(deltaTime);

            if (position.X < 0)
                position.X = 0;
            else if (position.X + size.X > gameState.mapRect.Right)
                position.X = gameState.mapRect.Right - size.X;

            if (isGrounded)
            {
                coyoteTime = coyoteTimeAmount;

                if (jumpBuffer > 0)
                    Jump();

                jumpBuffer = 0f;
            }
            else
            {
                coyoteTime -= deltaTime;
                jumpBuffer -= deltaTime;
            }
        }

        private void HandleInput(float deltaTime)
        {
            if (InputManager.IsKeyDown(Keys.Left))
            {
                moveX = -1;
                flip = true;
            }
            else if (InputManager.IsKeyDown(Keys.Right))
            {
                moveX = 1;
                flip = false;
            }
            else
                moveX = 0;

            if (InputManager.KeyPushed(Keys.Z) && coyoteTime > 0f)
                Jump();
            else if (!isGrounded && InputManager.KeyPushed(Keys.Z))
                jumpBuffer = jumpBufferAmount;
        }

        private void GrabInstrument()
        {
            if (isCollecting) return;

            gameState.entities.Remove(gameState.instrument);
            gameState.instrument.Dispose();

            isCollecting = true;

            collectIntrument.Play();

            new Delay(1, FinishLevel);
        }

        private void FinishLevel()
        {
            levelCompleteSFX.Play();
            gameState.musicInstance.Stop();

            new Delay(2.5f, SwapLevels);
        }

        private void SwapLevels()
        {
            if (gameState.isFinale)
            {
                Program.Engine.ChangeGameState("MenuState");
            }
            else
                Program.Engine.ChangeGameState("MainState", gameState.nextLevel);
        }

        public void Jump()
        {
            isGrounded = false;
            position.Y--; // avoids weird bug, ik it's not a good fix
            velocity.Y = -jumpForce;
            coyoteTime = 0;
            jumpSound.Play();
        }

        public void SpikeHit(Vector2 normal, Vector2 pos, float time)
        {
            if (normal.Y == - 1)
                Died();
            else
                velocity += normal * new Vector2(Math.Abs(velocity.X), Math.Abs(velocity.Y)) * (1.0f - time);
        }

        private void Died()
        {
            if (isDead) return;
            isDead = true;

            velocity = Vector2.Zero;
            frozen = true;

            deathSound.Play();

            gameState.musicInstance.Pause();

            new Delay(3, Respawn);
        }

        private void Respawn()
        {
            isDead = false;
            frozen = false;
            position = startPosition;
            gameState.camera.position = Vector2.Zero;
            gameState.camera.target = Vector2.Zero;

            gameState.musicInstance.Resume();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isDead)
                spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, 1, (flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0.0f);
                //spriteBatch.Draw(texture, position, Color.White);
        }

        public override void Dispose()
        {
        }
    }
}
