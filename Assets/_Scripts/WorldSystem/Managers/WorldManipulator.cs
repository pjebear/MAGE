using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldSystem
{
    class WorldManipulator
    {
        public static WorldManipulator Instance;

        public WorldManipulator()
        {
            Instance = this;
        }
    }
}
