using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus
{
    class Player
    {
        public int Health { get; private set; }

        public bool HasWeapon { get; private set; }

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
