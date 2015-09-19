using System.Collections.Generic;

namespace DragoonSimulator.Entities
{
    public class RecordedDPS
    {
        public Player Player { get; set; }
        public double DPS { get; set; }
        public List<string> EquipmentNames { get; set; }
    }
}
