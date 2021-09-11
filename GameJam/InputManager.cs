using Microsoft.Xna.Framework.Input;

namespace GameJam
{
    static class InputManager
    {
        private static KeyboardState keyboardState;
        private static KeyboardState previousKeyboardState;

        public static void Initalize()
        {
            keyboardState = Keyboard.GetState();
            previousKeyboardState = keyboardState;
        }

        public static void Update()
        {
            keyboardState = Keyboard.GetState();
        }

        public static void LateUpdate()
        {
            previousKeyboardState = keyboardState;
        }

        public static bool IsKeyDown(Keys key)
        {
            /// <summary>Determines if a key is being pressed</summary>

            return keyboardState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            /// <summary>Determines if a key is not being pressed</summary>

            return keyboardState.IsKeyUp(key);
        }

        public static bool KeyPushed(Keys key) 
        {
            /// <summary>Determines if a key is being pushed but not held down</summary>
            return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }
    }
}
