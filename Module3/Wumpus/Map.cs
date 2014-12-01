using System;
using System.Diagnostics;
using System.Linq;

namespace Wumpus
{
    class Map
    {
        private Random _random;

        public delegate void OnPlayerMoved(int currentRoom, int newRoom);

        private readonly Room[] _rooms;

        public int Rows { get; private set; }

        public int Columns { get; private set; }

        public int AlienRoom { get; private set; }

        public int WeaponRoom { get; set; }

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

        public Map(int seed)
        {
            // Need a random to generate the map.
            _random = new Random(seed);

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
                var i = _random.Next(_rooms.Length);
                _rooms[i].HasTrap = true;
            }

            PlaceWeapon();
          
            // Randomly place the monster in a room without traps or a weapon.
            var monsterRooms = _rooms.Where(e => !e.HasTrap && e.Index != WeaponRoom).ToArray();
            AlienRoom = monsterRooms[_random.Next(monsterRooms.Length)].Index;

            // Pick a random location for the player.
            var startRooms = _rooms.Where(e => !e.HasTrap && e.Index != WeaponRoom && e.Index != AlienRoom).ToArray();
            PlayerRoomIndex = startRooms[_random.Next(startRooms.Length)].Index;
        }

        public void MovePlayerNorth(OnPlayerMoved callback)
        {
            var room = PlayerRoom;
            if (room.NorthRoom != -1)
            {
                callback(PlayerRoomIndex, room.NorthRoom);
                PlayerRoomIndex = room.NorthRoom;
            }
        }

        public void MovePlayerEast(OnPlayerMoved callback)
        {
            var room = PlayerRoom;
            if (room.EastRoom != -1)
            {
                callback(PlayerRoomIndex, room.EastRoom);
                PlayerRoomIndex = room.EastRoom;
            }
        }

        public void MovePlayerSouth(OnPlayerMoved callback)
        {
            var room = PlayerRoom;
            if (room.SouthRoom != -1)
            {
                callback(PlayerRoomIndex, room.SouthRoom);
                PlayerRoomIndex = room.SouthRoom;
            }
        }

        public void MovePlayerWest(OnPlayerMoved callback)
        {
            var room = PlayerRoom;
            if (room.WestRoom != -1)
            {
                callback(PlayerRoomIndex, room.WestRoom);
                PlayerRoomIndex = room.WestRoom;
            }
        }

        public bool IsTrapNear(int index)
        {
            var room = _rooms[index];

            if (room.NorthRoom != -1 && _rooms[room.NorthRoom].HasTrap)
                return true;
            if (room.EastRoom != -1 && _rooms[room.EastRoom].HasTrap)
                return true;
            if (room.SouthRoom != -1 && _rooms[room.SouthRoom].HasTrap)
                return true;
            if (room.WestRoom != -1 && _rooms[room.WestRoom].HasTrap)
                return true;

            return false;
        }

        public bool IsAlienNear(int index)
        {
            var room = _rooms[index];

            if (room.NorthRoom == AlienRoom)
                return true;
            if (room.EastRoom == AlienRoom)
                return true;
            if (room.SouthRoom == AlienRoom)
                return true;
            if (room.WestRoom == AlienRoom)
                return true;

            return false;
        }

        public void PlaceWeapon()
        {
            // Randomly place the weapon in a room without a trap, alien, or player.
            var traplessRooms = _rooms.Where(e => !e.HasTrap && e.Index != PlayerRoomIndex && e.Index != AlienRoom).ToArray();
            WeaponRoom = traplessRooms[_random.Next(traplessRooms.Length)].Index;
        }
    }
}
