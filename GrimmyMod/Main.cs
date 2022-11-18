using System;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using Sims3.UI;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.UI;

//Template Created by Battery

namespace probablyzora.GrimmyMod
{
    public class Main
    {
        [Tunable] static bool init;
        static EventListener simInstantiatedEventListener;

        static Main()
        {
            World.sOnWorldLoadFinishedEventHandler += OnWorldLoad;
            World.sOnWorldQuitEventHandler += OnWorldQuit;
        }

        static void OnWorldLoad(object sender, System.EventArgs e)
        {
            simInstantiatedEventListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, OnSimInstantiated);
        }

        static void OnWorldQuit(object sender, System.EventArgs e)
        {
            if (simInstantiatedEventListener != null)
            {
                EventTracker.RemoveListener(simInstantiatedEventListener);
                simInstantiatedEventListener = null;
            }
        }

        /// <summary>
        /// Checks if SimDescription is in the GrimReaper service pool.
        /// </summary>
        /// <param name="simDescription">SimDescription to check.</param>
        /// <returns>True if simDescription is a Grim Reaper, False otherwise.</returns>
        static bool IsGrimReaper(SimDescription simDescription)
        {
            var grimReaper = GrimReaper.Instance;
            if (grimReaper == null)
                return false;
            if (grimReaper.IsSimDescriptionInPool(simDescription))
                return true;
            return false;
        }

        static ListenerAction OnSimInstantiated(Event e)
        {
            Sim sim = null;
            if (e.TargetObject != null)
            {
                if (e.TargetObject is Sim)
                    sim = (Sim)e.TargetObject;
            }
            if (sim == null)
            {
                if (e.Actor != null)
                {
                    if (e.Actor is Sim)
                        sim = (Sim)e.Actor;
                }
            }

            if (sim == null)
                return ListenerAction.Keep;

            if (!IsGrimReaper(sim.SimDescription))
                return ListenerAction.Keep;

            // DEBUG NOTIF
            StyledNotification.Show(new StyledNotification.Format("A " + sim.SimDescription.FullName + " has been instantiated.",
                ObjectGuid.InvalidObjectGuid, sim.ObjectId, StyledNotification.NotificationStyle.kSimTalking));

            // Sets the favorites to random favorites from the GRSituation incase this wasn't done before.
            if (sim.SimDescription.FavoriteFood == FavoriteFoodType.None)
            {
                sim.SimDescription.FavoriteFood = GrimReaper.kFavoriteFoods[RandomUtil.GetInt(0, GrimReaper.kFavoriteFoods.Length - 1)];
            }
            if (sim.SimDescription.FavoriteMusic == FavoriteMusicType.None)
            {
                sim.SimDescription.FavoriteMusic = GrimReaper.kFavoriteMusic[RandomUtil.GetInt(0, GrimReaper.kFavoriteMusic.Length - 1)];
            }
            if (sim.SimDescription.FavoriteColor == Color.Preset.None)
            {
                sim.SimDescription.FavoriteColor = new CASCharacter.NameColorPair("Black", new Color(0, 0, 0)).mColor;
            }

            // Sets Logic Skill if skill level is below minimum.
            Skill skill = sim.SkillManager.AddElement(SkillNames.Logic);
            if (sim.SkillManager.GetSkillLevel(SkillNames.Logic) < GrimReaper.kMinLogicSkill)
            {
                int RandomSkillLevel = RandomUtil.GetInt(GrimReaper.kMinLogicSkill, GrimReaper.kMaxLogicSkill);
                skill.ForceSkillLevelUp(RandomSkillLevel);
            }

            // Sets Chess Hidden Skill if skill level is below minimum.
            skill = sim.SkillManager.AddElement(SkillNames.Chess);
            if (sim.SkillManager.GetSkillLevel(SkillNames.Chess) < GrimReaper.kMinChessSkill)
            {
                int RandomSkillLevel = RandomUtil.GetInt(GrimReaper.kMinChessSkill, GrimReaper.kMaxChessSkill);
                skill.ForceSkillLevelUp(RandomSkillLevel);
            }

            // Gives Reaper smoke effect if not in reaper situation ( that already gives smoke ) and disable it if it does???
            VisualEffect ReaperSmokeFX = null;
            if (!ServiceSituation.IsSimOnJob(sim))
            {
                ReaperSmokeFX = VisualEffect.Create("reaperSmokeConstant");
                ReaperSmokeFX.ParentTo(sim, Sim.FXJoints.Pelvis);
                ReaperSmokeFX.Start();
            }

            // Freeze or remove??? all Physiological Needs
            if (!ServiceSituation.IsSimOnJob(sim))
            {
                (Sims3.Gameplay.UI.Responder.Instance.HudModel as HudModel).OnSimFavoritesChanged(sim.ObjectId);
                sim.Motives.SetValue(CommodityKind.Energy, 100f);
                sim.Motives.SetValue(CommodityKind.Bladder, 100f);
                sim.Motives.SetValue(CommodityKind.Hunger, 100f);
                sim.Motives.SetValue(CommodityKind.Hygiene, 100f);
                sim.Motives.RemoveMotive(CommodityKind.Energy);
                sim.Motives.RemoveMotive(CommodityKind.Bladder);
                sim.Motives.RemoveMotive(CommodityKind.Hunger);
                sim.Motives.RemoveMotive(CommodityKind.Hygiene);
            }

            return ListenerAction.Keep;
        }
        

    }
}