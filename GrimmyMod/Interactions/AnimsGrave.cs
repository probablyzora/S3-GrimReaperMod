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

namespace probablyzora.GrimmyMod.Interactions
{
    public sealed class AnimsGrave : InteractionGameObjectHit<Sim, Urnstone>
    {

    
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
			Sim sim = this.Actor as Sim;
            Urnstone urnstone = this.Target as Urnstone;
            sim.PlaySoloAnimation("a_death_showOff_x");
            sim.PlaySoloAnimation("a_death_putScythe_x");
            sim.PlaySoloAnimation("a_death_point_x");
            sim.PlaySoloAnimation("a_death_create_x");
            sim.PlaySoloAnimation("a_appear_x");
            sim.PlaySoloAnimation("a_die_electrocution_x");
            sim.PlaySoloAnimation("a_die_drowning_x");
            sim.PlaySoloAnimation("a_die_burning_x");
            sim.BuffManager.AddElement(BuffNames.OnFire, Origin.FromWooHooWithOccult);
            return true;

        }
        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, Urnstone, AnimsGrave>
        {
            public  override string GetInteractionName(Sim sim, Urnstone urnstone, InteractionObjectPair interaction)
            {
                return "AnimsGrave";
            }
            public override bool Test(Sim sim, Urnstone urnstone, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
