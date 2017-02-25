using System;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class ZD_SWForceGesture : IPart
    {
        public bool OnPlayer = false;
        public GameObject Wearer;

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

        private bool EndSignalSent = false;
        public override bool FireEvent(Event E)
        {            

            if (E.ID == "UsingEnergy" && !EndSignalSent)
            {
                
                Event e = E.GetParameter("Event") as Event;
                if (!e.HasParameter("Type"))
                {
                    return true; //If someone implement am ability without tag it a type, it shall get pass.
                }
                //if (OnPlayer)
                //    Popup.Show("You used energy!", true);
                string eParameter = e.GetParameter("Type") as string;
                if (eParameter.Contains("Mental") && !eParameter.Contains("Maintain") && !eParameter.Contains("ForceChoke"))
                {
                    EndSignalSent = true;
                    if (OnPlayer)
                        Popup.Show("You no longer maintain force choke as your mind focus on other mental powers.", true);
                    if (Wearer != null)
                        Wearer.FireEvent(Event.New("CommandStopSWForceChoke"));
                    return true;
                }
                else if (eParameter.Contains("Missile"))
                {
                    EndSignalSent = true;
                    if (OnPlayer)
                        Popup.Show("You no longer maintain force choke as your mind focus on missile attacks.", true);
                    if (Wearer != null)
                        Wearer.FireEvent(Event.New("CommandStopSWForceChoke"));
                    return true;
                }
            }

            if (E.ID == "Equipped")
            {
                if ((E.GetParameter("EquippingObject") as GameObject).IsPlayer())
                {
                    OnPlayer = true;
                    Popup.Show("You equipped the Force Gesture!" + (E.GetParameter("EquippingObject") as GameObject).ToString(), true);
                }
                Wearer = (E.GetParameter("EquippingObject") as GameObject);

                Wearer.RegisterPartEvent((IPart)this, "UsingEnergy");               
                return true;
            }

            if ((E.ID == "Unequipped"))
            {
                if ((E.GetParameter("UnequippingObject") as GameObject).IsPlayer())
                {
                    OnPlayer = false;
                }

                Wearer.UnregisterPartEvent((IPart)this, "UsingEnergy");
                Wearer = null;
                               
                return true;
            }

            return true;
        }
    }
}