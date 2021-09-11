using GameJam.Objects;
using System;
using System.Collections.Generic;

/*
Team members:
Jaxson
Johnathan
Kajananan
Nathan
William
Yash
Volodymyr
*/

namespace GameJam
{
#if WINDOWS || LINUX
    public static class Program
    {
        public static Engine Engine;
        [STAThread]

        static void Main()
        {
            Engine = new Engine();

            using (Engine)
                Engine.Run();
        }
    }
#endif
}
