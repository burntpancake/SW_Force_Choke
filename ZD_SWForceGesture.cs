using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class ZD_SWForceGesture : IPart
    {
        public int TurnCount;

        public ZD_SWForceGesture()
        {
            this.Name = "SWForceGesture";
        }

        public override bool SameAs(IPart p)
        {
            return false;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart)this, "Equipped");
            Object.RegisterPartEvent((IPart)this, "Unequipped");
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "Equipped")
            {                
                return true;
            }
            if (E.ID == "Unequipped")
            {
                return true;
            }                           
            return true;
        }
    }
}