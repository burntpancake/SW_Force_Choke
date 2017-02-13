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
            Object.RegisterPartEvent((IPart)this, "DealingMissileDamage");
            Object.RegisterPartEvent((IPart)this, "UsingEnergy");
        }

        public override bool FireEvent(Event E)
        {
            
            if (E.ID == "DealingMissileDamage")
            {
                if (OnPlayer)
                    Popup.Show("You cannot deal missile attacks while maintaining Force choke!", true);
                return false;
            }

            if (E.ID == "UsingEnergy")
            {
                
                Event e = E.GetParameter("Event") as Event;
                if (!e.HasParameter("Type"))
                {
                    return true; //If someone implement am ability without tag it a type, it shall get pass.
                }
                if (OnPlayer)
                    Popup.Show("You used energy!", true);
                string eParameter = e.GetParameter("Type") as string;
                if (eParameter.Contains("Mental") && !eParameter.Contains("Maintain") && !eParameter.Contains("ForceChoke"))
                {
                    if (OnPlayer)
                        Popup.Show("You cannot use mental powers while maintaining Force choke!", true);
                    return false;
                }
            }

            if (E.ID == "Equipped")
            {
                if ((E.GetParameter("EquippingObject") as GameObject).IsPlayer())
                {
                    OnPlayer = true;
                    Popup.Show("You equipped the Force Gesture!" + (E.GetParameter("EquippingObject") as GameObject).ToString(), true);
                }
                    
                //(E.GetParameter("EquippingObject") as GameObject).RegisterPartEvent((IPart)this, "DealingMissileDamage");
                //(E.GetParameter("EquippingObject") as GameObject).RegisterPartEvent((IPart)this, "UsingEnergy");               
                return true;
            }

            if (!(E.ID == "Unequipped"))
            {
                if ((E.GetParameter("EquippingObject") as GameObject).IsPlayer())
                {
                    OnPlayer = false;
                }
                (E.GetParameter("UnequippingObject") as GameObject).UnregisterPartEvent((IPart)this, "DealingMissileDamage");
                (E.GetParameter("UnequippingObject") as GameObject).UnregisterPartEvent((IPart)this, "UsingEnergy");
                return true;
            }

            return true;
        }
    }
}