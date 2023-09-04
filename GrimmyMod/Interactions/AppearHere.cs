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
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.UI.CAS;

namespace probablyzora.GrimmyMod.Interactions
{
    public sealed class AppearHere : TerrainInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            Sim sim = this.Actor;
            ObjectSound objectsound = new ObjectSound(this.Actor.ObjectId, "death_reaper_lp");
            base.StandardEntry();
            base.BeginCommodityUpdates();
            base.AcquireStateMachine("zora_ReaperInteractions");
            base.SetActor("x", base.Actor);
            base.EnterSim("Enter");
            base.AnimateSim("Enter");
            sim.FadeOut(false, false, 2f);
            base.AnimateSim("ReaperDisappear");
            sim.SimRoutingComponent.DisableDynamicFootprint();
            sim.SetPosition(this.Destination);;
            sim.FadeIn(false,0.5f);
            base.AnimateSim("ReaperAppear");
            ReactionBroadcaster scaredReactionBroadcaster = new ReactionBroadcaster(this.Actor, Sims3.Gameplay.Services.GrimReaperSituation.ScaredParams, Sims3.Gameplay.Services.GrimReaperSituation.ScaredDelegate);
            if (RandomUtil.RandomChance(25f))
            {
                base.AnimateSim("ReaperBrushingHimself");
            }
            base.StandardExit();
            if (objectsound != null)
            {
                objectsound.Dispose();
                objectsound = null;
            }
            return true;
        }
        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, Terrain, AppearHere>
        {
            public override string GetInteractionName(Sim sim, Terrain target, InteractionObjectPair interaction)
            {
                return "Appear Here";
            }
            public override bool Test(Sim sim, Terrain target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((Main.IsGrimReaper(sim) == true) & (!(sim.Posture is SwimmingInPool)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
