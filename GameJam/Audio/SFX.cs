using Microsoft.Xna.Framework.Audio;

namespace GameJam.Audio
{
    class SFX
    {
        SoundEffect soundEffect;
        public float volume;

        public SFX(string name)
        {
            soundEffect = Program.Engine.Content.Load<SoundEffect>(name);
            volume = 1;
        }

        public void Play()
        {
            SoundEffectInstance instance = soundEffect.CreateInstance();

            instance.Volume = volume;
            instance.Play();

            void DisposeInstance()
            {
                instance.Dispose();
            }

            Delay delay = new Delay((float)soundEffect.Duration.TotalSeconds, DisposeInstance);
        }
    }
}
