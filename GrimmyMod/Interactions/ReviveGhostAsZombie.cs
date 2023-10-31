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
using Sims3.Gameplay.Socializing;

namespace probablyzora.GrimmyMod.Interactions
{
    public sealed class ReviveGhostAsZombie : InteractionGameObjectHit<Sim, Urnstone>
    {
        public void ShowZombie(StateMachineClient sender, IEvent evt)
        {
            this.Target.MyGhost.SetOpacity(1f, 0f);

        }
        public static readonly InteractionDefinition Singleton = new Definition();
        public override bool Run()
        {
            Sim sim = this.Actor as Sim;
            Urnstone urnstone = this.Target as Urnstone;
            Sim GhostSim = urnstone.MyGhost;
            SimOutfit simOutfit = null;
            ObjectSound objectsound = new ObjectSound(this.Actor.ObjectId, "death_reaper_lp");

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
            VisualEffect mSmokeEffect = VisualEffect.Create("reaperSmokeConstant");
            mSmokeEffect.SetPosAndOrient(this.Target.Position, this.Target.ForwardVector, Vector3.UnitY);
            mSmokeEffect.Start();
            //sim.PlaySoloAnimation("a_death_showOff_x");
            //sim.PlaySoloAnimation("a_death_create_x");
            //sim.PlaySoloAnimation("a_death_point_x");
            //this.mCurrentStateMachine = StateMachineClient.Acquire(GhostSim, "ReanimationClimbOut", AnimationPriority.kAPDefault);
            base.StandardEntry();
            base.BeginCommodityUpdates();
            base.AcquireStateMachine("zora_ReaperInteractions");
            base.SetActor("x", base.Actor);
            base.EnterSim("Enter");
            base.AnimateSim("GetScythe");
            base.AnimateSim("CreateTombstone");
            try
            {
                simOutfit = this.Target.DeadSimsDescription.GetOutfit(OutfitCategories.Everyday, 0);
                ThumbnailManager.GenerateHouseholdSimThumbnail(simOutfit.Key, simOutfit.Key.InstanceId, 0U, ThumbnailSizeMask.Large, ThumbnailTechnique.Default, false, false, this.Target.DeadSimsDescription.AgeGenderSpecies);
                ThumbnailKey thumbnailKey = new ThumbnailKey(simOutfit, 0, ThumbnailSize.Large, ThumbnailCamera.Default);
                ThoughtBalloonManager.DoubleBalloonData doubleBalloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_zombie", thumbnailKey);
                doubleBalloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                doubleBalloonData.mPriority = ThoughtBalloonPriority.High;
                this.Actor.ThoughtBalloonManager.ShowBalloon(doubleBalloonData);
                base.AnimateSim("ReaperPointAtGrave");
                base.AnimateSim("PutAwayScythe");
                base.AnimateSim("Exit");
                base.StandardExit();
                if (objectsound != null)
                {
                    objectsound.Dispose();
                    objectsound = null;
                }
                urnstone.GhostSpawn(false);
                urnstone.GhostToSim(GhostSim, false, false);
                if ((GhostSim.SimDescription.IsPet != false)&&(GhostSim.SimDescription.ChildOrBelow != false))
                {
                    GhostSim.BuffManager.AddElement(BuffNames.PermaZombie, Origin.FromGrimReaper);
                    Relationship relationship = Relationship.Get(this.Actor, GhostSim, true);
                    relationship.UpdateSTCAndLTR(this.Actor, GhostSim, CommodityTypes.Friendly, MagicWand.CastReanimate.kRelationshipBoostWithZombie);
                    MagicWand.BeReanimated beReanimated = MagicWand.BeReanimated.Singleton.CreateInstance(GhostSim, GhostSim, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, false) as MagicWand.BeReanimated;
                    beReanimated.LinkedInteractionInstance = this;
                    GhostSim.InteractionQueue.AddNext(beReanimated);
                    base.StandardEntry();
                    base.BeginCommodityUpdates();
                    base.AcquireStateMachine("ReanimationClimbOut");
                    base.SetActor("x", GhostSim);
                    base.StandardEntry();
                    base.AnimateSim("climb_out");
                    base.AddOneShotScriptEventHandler(201U, new SacsEventHandler(this.ShowZombie));
                    base.AnimateSim("Exit");
                    base.StandardExit();
                }
                else
                {
                    Relationship relationship = Relationship.Get(this.Actor, GhostSim, true);
                    relationship.UpdateSTCAndLTR(this.Actor, GhostSim, CommodityTypes.Friendly, MagicWand.CastReanimate.kRelationshipBoostWithZombie);
                }
                if (mSmokeEffect != null)
                {
                    mSmokeEffect.Stop();
                    mSmokeEffect.Dispose();
                    mSmokeEffect = null;
                }
                EventTracker.SendEvent(EventTypeId.kZombieMaster, this.Actor, GhostSim);
            }
            catch (Exception e)
            {
                StyledNotification.Show(new StyledNotification.Format(e.ToString(),
                                        StyledNotification.NotificationStyle.kGameMessageNegative));
            }
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
                return "Reanimate " + GhostName;
            }
            else
            {
                return "Reanimate Ghost";
            }
        }
        public override bool Test(Sim sim, Urnstone urnstone, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if ((Main.IsGrimReaper(sim) == true))
            {
                if (urnstone.DeadSimsDescription != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

