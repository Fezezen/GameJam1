using GameJam.Objects;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace GameJam.GameStates
{
    public class MainState : GameState // This is the game state that handles the main part of the game where the player has control of their character.
    {       
        public List<Entity> entities;
        public int[,] tiles;
        public int tileSize = 32;
        public Point gridSize;

        Texture2D background;
        Texture2D backgroundFar;

        Map map;
        public Camera camera;
        public Rectangle mapRect;

        private SoundEffect music;
        public SoundEffectInstance musicInstance;

        public Instrument instrument;
        public bool isFinale;
        public string nextLevel;

        public override void Initalize(object[] list)
        {
            map = TiledLoader.LoadMap((string)list[0]);

            instrument = new Instrument(map.instruPos, new Vector2(30,30), "Instruments/" + map.instrument);

            entities = new List<Entity>
            {
                new Player(new Vector2(map.playerX*map.tilesize,map.playerY*map.tilesize)),
                instrument
            };

            gridSize = new Point(map.width,map.height);
            tiles = new int[gridSize.X,gridSize.Y];
            mapRect = new Rectangle(new Point(), new Point(gridSize.X * map.tilesize, gridSize.Y * map.tilesize));

            camera = new Camera();

            background = Program.Engine.Content.Load<Texture2D>("Background");
            backgroundFar = Program.Engine.Content.Load<Texture2D>("BackgroundFar");

            if (map != null)
                tiles = map.grid;

            music = Program.Engine.Content.Load<SoundEffect>("Sounds/Music/" + map.music);
            musicInstance = music.CreateInstance();
            musicInstance.Volume = .05f;
            musicInstance.IsLooped = true;
            musicInstance.Play();

            isFinale = map.isFinale;
            nextLevel = map.nextLevel;

            Delay.delays.Clear(); // Make sure no delays fire in the new state.
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            foreach (Entity entity in entities)
                entity.LoadContent(graphicsDevice);
        }

        public override void UnloadContent()
        {
            if (unloading) return; // just incase this is called twice
            unloading = true;

            foreach (Entity entity in entities)
                entity.Dispose();

            entities.Clear();

            //background.Dispose();
            //backgroundFar.Dispose();
            musicInstance.Dispose();
            Delay.delays.Clear();
        }

        public override void Update(float deltaTime)
        {
            for (int i = Delay.delays.Count-1; i >= 0; i--)
            {
                Delay.delays[i].Update(deltaTime);
            }

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Update(deltaTime);
            }

            camera.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            spriteBatch.Draw(backgroundFar, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, camera.Transform);
            spriteBatch.Draw(background, camera.position, new Rectangle((camera.position*.1f).ToPoint(), camera.size.ToPoint()), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Draw(spriteBatch);
            }

            if (map != null)
                map.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
