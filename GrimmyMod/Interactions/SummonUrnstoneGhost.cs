﻿using Sims3.Gameplay.Actors;
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
using Sims3.Gameplay.ThoughtBalloons;

namespace probablyzora.GrimmyMod.Interactions
{
    public sealed class SummonGhost : InteractionGameObjectHit<Sim, Urnstone>
    {

    
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
			Sim sim = this.Actor as Sim;
            Urnstone urnstone = this.Target as Urnstone;
            /// ROUTE TO URNSTONE
            Route route = this.Actor.CreateRoute();
            route.PlanToPointRadialRange(this.Target.Position, 1.5f, 3f, RouteDistancePreference.PreferNearestToRouteDestination, RouteOrientationPreference.TowardsObject, this.Target.LotCurrent.LotId, new int[]
            {
                    this.Target.RoomId
            });
            if (!this.Actor.DoRoute(route))
            {
                return false;
            }
            ///
            sim.PlaySoloAnimation("a_death_showOff_x");
            sim.PlaySoloAnimation("a_death_create_x");
            sim.PlaySoloAnimation("a_death_point_x");
            urnstone.GhostSpawn(false);
            //StyledNotification.Show(new StyledNotification.Format(string.Format("{0} summoned a ghost lmao", sim.SimDescription.FullName),
            //ObjectGuid.InvalidObjectGuid, sim.ObjectId, StyledNotification.NotificationStyle.kSimTalking));
            return true;

        }
        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, Urnstone, SummonGhost>
        {
            public  override string GetInteractionName(Sim sim, Urnstone urnstone, InteractionObjectPair interaction)
            {
                string GhostName = urnstone.DeadSimsDescription.FullName;
                if (!(GhostName==null))
                {
                    return "Call Forth " + GhostName;
                }
                else
                {
                    return "Call Forth Ghost";
                }
            }
            public override bool Test(Sim sim, Urnstone urnstone, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (Main.IsGrimReaper(sim) == true)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
