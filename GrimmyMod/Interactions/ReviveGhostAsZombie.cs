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
                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_zombie", thumbnailKey);
                balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                balloonData.mPriority = ThoughtBalloonPriority.High;
                this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                base.AnimateSim("ReaperPointAtGrave");
                base.AnimateSim("PutAwayScythe");
                StyledNotification.Show(new StyledNotification.Format(string.Format("put away scythe", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                base.AnimateSim("Exit");
                StyledNotification.Show(new StyledNotification.Format(string.Format("exit", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                base.StandardExit();
                StyledNotification.Show(new StyledNotification.Format(string.Format("standard exit", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));

                if (objectsound != null)
                {
                    objectsound.Dispose();
                    StyledNotification.Show(new StyledNotification.Format(string.Format("object sound dispose", sim.SimDescription.FullName),
                                                                          ObjectGuid.InvalidObjectGuid,
                                                                          sim.ObjectId,
                                                                          StyledNotification.NotificationStyle.kSimTalking));
                    objectsound = null;
                }
                urnstone.GhostSpawn(false);
                StyledNotification.Show(new StyledNotification.Format(string.Format("ghost spawn", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                //GhostSim.DisableAutonomousInteractions();
                StyledNotification.Show(new StyledNotification.Format(string.Format("disable autonomy", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                if (GhostSim == null)
                {
                    GhostSim = urnstone.MyGhost;
                }
                urnstone.GhostToSim(GhostSim, true, false);
                StyledNotification.Show(new StyledNotification.Format(string.Format("ghost to sim", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                MagicWand.BeReanimated entry = MagicWand.BeReanimated.Singleton.CreateInstance(GhostSim,
                                                                                               GhostSim,
                                                                                               new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior),
                                                                                               false,
                                                                                               false) as MagicWand.BeReanimated;
                GhostSim.InteractionQueue.CancelAllInteractions();
                StyledNotification.Show(new StyledNotification.Format(string.Format("cancel interactions", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                GhostSim.InteractionQueue.Add(entry);
                StyledNotification.Show(new StyledNotification.Format(string.Format("added entry", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                StyledNotification.Show(new StyledNotification.Format(string.Format("before state machine", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                /// /// /// /// /// /// /// /// /// /// /// /// /// /// HANGS UP HERE
                StyledNotification.Show(new StyledNotification.Format(string.Format("exit", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                GhostSim.EnableAutonomousInteractions();
                StyledNotification.Show(new StyledNotification.Format(string.Format("enable inter", sim.SimDescription.FullName),
                                                                      ObjectGuid.InvalidObjectGuid,
                                                                      sim.ObjectId,
                                                                      StyledNotification.NotificationStyle.kSimTalking));
                EventTracker.SendEvent(EventTypeId.kCastReanimation, this.Actor, GhostSim);
                EventTracker.SendEvent(EventTypeId.kZombieMaster, this.Actor, GhostSim);
            }
            catch(Exception e)
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

