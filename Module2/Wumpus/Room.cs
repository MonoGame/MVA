using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus
{
    //[DebuggerDisplay("R{Index} Trap:{HasTrap}")]
    class Room
    {
        public int Index { get; private set; }

        public int NorthRoom { get; private set; }

        public int EastRoom { get; private set; }

        public int SouthRoom { get; private set; }

        public int WestRoom { get; private set; }

        public bool HasTrap { get; set; }

        public Room(int index, int north, int east, int south, int west)
        {
            Index = index;
            NorthRoom = north;
            EastRoom = east;
            SouthRoom = south;
            WestRoom = west;
        }
    }
}
