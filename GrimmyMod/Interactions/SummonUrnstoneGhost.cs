using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using probablyzora.GrimmyMod;
using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Pools;

namespace probablyzora.GrimmyMod.Interactions
{
    public sealed class SummonGhost : InteractionGameObjectHit<Sim, Urnstone>
    {

    
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
			Sim sim = this.Actor as Sim;
            Urnstone urnstone = this.Target as Urnstone;

            sim.RouteTurnToFace(Target.Position);
            sim.PlaySoloAnimation("a_react_point_laugh_x");
            urnstone.GhostSpawn(false);
            StyledNotification.Show(new StyledNotification.Format(string.Format("{0} summoned a ghost lmao", sim.SimDescription.FullName),
            ObjectGuid.InvalidObjectGuid, sim.ObjectId, StyledNotification.NotificationStyle.kSimTalking));
            return true;

        }
        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, Urnstone, SummonGhost>
        {
            public  override string GetInteractionName(Sim sim, Urnstone urnstone, InteractionObjectPair interaction)
            {
                return "Summon Ghost";
            }
            public override bool Test(Sim sim, Urnstone urnstone, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (urnstone.DeadSimsDescription != null)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
