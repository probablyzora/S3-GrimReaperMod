using System;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using Sims3.UI;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.UI;
using Sims3.UI.Hud;

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
            VisualEffect.sOnEffectFinishedEventHandler += OnEffectFinished;
            InteractionInjector.Initialize();
            InteractionInjector.RegisterInteraction<Sim>(TestInteraction.Singleton);
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

        /// <summary>
        /// Checks if Sim has GrimReaperFullName. Used incase the Sim isn't spawned naturally or 
        /// isn't in GrimReaper service pool ( eg. NRaas DebugEnabler ).
        /// </summary>
        /// <param name="sim">Sim to check.</param>
        /// <returns>True if sim has both FirstName and LastName, False otherwise. </returns>
        static bool HasGrimReaperName(Sim sim)
        {
            string GrimFirstName = StringTable.GetLocalizedString("Gameplay/SimNames/Custom:GrimReaperFirstName");
            string GrimLastName = StringTable.GetLocalizedString("Gameplay/SimNames/Custom:GrimReaperLastName");
            if (sim == null)
                return false;
            if (sim.FirstName == GrimFirstName || sim.LastName == GrimLastName)
                return true;
            return false;
        }

        //Restart the Grim Reaper smoke effect, so that it loops forever.
        static void OnEffectFinished(object sender, EventArgs e)
        {
            var effectFinishedEventArgs = e as VisualEffect.EffectFinishedEventArgs;
            var fx = effectFinishedEventArgs.ObjectId.ObjectFromId<VisualEffect>();
            if (fx == null)
                return;
            if (fx.EffectName != "reaperSmokeConstant")
                return;
            fx.Start();
        }

        static ListenerAction OnSimInstantiated(Event e)
        {
            var sim = e.TargetObject as Sim;
            if (!HasGrimReaperName(sim))
            {
                if (!IsGrimReaper(sim.SimDescription))
                    return ListenerAction.Keep;
            }

            // DEBUG NOTIF
            StyledNotification.Show(new StyledNotification.Format(string.Format("A {0} has been instantiated", sim.SimDescription.FullName),
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
            // Sets the Walkstyle 
            sim.RequestWalkStyle(Sim.WalkStyle.DeathWalk);

            // Gives ReaperSmokeFX to Sim
            if (IsGrimReaper(sim.SimDescription) || HasGrimReaperName(sim))
            {
                var ReaperSmokeFX = VisualEffect.Create("reaperSmokeConstant");
                ReaperSmokeFX.ParentTo(sim, Sim.FXJoints.Pelvis);
                ReaperSmokeFX.SetAutoDestroy(false);
                ReaperSmokeFX.Start();
            }

            // (Set the affected needs to 100 bcs otherwise it doesn't work??? and) Remove Physiological Needs
            sim.Motives.SetValue(CommodityKind.Energy, 100f);
            sim.Motives.SetValue(CommodityKind.Bladder, 100f);
            sim.Motives.SetValue(CommodityKind.Hunger, 100f);
            sim.Motives.SetValue(CommodityKind.Hygiene, 100f);
            sim.Motives.RemoveMotive(CommodityKind.Energy);
            sim.Motives.RemoveMotive(CommodityKind.Bladder);
            sim.Motives.RemoveMotive(CommodityKind.Hunger);
            sim.Motives.RemoveMotive(CommodityKind.Hygiene);
            
            return ListenerAction.Keep;

        }

    }
}