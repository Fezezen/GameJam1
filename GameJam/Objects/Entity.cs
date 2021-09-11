using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameJam.Objects
{ 
    public abstract class Entity
    {
        /*
        This class handles all things that a base entity would need to handle, like position, size, gravity and collion handling.
        */
        public Vector2 position;
        public Vector2 size;
        public Vector2 velocity;
        protected Rectangle bounds;
        protected bool isGrounded;
        protected GameStates.MainState gameState;

        public bool gravityOn;

        public Entity(Vector2 _position, Vector2 _size, bool grav)
        {
            position = _position;
            size = _size;
            bounds = new Rectangle(position.ToPoint(), size.ToPoint());
            gravityOn = grav;

            gameState = (GameStates.MainState)Program.Engine.gameState;
        }

        public abstract void LoadContent(GraphicsDevice graphicsDevice);

        public virtual void Update(float deltaTime)
        {
            if (gravityOn)
                velocity += Vector2.UnitY * Program.Engine.gravity * deltaTime;

            position = HandleCollision(deltaTime);

            //position += velocity*deltaTime;
            bounds.Location = position.ToPoint();
        }

        protected Vector2 HandleCollision(float deltaTime)
        {
            if (velocity.X == 0 && velocity.Y == 0) // assuming that if there is no movement, then it can't possibly be intersecting with a rectangle
                return position;

            isGrounded = false;

            Vector2 projectedPosition = position + velocity * deltaTime;
            bounds.Location = projectedPosition.ToPoint();

            int leftTile = (int)Math.Min(projectedPosition.X, position.X) / gameState.tileSize;
            int rightTile = (int)(Math.Max(projectedPosition.X, position.X) + size.X) / gameState.tileSize;
            int topTile = (int)Math.Min(projectedPosition.Y, position.Y) / gameState.tileSize;
            int bottomTile = (int)(Math.Max(projectedPosition.Y, position.Y) + size.Y) / gameState.tileSize;

            List<Rectangle> tileRects = new List<Rectangle>();

            for (int y = Math.Max(topTile,0); y <= Math.Min(bottomTile,gameState.gridSize.Y - 1); y++)
            {
                for (int x = Math.Max(leftTile, 0); x <= Math.Min(rightTile, gameState.gridSize.X - 1); x++)
                {
                    if (gameState.tiles[x, y] == 1)
                    {
                        Rectangle rect = new Rectangle(new Point(x * gameState.tileSize, y * gameState.tileSize), new Point(gameState.tileSize, gameState.tileSize));
                        tileRects.Add(rect);
                    }
                }
            }

            List<Tuple<int, float>> potentiallyColliding = new List<Tuple<int, float>>();

            for (int i = 0; i < tileRects.Count; i++)
            {
                Vector2 point = Vector2.Zero;
                Vector2 normal = Vector2.Zero;
                float time = 0.0f;

                if (EntityRect(tileRects[i], ref point, ref normal, ref time, deltaTime))
                {
                    bool allow = true;
                    Point tPos = new Point(tileRects[i].X/gameState.tileSize, tileRects[i].Y / gameState.tileSize); // tile position of this tile
                    Point n = tPos + normal.ToPoint(); // tile the normal of the collision is pointing to

                    if (n.X > 0 && n.X < gameState.gridSize.X && n.Y > 0 && n.Y < gameState.gridSize.Y)
                    {
                        // sometimes collisions between multiple horitonally or vertically can result in collision response being applied when the entitiy should still move along the tiles
                        // so we do this check to see if the edge we've collided with isn't hidden (next to another collidable tile)
                        if (gameState.tiles[n.X, n.Y] > 0)
                            allow = false;
                    }

                    if (allow)
                    {
                        velocity += normal * new Vector2(Math.Abs(velocity.X), Math.Abs(velocity.Y)) * (1.0f - time);
                        if (normal.Y == -1) // if the normal is pointing up, it's the ground
                            isGrounded = true;
                    }
                }
            }

            return position+velocity*deltaTime;
        }

        private bool RayRect(Vector2 rayOrigin, Vector2 rayDir, Rectangle target, ref Vector2 collisionPoint, ref Vector2 collisionNormal, ref float hitNear) // this is a function for testing if a ray intersects with a rectangle
        {
            Vector2 invdir = Vector2.One / rayDir;

            Vector2 near = (target.Location.ToVector2() - rayOrigin) * invdir; // this is the near intersection time with the rectangle (really just the sides of the rectangles)
            Vector2 far = (target.Location.ToVector2() + target.Size.ToVector2() - rayOrigin) * invdir; // this is the far interestion time with the rectangle
            // This isn't literal "Time", it's the "Time" along the ray, where 0 is the start of the ray (origin) or 1, the end of the ray

            if (float.IsNaN(near.X) || float.IsNaN(near.Y)) return false; // sometimes results can return NAN and screw up the position of the entity.
            if (float.IsNaN(far.X) || float.IsNaN(far.Y)) return false;

            // if the near is farther than the far then that makes no sense, therefore we swap them around.
            if (near.X > far.X) {
                float t = far.X;
                far.X = near.X;
                near.X = t;
            }
            if (near.Y > far.Y)
            {
                float t = far.Y;
                far.Y = near.Y;
                near.Y = t;
            }

            if (near.X > far.Y || near.Y > far.X) return false; // no intersection

            hitNear = Math.Max(near.X, near.Y); // sort by which happened first, in the x or the y first
            float hitFar = Math.Min(far.X, far.Y); // same for here but the other way around

            if (hitFar < 0) return false; // the point is in the opposite direction, and therefore we aren't interested in the result

            collisionPoint = rayOrigin + hitNear * rayDir;

            if (near.X > near.Y)
                if (invdir.X < 0)
                    collisionNormal = Vector2.UnitX;
                else
                    collisionNormal = -Vector2.UnitX;
            else if (near.X < near.Y)
                if (invdir.Y < 0)
                    collisionNormal = Vector2.UnitY;
                else
                    collisionNormal = -Vector2.UnitY;

            return true;
        }

        private bool EntityRect(Rectangle target, ref Vector2 collisionPoint, ref Vector2 collisionNormal, ref float contactTime, float deltaTime)
        {
            Rectangle expandedTarget = new Rectangle(target.Location-(size/2).ToPoint(), target.Size + size.ToPoint());

            if (RayRect(position + size/2, velocity * deltaTime, expandedTarget, ref collisionPoint, ref collisionNormal, ref contactTime))
            {
                if (contactTime >= 0.0f && contactTime <= 1.0f)
                    return true;
            }

            return false;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Dispose();
    }
}
