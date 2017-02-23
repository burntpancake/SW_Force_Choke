using System;
using XRL.Core;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts.Effects
{
    [Serializable]
    public class ZD_MaintainForce : Effect
    {
        public ZD_MaintainForce()
        {
            this.Duration = 1;
            this.DisplayName = "maintain force choke";
        }

        public override string GetDescription()
        {
            return "maintain force choke";
        }

        public override bool Apply(GameObject Object)
        {
            return true;
        }

        public override void Remove(GameObject Object)
        {
        }

        public override void Register(GameObject Object)
        {
            //Object.RegisterEffectEvent((Effect)this, "DealingMissileDamage");
            //Object.RegisterEffectEvent((Effect)this, "UsingEnergy");
        }

        public override void Unregister(GameObject Object)
        {
            //Object.UnregisterEffectEvent((Effect)this, "DealingMissileDamage");
            //Object.UnregisterEffectEvent((Effect)this, "UsingEnergy");
        }

        public override bool Render(Cell.RenderEvent E)
        {            
            return true;
        }

        public override bool FireEvent(Event E)
        {
            /*
            if (E.ID == "DealingMissileDamage")
            {
                if (this.Object.IsPlayer())
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
                if (this.Object.IsPlayer())
                    Popup.Show("You used energy!", true);
                string eParameter = e.GetParameter("Type") as string;
                if (eParameter.Contains("Mental") && !eParameter.Contains("Maintain") && !eParameter.Contains("ForceChoke"))
                {
                    if (this.Object.IsPlayer())
                        Popup.Show("You cannot use mental powers while maintaining Force choke!", true);
                    return false;
                }
            }*/
            return true;
        }
    }
}