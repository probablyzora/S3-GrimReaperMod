using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace probablyzora.GrimmyMod.Interactions
{
    public sealed class SwitchWalkstyle : ImmediateInteractionGameObjectHit<Sim, IGameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            Sim sim = this.Actor as Sim;

            if (sim.CurrentWalkStyle == Sim.WalkStyle.DeathWalk)
            {
                sim.UnrequestWalkStyle(Sim.WalkStyle.DeathWalk);
            }
            else
            {
                sim.RequestWalkStyle(Sim.WalkStyle.DeathWalk);
            }
            
            return true;
        }
        [DoesntRequireTuning]
        public sealed class Definition : ActorlessInteractionDefinition<Sim, IGameObject, SwitchWalkstyle>
        {
            public override string GetInteractionName(Sim sim, IGameObject target, InteractionObjectPair interaction)
            {
                return "Switch Walkstyle";
                /// I also tried two strings but like it doesn't matter so li
            }
            public override bool Test(Sim sim, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!(sim.IsSleeping || sim.Posture is SwimmingInPool))
                try{
                    if (Main.IsGrimReaper(sim))
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    StyledNotification.Show(new StyledNotification.Format(e.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                return false;

            }
        }
    }
}
