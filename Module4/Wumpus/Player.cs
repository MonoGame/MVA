using System;

namespace Wumpus
{
    class Player
    {
        public int Health { get; private set; }

        public bool HasWeapon { get; set; }

        public bool IsDead 
        {
            get { return Health <= 0; }
        }

        public Player()
        {
            Health = 3;
        }

        public void Damage()
        {
            Health -= 1;
        }

        public void Kill()
        {
            Health = 0;
        }
    }
}
