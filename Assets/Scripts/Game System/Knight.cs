using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game_System
{
    public class Knight : GameCharacter
    {
        public float timeBetweenSwings { get; protected set; }
        public float defenseAgainstAttack { get; protected set; }
        public Knight()
        {
            Speed = 18;
            JumpSpeed = 40;
            MaxHealth = 100;
            BaseDamage = 50;
            timeBetweenSwings = 1.0f;
            defenseAgainstAttack = 0.2f;

            CharacterName = "Knight";
            WeaponName = "Long Sword";
        }
    }
}
