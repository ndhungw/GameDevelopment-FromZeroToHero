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
        public float launchArrowForce { get; private set; }
        public float critRate { get; private set; }
        public float critDamage { get; private set; }
        public float arrowRange { get; private set; }


        public Archer()
        {
            Speed = 21;
            JumpSpeed = 50;
            MaxHealth = 60;
            CurrentHealth = MaxHealth;
            BaseDamage = 35;
            timeBetweenShots = 0.5f;
            launchArrowForce = 300.0f;
            critRate = 20.0f;
            critDamage = 1.5f;
            arrowRange = 6.0f;

            CharacterName = "Archer";
            WeaponName = "Composite Bow";
            WeaponLevel = 1;

        }
    }
}
