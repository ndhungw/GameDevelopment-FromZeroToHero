using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game_System
{    
    public class Archer : GameCharacter
    {
        public float timeBetweenShots { get; private set; }
        public Archer()
        {
            Speed = 23;
            JumpSpeed = 50;
            MaxHealth = 60;
            BaseDamage = 35;
            timeBetweenShots = 0.5f;  
        }
    }
}
