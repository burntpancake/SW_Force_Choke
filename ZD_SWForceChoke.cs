using ConsoleLib.Console;
using System;
using System.Collections.Generic;
using System.Threading;
using XRL.Core;
using XRL.Rules;
using XRL.UI;
using XRL.World.AI.GoalHandlers;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ZD_SWForceChoke : BaseMutation
    {
        public Guid SWForceChokeActivatedAbilityID = Guid.Empty;
        public ActivatedAbilityEntry SWForceChokeActivatedAbility = (ActivatedAbilityEntry)null;
        public int CooldownRound = 15;
        public GameObject ForceGestureObject;

        public string partName; //remembers which hand the force gesture is put on

        public ZD_SWForceChoke()
        {
            Name = "SWForceChoke";
            DisplayName = "Force Choke";
            this.Type = "Mental";
        }

        public override void Register(GameObject Object)
        {

            Object.RegisterPartEvent((IPart)this, "CommandSWForceChoke");
            Object.RegisterPartEvent((IPart)this, "AIGetOffensiveMutationList");
            Object.RegisterPartEvent((IPart)this, "BeginEquip");
        }

        public override string GetDescription()
        {
            return "You can use the Force to squeeze a victim's throat continuously, doing so requires one free hand. \n" 
                + "Your vitim may break free each turn by passing a toughness save, or getting out of range. \n"
                + "While maintaing the Force Choke gesture, you may NOT use another ability or missile weapons. \n";
        }

        public override string GetLevelText(int Level)
        {
            string Ret = "Employ the Force to choke a victim's throat, render them painful and 30% slower.\n";
            Ret += "Cooldown: " + GetCooldown(Level) + " rounds\n";
            Ret += "Base damage each turn: " + GetBaseDamage(Level) + "\n";
            Ret += "Extra damage for each consecutive turn: " + GetBonusDamage(Level) + "\n";
            Ret += "Range: " + GetRange(Level).ToString() + "\n";
            if(Level != this.Level)
            {
                Ret += "Increase the save difficulty of your Force Choke ";
                if (Level == 2 || Level == 6 || Level == 10)
                    Ret += "moderately.";
                else
                    Ret += "slightly.";
            }
            return Ret;
        }

        public string GetBaseDamage(int Level)
        {
            //return "1d" + ((Level - 1) / 4 + 3).ToString();
            if (Level == 1)
                return "1";
            else if (Level == 2)
                return "1d2";
            else if (Level == 3)
                return "1d3";
            else if (Level == 4)
                return "1d3";
            else if (Level == 5)
                return "1d4";
            else if (Level == 6)
                return "2d2";
            else if (Level == 7)
                return "2d2+1";
            else if (Level == 8)
                return "2d2+1";
            else if (Level == 9)
                return "2d3";
            else if (Level >= 11)
                return "2d3+" + ((Level - 9)/2).ToString();
            else 
                return "1";
        }

        public string GetBonusDamage(int Level)
        {
            //return "1d" + ((Level + 1) / 2).ToString();
            if (Level == 1)
                return "1d2";
            else if (Level == 2)
                return "1d2";
            else if (Level == 3)
                return "1d3";
            else if (Level == 4)
                return "1d3";
            else if (Level == 5)
                return "1d4";
            else if (Level == 6)
                return "2d2";
            else if (Level == 7)
                return "2d2+1";
            else if (Level >= 8)
                return "2d2+1";
            else if (Level == 9)
                return "3d2";
            else if (Level >= 10)
                return "3d2+" + ((Level - 8)/2).ToString();
            else
                return "1d3";
        }

        public int GetSaveBonus(int Level)
        {
            if (Level == 1)
                return 1;
            else if (Level == 2 || Level == 6 || Level == 10)
                return 2 + GetSaveBonus(Level - 1);
            else if (Level > 1)
                return 1 + GetSaveBonus(Level - 1);

            return 0;
        }

        //This is the maximum turn you can maintain force choke as well.
        public int GetRange(int Level)
        {
            if(Level < 4)
            {
                return 3;
            }
            else if(Level < 7)
            {
                return 4;
            }
            else if(Level >= 7)
            {
                return 5;
            }

            return 3;
        }

        public int GetCooldown(int Level)
        {
            if (Level <= 10)
            {
                return CooldownRound - Level/2;
            }
            else if (Level > 10)
            {
                return CooldownRound - 5;
            }

            return CooldownRound;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Unmutate(GO);
            ActivatedAbilities part = GO.GetPart("ActivatedAbilities") as ActivatedAbilities;
            SWForceChokeActivatedAbilityID = part.AddAbility("Force Choke", "CommandSWForceChoke", "Mental Mutation", "#");
            SWForceChokeActivatedAbility = part.AbilityByGuid[SWForceChokeActivatedAbilityID];
            return true;
        }

        public override bool Unmutate(GameObject GO)
        {
            if (this.SWForceChokeActivatedAbilityID != Guid.Empty)
            {
                (GO.GetPart("ActivatedAbilities") as ActivatedAbilities).RemoveAbility(SWForceChokeActivatedAbilityID);
                SWForceChokeActivatedAbilityID = Guid.Empty;
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
        
        public void SW_Choke(Cell C)
        {          
            TextConsole textConsole = Look._TextConsole;
            ScreenBuffer scrapBuffer = TextConsole.ScrapBuffer;
            XRLCore.Core.RenderMapToBuffer(scrapBuffer);
            bool bDraw = C.IsVisible();

            int nBonus = Math.Max(ParentObject.Statistics["Ego"].Modifier, Level);
            if (C != null)
            {
                foreach (GameObject GO in C.GetObjectsInCell())
                {
                    if (GO.HasPart("Brain"))//Check if the creature can breathe
                    {
                        //Apply force choking effect to victim
                        GO.ApplyEffect((Effect)new XRL.World.Parts.Effects.ZD_Choking(Level, GetBaseDamage(Level), GetBonusDamage(Level), ParentObject, GetSaveBonus(Level), GetRange(Level)));
                        /*
                        int Pens = Stat.RollDamagePenetrations(GetToughnessDefense(GO), nBonus, nBonus);
                        int TotalDamage = 0;
                        for (int index = 0; index < Pens; ++index)
                            TotalDamage += Stat.Roll(GetBaseDamage(this.Level));
                        if (TotalDamage == 0)
                        {
                            if (this.ParentObject.IsPlayer())
                                XRLCore.Core.Game.Player.Messages.Add("&rYou fail to force " + GO.DisplayName + " to choke.");
                        }
                        else
                        {
                            Damage damage = new Damage(TotalDamage);
                            damage.AddAttribute("Physical");
                            string resultColor = Stat.GetResultColor(TotalDamage);
                            Event E = Event.New("TakeDamage");
                            E.AddParameter("Damage", (object)damage);
                            E.AddParameter("Owner", (object)this.ParentObject);
                            E.AddParameter("Attacker", (object)this.ParentObject);
                            if (!GO.FireEvent(E) || damage.Amount == 0)
                            {
                                if (GO.IsPlayer())
                                    XRLCore.Core.Game.Player.Messages.Add("&rThe " + this.ParentObject.DisplayName + " fails to force choke you.");
                                else if (this.ParentObject.IsPlayer())
                                    XRLCore.Core.Game.Player.Messages.Add("Your attack does not affect " + GO.the + GO.DisplayName + ".");
                            }
                            else if (GO.IsPlayer())
                                XRLCore.Core.Game.Player.Messages.Add("&rThe " + this.ParentObject.DisplayName + " force choke you " + resultColor + "(x" + TotalDamage.ToString() + ")&y for " + damage.Amount.ToString() + " damage!");
                            else if (this.ParentObject.IsPlayer())
                                XRLCore.Core.Game.Player.Messages.Add("&gYou force choke " + GO.DisplayName + resultColor + "(x" + TotalDamage.ToString() + ")&y for " + damage.Amount.ToString() + " damage!");
                            if (bDraw)
                            {
                                for (int index1 = 0; index1 < 4; ++index1)
                                {
                                    for (int index2 = 0; index2 < 5; ++index2)
                                        GO.ParticleText("&B" + (object)(char)(219 + Stat.Random(0, 4)), 4.9f, 5);
                                    for (int index2 = 0; index2 < 5; ++index2)
                                        GO.ParticleText("&b" + (object)(char)(219 + Stat.Random(0, 4)), 4.9f, 5);
                                    for (int index2 = 0; index2 < 5; ++index2)
                                        GO.ParticleText("&W" + (object)(char)(219 + Stat.Random(0, 4)), 4.9f, 5);
                                }
                            }
                            
                            
                        }*/
                    }
                }
            }
            scrapBuffer.Goto(C.X, C.Y);
            scrapBuffer.Write("&Y#");
            if (bDraw)
                textConsole.DrawBuffer(scrapBuffer, (IScreenBufferExtra)null, false);
            if (!bDraw)
                return;
            Thread.Sleep(50);
        }

        public void EquipForceGesture()
        {
            if (ParentObject == null)
                return;
            if (ForceGestureObject == null)
                ForceGestureObject = GameObjectFactory.Factory.CreateObject("ZD_SWForceGesture");
            Armor part = ForceGestureObject.GetPart("Armor") as Armor;
            part.WornOn = partName;
            (ForceGestureObject.GetPart("Render") as Render).DisplayName = this.DisplayName;
            Event E = Event.New("CommandForceEquipObject");
            E.AddParameter("Object", (object)ForceGestureObject);
            E.AddParameter("BodyPartName", partName);
            this.ParentObject.FireEvent(E);
        }

        public void UnequipForceGesture()
        {
            ParentObject.FireEvent(Event.New("CommandForceUnequipObject", "BodyPartName", partName));
        }

        private bool AllowFullHand = true;
        public override bool FireEvent(Event E)
        {
            
            if (E.ID == "AIGetOffensiveMutationList")
            {
                int parameter1 = (int)E.GetParameter("Distance");
                GameObject parameter2 = E.GetParameter("Target") as GameObject;
                List<AICommandList> parameter3 = (List<AICommandList>)E.GetParameter("List");
                if (this.SWForceChokeActivatedAbility != null && this.SWForceChokeActivatedAbility.Cooldown <= 0 && parameter1 <= this.GetRange(this.Level) && this.ParentObject.HasLOSTo(parameter2))
                    parameter3.Add(new AICommandList("CommandSWForceChoke", 1));
                return true;
            }
            if (E.ID == "CommandSWForceChoke")
            {
                Cell C = this.PickDestinationCell(this.GetRange(this.Level), AllowVis.OnlyVisible, true);
                if (C == null)
                    return true;
                //Check range
                if (C.DistanceTo((this.ParentObject.GetPart("Physics") as Physics).CurrentCell) > this.GetRange(this.Level))
                {
                    if (this.ParentObject.IsPlayer())
                        Popup.Show("That is out of range! (" + this.GetRange(this.Level) + " squares)", true);
                    return true;
                }
                //Check free hand
                Body body = this.ParentObject.GetPart("Body") as Body;
                bool HasEmptyHand = false;
                if (body != null)
                {
                    List<BodyPart> hands = body.GetPart("Hand");
                    
                    foreach(BodyPart hand in hands)
                    {
                        if(hand._Equipped == null)
                        {
                            HasEmptyHand = true;
                            partName = hand.Name;//Records the free hand
                            break;
                        }
                    }                    
                }
                if (!HasEmptyHand && !AllowFullHand)
                {
                    if (this.ParentObject.IsPlayer())
                        Popup.Show("You must have a free hand without any equipment to do a force gesture!", true);
                    return true;
                }
                    

                if (C != null)
                {
                    this.SWForceChokeActivatedAbility.Cooldown = this.GetCooldown(Level) * 10 + 10;
                    //Popup.Show("Energy before equip: " + this.ParentObject.Energy, true);
                    EquipForceGesture();
                    //Popup.Show("Energy after equip: " + this.ParentObject.Energy, true);
                    this.SW_Choke(C);
                    //Popup.Show("Energy after choke before energy usage: " + this.ParentObject.Energy, true);
                    //Cost 0 energy because the mandatory ForceGetureEquip already cost 1000
                    //this.UseEnergy(1000, "Mental Mutation");
                    //Popup.Show("Energy afterwards: " + this.ParentObject.Energy, true);
                }
            }


            return true;
        } 
             
    }
}