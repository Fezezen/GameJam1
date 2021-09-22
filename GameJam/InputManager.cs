using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameJam
{
    static class InputManager
    {
        private static KeyboardState keyboardState;
        private static KeyboardState previousKeyboardState;
        private static MouseState mouseState;
        private static MouseState previousMouseState;

        public static void Initalize()
        {
            keyboardState = Keyboard.GetState();
            previousKeyboardState = keyboardState;
        }

        public static void Update()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }

        public static void LateUpdate()
        {
            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;
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

        public static Point GetMousePos()
        {
            return mouseState.Position;
        }

        public static bool MouseDown(int i)
        {
            switch (i)
            {
                case 1:
                    return mouseState.LeftButton == ButtonState.Pressed;
                case 2:
                    return mouseState.RightButton == ButtonState.Pressed;
                default:
                    break;
            }
            return false;
        }

        public static bool MouseDownPrev(int i)
        {
            switch (i)
            {
                case 1:
                    return previousMouseState.LeftButton == ButtonState.Pressed;
                case 2:
                    return previousMouseState.RightButton == ButtonState.Pressed;
                default:
                    break;
            }
            return false;
        }

        public static bool MouseClick(int i)
        {
            return MouseDown(i) && !MouseDownPrev(i);
        }
    }
}
