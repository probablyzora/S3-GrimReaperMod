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
using System;
using System.Collections.Generic;
using System.Text;

namespace probablyzora.GrimmyMod.Interactions
{
    public sealed class CreateGrimReaper : ImmediateInteractionGameObjectHit<IActor, IGameObject>
	{
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
			SimUtils.SimCreationSpec simCreationSpec = new SimUtils.SimCreationSpec();
			simCreationSpec.Normalize();
			simCreationSpec.Age = CASAgeGenderFlags.YoungAdult;
			simCreationSpec.Gender = CASAgeGenderFlags.Male;
			simCreationSpec.GivenName = StringTable.GetLocalizedString("Gameplay/SimNames/Custom:GrimReaperFirstName");
			simCreationSpec.FamilyName = StringTable.GetLocalizedString("Gameplay/SimNames/Custom:GrimReaperLastName");
			SimDescription simDescription = simCreationSpec.Instantiate();
			simDescription.TraitManager.RemoveAllElements();
			Main.SetReaperTraits(simDescription);
			PlumbBob.SelectedActor.Household.Add(simDescription);
			Sim sim = simDescription.Instantiate(this.Hit.mPoint);
			ResourceKey key = ResourceKey.CreateOutfitKey("YmDeath", 0U);
			SimOutfit outfit = new SimOutfit(key);
			SimBuilder simBuilder = new SimBuilder();
			simBuilder.UseCompression = true;
			OutfitUtils.SetOutfit(simBuilder, outfit, null);
			ResourceKey key2 = simBuilder.CacheOutfit("CreateGrimReaper");
			SimOutfit outfit2 = new SimOutfit(key2);
			simDescription.AddOutfit(outfit2, OutfitCategories.Everyday, true);
			Sim.SwitchOutfitHelper switchOutfitHelper = new Sim.SwitchOutfitHelper(sim, OutfitCategories.Everyday, 0);
			switchOutfitHelper.Start();
			switchOutfitHelper.Wait(false);
			switchOutfitHelper.ChangeOutfit();
			switchOutfitHelper.Dispose();
			HudModel hudModel = (HudModel)Sims3.Gameplay.UI.Responder.Instance.HudModel;
			if (hudModel != null)
			{
				hudModel.OnSimAppearanceChanged(sim.ObjectId);
			}
			//VisualEffect.FireOneShotEffect("reaperSmokeConstant", sim, Sim.FXJoints.Pelvis, VisualEffect.TransitionType.HardTransition);
			return true;
		}
        [DoesntRequireTuning]
        public sealed class Definition : ActorlessInteractionDefinition<IActor, IGameObject, CreateGrimReaper>
        {
            public  override string GetInteractionName(IActor a, IGameObject target, InteractionObjectPair interaction)
            {
                return "Create Grim Reaper";
            }
            public override bool Test(IActor a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
