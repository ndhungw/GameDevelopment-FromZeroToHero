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
            Speed = 22;
            JumpSpeed = 45;
            MaxHealth = 40;
            BaseDamage = 25;
            Cast01SkillDuration = 2.5f;
            Cast01SkillMultiplier = 0.6f;
            Cast01SkillCooldown = 8.0f;
        }
    }
}
