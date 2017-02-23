using System;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class ZD_SWForceGesture : IPart
    {
        public bool OnPlayer = false;

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
                if ((E.GetParameter("EquippingObject") as GameObject).IsPlayer())
                {
                    OnPlayer = true;
                    Popup.Show("You equipped the Force Gesture! Equipper is " + (E.GetParameter("EquippingObject") as GameObject).ToString(), true);
                }

                (E.GetParameter("EquippingObject") as GameObject).ApplyEffect(new XRL.World.Parts.Effects.ZD_MaintainForce());
                //(E.GetParameter("EquippingObject") as GameObject).RegisterPartEvent((IPart)this, "DealingMissileDamage");
                //(E.GetParameter("EquippingObject") as GameObject).RegisterPartEvent((IPart)this, "UsingEnergy");               
                return true;
            }

            if (!(E.ID == "Unequipped"))
            {
                if ((E.GetParameter("UnequippingObject") as GameObject).IsPlayer())
                {
                    OnPlayer = false;
                    Popup.Show("You unequipped the Force Gesture! Unequipper is " + (E.GetParameter("UnequippingObject") as GameObject).ToString(), true);
                }
                //(E.GetParameter("UnequippingObject") as GameObject).UnregisterPartEvent((IPart)this, "DealingMissileDamage");
                //(E.GetParameter("UnequippingObject") as GameObject).UnregisterPartEvent((IPart)this, "UsingEnergy");
                return true;
            }

            return true;
        }
    }
}