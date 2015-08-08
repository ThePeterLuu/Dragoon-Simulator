using DragoonSimulator.Entities;

namespace DragoonSimulator.FFXIVConcepts
{
    public class EffectSnapshot
    {
        public Actor Caster { get; set; }
        public Actor Target { get; set; }
        public long Duration { get; set; }
        public double Potency { get; set; }
        public double Multiplier { get; set; }
        public double Str { get; set; }
        public double WeaponDamage { get; set; }
        public double Det { get; set; }
        public double Crt { get; set; }
        public double CritChance { get; set; }
    }
}
