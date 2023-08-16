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
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.Alchemy;

namespace probablyzora.GrimmyMod.Interactions
{
    public sealed class ReviveGhostAsZombie : InteractionGameObjectHit<Sim, Urnstone>
    {


        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            Sim sim = this.Actor as Sim;
            Urnstone urnstone = this.Target as Urnstone;
            Sim GhostSim = urnstone.MyGhost;

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
            //this.mCurrentStateMachine = StateMachineClient.Acquire(GhostSim, "ReanimationClimbOut", AnimationPriority.kAPDefault);
            urnstone.GhostSpawn(false);
            urnstone.GhostToSim(GhostSim, false, false);
            GhostSim.BuffManager.AddElement(BuffNames.PermaZombie, Origin.FromGrimReaper);
            GhostSim.PlaySoloAnimation("a_magicWand_zombie_reanimation_graveClimbOut_x");
            return true;
        }


    }
    [DoesntRequireTuning]
    public sealed class Definition : InteractionDefinition<Sim, Urnstone, ReviveGhostAsZombie>
    {
        public override string GetInteractionName(Sim sim, Urnstone urnstone, InteractionObjectPair interaction)
        {
            string GhostName = urnstone.DeadSimsDescription.FirstName;
            if (!(GhostName == null))
            {
                return "Revive " + GhostName + " as Zombie";
            }
            else
            {
                return "Revive as Zombie";
            }
        }
        public override bool Test(Sim sim, Urnstone urnstone, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if ((GameUtils.IsInstalled(ProductVersion.EP7)) & (Main.IsGrimReaper(sim) == true))
            {
                return true;

            }
            return false;
        }
    }
}

