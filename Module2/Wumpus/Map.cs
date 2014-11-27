using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Wumpus
{
    class Map
    {
        private readonly Room[] _rooms;

        public int Rows { get; private set; }

        public int Columns { get; private set; }

        public int MonsterRoom { get; private set; }

        public int WeaponRoom { get; private set; }

        public int PlayerRoomIndex { get; private set; }

        public Room PlayerRoom
        {
            get
            {
                return _rooms[PlayerRoomIndex];
            }        
        }

        public Room this[int index]
        {
            get
            {
                Debug.Assert(index > -1);
                Debug.Assert(index < _rooms.Length);
                return _rooms[index];
            }
        }

        public Map()
        {
            // Need a random to generate the map.
            var random = new Random(DateTime.Now.Millisecond);

            // Create the empty rooms.
            Rows = 10;
            Columns = 10;
            var lastRow = (Rows-1) * Columns;
            _rooms = new Room[Rows * Columns];
            for (var i = 0; i < _rooms.Length; i++)
            {
                var north = i < Columns ? -1 : i - Columns;
                var east = ((i+1)%Columns) == 0 ? -1 : i+1;
                var south = i >= lastRow ? -1 : i + Columns;
                var west = (i % Columns) == 0 ? -1 : i-1;
                var room = new Room(i, north, east, south, west);
                _rooms[i] = room;
            }

            // Pick some random spots for traps.
            var trapCount = Rows;
            for (var t = 0; t < trapCount; t++)
            {
                var i = random.Next(_rooms.Length);
                _rooms[i].HasTrap = true;
            }

            // Randomly place the weapon in a room without a trap.
            var traplessRooms = _rooms.Where(e => !e.HasTrap).ToArray();
            WeaponRoom = traplessRooms[random.Next(traplessRooms.Length)].Index;
          
            // Randomly place the monster in a room without traps or a weapon.
            var monsterRooms = _rooms.Where(e => !e.HasTrap && e.Index != WeaponRoom).ToArray();
            MonsterRoom = monsterRooms[random.Next(monsterRooms.Length)].Index;

            // Pick a random location for the player.
            var startRooms = _rooms.Where(e => !e.HasTrap && e.Index != WeaponRoom && e.Index != MonsterRoom).ToArray();
            PlayerRoomIndex = startRooms[random.Next(startRooms.Length)].Index;
        }

        public void MovePlayerNorth()
        {
            var room = PlayerRoom;
            if (room.NorthRoom != -1)
                PlayerRoomIndex = room.NorthRoom;
        }

        public void MovePlayerEast()
        {
            var room = PlayerRoom;
            if (room.EastRoom != -1)
                PlayerRoomIndex = room.EastRoom;
        }

        public void MovePlayerSouth()
        {
            var room = PlayerRoom;
            if (room.SouthRoom != -1)
                PlayerRoomIndex = room.SouthRoom;
        }

        public void MovePlayerWest()
        {
            var room = PlayerRoom;
            if (room.WestRoom != -1)
                PlayerRoomIndex = room.WestRoom;
        }
    }
}
