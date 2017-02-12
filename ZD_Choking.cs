using System;
using XRL.Core;
using XRL.Messages;
using XRL.Rules;

namespace XRL.World.Parts.Effects
{
    [Serializable]
    public class ZD_Choking : Effect
    {
        public string BaseDamage;
        public string BonusDamage;
        public int SaveTarget;
        public GameObject Drainer;
        public int Level;
        public int SaveDifficulty;
        public int bonusDamageMutiplier = 0;
        public int range;

        public ZD_Choking()
        {
            this.BaseDamage = "1";
            this.SaveTarget = 20;
        }

        public ZD_Choking(int _Level, string BaseDamagePerRound, string BonusDamagePerRound, GameObject _Drainer, int _SaveDifficulty, int _range)
        {
            this.SaveTarget = 20;
            this.DisplayName = "force choking";
            this.BaseDamage = BaseDamagePerRound;
            this.BonusDamage = BonusDamagePerRound;
            this.Duration = 1;//Force choking is always maintained until break, no fixed duration
            this.Drainer = _Drainer;
            this.Level = _Level;
            SaveDifficulty = _SaveDifficulty;
            range = _range;
        }

        public override bool Apply(GameObject Object)
        {
            if (!Object.FireEvent(Event.New("ApplyLifeDrain")))
                return false;
            if (Object.IsPlayer())
                MessageQueue.AddPlayerMessage("&r" + this.Drainer.The + this.Drainer.ShortDisplayName + " is choking you using the Force!");
            else if (this.Drainer.IsPlayer())
                MessageQueue.AddPlayerMessage("&gYou begin to choke " + Object.The + Object.DisplayName + " with the Force!");
            return true;
        }

        public override void Remove(GameObject Object)
        {
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterEffectEvent((Effect)this, "EndTurn");
        }

        public override void Unregister(GameObject Object)
        {
            Object.UnregisterEffectEvent((Effect)this, "EndTurn");
        }

        public override bool Render(Cell.RenderEvent E)
        {
            int num = XRLCore.CurrentFrame % 60;
            if (num > 25 && num < 35)
            {
                E.Tile = (string)null;
                E.AddParameter("RenderString", string.Empty + (object)'\x0003');
                E.AddParameter("ColorString", "&K^k");
            }
            return true;
        }

        public int GetToughnessDefense(GameObject GO)
        {
            int num = 3;
            if (GO.Statistics.ContainsKey("Toughness"))
                num += GO.Statistics["Toughness"].Modifier;
            return num;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "EndTurn")
            {
                if ((this.Drainer.GetPart("Physics") as Physics).CurrentCell.IsGraveyard())
                    this.Duration = 0;
                //Check range
                if ((this.Object.GetPart("Physics") as Physics).CurrentCell.DistanceTo((Drainer.GetPart("Physics") as Physics).CurrentCell) > range)
                    Duration = 0;

                if (this.Duration > 0)
                {
                    if (Stat.Roll("1d8") + Math.Max(this.Drainer.Statistics["Ego"].Modifier, this.Level) + SaveDifficulty > GetToughnessDefense(this.Object) + 2 + (bonusDamageMutiplier * bonusDamageMutiplier)/2)
                    {
                        XRL.World.Damage damage = new XRL.World.Damage(Stat.Roll(BaseDamage) + bonusDamageMutiplier * Stat.Roll(BonusDamage));
                        bonusDamageMutiplier++;
                        damage.AddAttribute("Physical");
                        if (damage.Amount > 0)
                        {
                            Event E1 = Event.New("TakeDamage");
                            E1.AddParameter("Damage", (object)damage);
                            E1.AddParameter("Owner", (object)this.Drainer);
                            E1.AddParameter("Attacker", (object)this.Drainer);
                            E1.AddParameter("Message", "when choking by the Force!");
                            if (this.Object.FireEvent(E1))
                                this.Drainer.Statistics["Hitpoints"].Penalty -= damage.Amount;
                        }
                    }
                    else
                    {
                        this.Duration = 0;
                        if (this.Drainer.IsPlayer())
                            MessageQueue.AddPlayerMessage(this.Object.The + this.Object.DisplayName + " breaks free from your Force choke!");
                        else if (this.Object.IsPlayer())
                            MessageQueue.AddPlayerMessage("You break free from Force choke!");
                    }                   
                }
            }
            return true;
        }
    }
}
