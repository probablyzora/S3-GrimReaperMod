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
                ThumbnailKey thumbnailKey = this.Target.DeadSimsDescription.GetDeceasedThumbnailKey(ThumbnailSize.Large, 0);
                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_moodlet_mourning", thumbnailKey);
                balloonData.mPriority = ThoughtBalloonPriority.High;
                this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
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
                if (mSmokeEffect != null)
                {
                    mSmokeEffect.Stop();
                    mSmokeEffect.Dispose();
                    mSmokeEffect = null;
                }
            }
            catch (Exception e)
            {
                StyledNotification.Show(new StyledNotification.Format(e.ToString(),
                                        StyledNotification.NotificationStyle.kGameMessageNegative));
            }
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
}
