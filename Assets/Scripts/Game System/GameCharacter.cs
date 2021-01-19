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

        public int CurrentHealth { get; protected set; }
        public int BaseDamage { get; protected set; }

        //UI based properties
        public String CharacterName { get; protected set; }
        public String WeaponName { get; protected set; }

        public int WeaponLevel { get; protected set; }

        public void SetWeaponLevel(int level)
        {
            if (level > 0)
            {
                WeaponLevel = level;
            }

            BaseDamage += (int) BaseDamage * (level - 1) * 3 / 100;
        }

        public void SetCurrentHealth(int health)
        {
            if (health >= 0)
            {
                CurrentHealth = health;
            }
            
        }
    }
}
