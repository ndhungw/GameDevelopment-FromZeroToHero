using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game_System
{
    public class Wizard : GameCharacter
    {
        public float Cast01SkillDuration { get; protected set; }
        public float Cast01SkillMultiplier { get; protected set; }
        public float Cast01SkillCooldown { get; protected set; }

        public Wizard()
        {
            Speed = 15;
            JumpSpeed = 45;
            MaxHealth = 40;
            CurrentHealth = MaxHealth;
            BaseDamage = 20;
            Cast01SkillDuration = 3.5f;
            Cast01SkillMultiplier = 1.05f;
            Cast01SkillCooldown = 7f;

            WeaponName = "Elder Staff";
            CharacterName = "Wizard";
            WeaponLevel = 1;
        }
    }
}
