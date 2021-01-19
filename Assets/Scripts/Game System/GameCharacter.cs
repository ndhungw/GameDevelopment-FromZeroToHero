using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game_System
{
    public class GameCharacter
    {
        public float Speed { get; protected set; }
        public float JumpSpeed { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int BaseDamage { get; protected set; }
        public int CurrentHealth { get; set; }
    }
}
