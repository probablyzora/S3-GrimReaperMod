using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace probablyzora.GrimmyMod
{
    public sealed class TestInteraction : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            StyledNotification.Show(new StyledNotification.Format("I'm slowly going insane",
                        StyledNotification.NotificationStyle.kGameMessagePositive));
            return true;
        }
        [DoesntRequireTuning]
        public sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, TestInteraction>
        {
            public  override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return "Grim Reaper Test Bullshit";
            }
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
