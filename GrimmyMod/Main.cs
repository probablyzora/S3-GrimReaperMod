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
using probablyzora.GrimmyMod.Interactions;
using Sims3.Gameplay.ActorSystems;
using System.Collections.Generic;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Abstracts;

//  Template Created by Battery , InteractionInjector and most of the technical Main.cs done by LazyDuchess
//  Ideas by ProbablyZora

namespace probablyzora.GrimmyMod
{
    public class Main
    {
        [Tunable] static bool init;
        static EventListener simInstantiatedEventListener;
        static EventListener eventSimSelected;
        static Main()
        {
            World.sOnWorldLoadFinishedEventHandler += OnWorldLoad;
            World.sOnWorldQuitEventHandler += OnWorldQuit;
            VisualEffect.sOnEffectFinishedEventHandler += OnEffectFinished;

            InteractionInjector.Initialize();
            InteractionInjector.RegisterInteraction<Terrain>(CreateGrimReaper.Singleton);
            InteractionInjector.RegisterInteraction<Urnstone>(SummonGhost.Singleton);
            InteractionInjector.RegisterInteraction<Urnstone>(ReviveGhostAsZombie.Singleton);
            InteractionInjector.RegisterInteraction<Terrain>(AppearHere.Singleton);
        }

        static void OnWorldLoad(object sender, System.EventArgs e)
        {
            simInstantiatedEventListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, OnSimInstantiated);
            //eventSimSelected = EventTracker.AddListener(EventTypeId.kEventSimSelected, OnEventSimSelected);

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
        static bool IsGrimReaperService(SimDescription simDescription)
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
        /// <summary>
        /// Checks if Sim either has the localized name or is in the GR Service Pool.
        /// Temporary solution until i manage to get the traits working lol
        /// </summary>
        /// <param name="simDescription"></param>
        /// <reutrns>True if either is true, false if not in pool or isn't named correctly</reutrns>
        public static bool IsGrimReaper(Sim sim)
        {
            if (HasGrimReaperName(sim))
            {
                return true;
            }
            if (IsGrimReaperService(sim.SimDescription))
            {
                return true;
            }
            return false;
        }
        public static void SetReaperTraits(SimDescription simDescription)
        {
            List<TraitNames> ReaperTraitsList = new List<TraitNames>
            {
                //////////////////////// Vanilla Traits
                TraitNames.AbsentMinded,
                TraitNames.BookWorm,
                TraitNames.Brave,
                TraitNames.Childish,
                TraitNames.CouchPotato,
                TraitNames.EasilyImpressed,
                TraitNames.GoodSenseOfHumor,
                TraitNames.Grumpy,
                TraitNames.Insane,
                TraitNames.Inappropriate,
                TraitNames.Kleptomaniac,
                TraitNames.Neat,
                TraitNames.NoSenseOfHumor,
                TraitNames.Neurotic,
                TraitNames.Evil,
                TraitNames.MeanSpirited,
                //////////////////////// New Traits
                // BG Traits
                TraitNames.Workaholic,
                TraitNames.Virtuoso,
                // WA Traits
                TraitNames.Adventurous,
                // AM Traits
                TraitNames.Eccentric,
                TraitNames.Perceptive,
                // LN Traits
                TraitNames.Shy,
                // GEN Traits
                TraitNames.Rebellious,
                // PET Traits
                TraitNames.Equestrian,
                TraitNames.AnimalLover,
                TraitNames.DogPerson,
                // ST Traits
                // none lmao
                // SN Traits
                TraitNames.BroodingTrait,
                TraitNames.GathererTrait,
                TraitNames.NightOwlTrait,
                TraitNames.SupernaturalFanTrait,
                // SEA Traits
                TraitNames.LovesTheCold,
                // UL Traits
                TraitNames.AvantGarde,
                TraitNames.SociallyAwkward,
                // IL Traits
                TraitNames.LovesToSwim,
                // ITF Traits
                // i would put unstable here but idk how it functions :sob: and would kinda make all of this redundant
            };
            int TraitsSlotsTaken = 0;
            while (TraitsSlotsTaken != 5)
            {
                TraitNames RandomTraitNameFromList = RandomUtil.GetRandomObjectFromList<TraitNames>(ReaperTraitsList);
                if (GameUtils.IsInstalled((TraitManager.GetTraitFromDictionary(RandomTraitNameFromList).ProductVersion)))
                {
                    simDescription.TraitManager.AddElement(RandomTraitNameFromList);
                    TraitsSlotsTaken += 1;
                }
                ReaperTraitsList.Remove(RandomTraitNameFromList);
            }
        }
        public static void OverrideServiceNPCProperties(SimDescription simDescription)
        {
            simDescription.MotivesDontDecay = false;
            simDescription.Marryable = true;
            simDescription.Contactable = true;
            simDescription.ShowSocialsOnSim = true;
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
        public static void UpdateSimFavorites(SimDescription simDescription)
        {
            simDescription.FavoriteFood = GrimReaper.kFavoriteFoods[RandomUtil.GetInt(0, GrimReaper.kFavoriteFoods.Length - 1)];
            simDescription.FavoriteMusic = GrimReaper.kFavoriteMusic[RandomUtil.GetInt(0, GrimReaper.kFavoriteMusic.Length - 1)];
            simDescription.FavoriteColor = new CASCharacter.NameColorPair("Black", new Color(0, 0, 0)).mColor;
            return;
        }
        static ListenerAction OnSimInstantiated(Event e)
        {
            var sim = e.TargetObject as Sim;
            if (!HasGrimReaperName(sim))
            {
                if (!IsGrimReaperService(sim.SimDescription))
                    return ListenerAction.Keep;
            }
            // DEBUG NOTIF
            StyledNotification.Show(new StyledNotification.Format(string.Format("A {0} has been instantiated", sim.SimDescription.FullName),
                ObjectGuid.InvalidObjectGuid, sim.ObjectId, StyledNotification.NotificationStyle.kSimTalking));
            OverrideServiceNPCProperties(sim.SimDescription);
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
            VisualEffect.FireOneShotEffect("reaperSmokeConstant", sim, Sim.FXJoints.Pelvis, VisualEffect.TransitionType.HardTransition);
            /*
            var ReaperSmokeFX = VisualEffect.Create("reaperSmokeConstant");
            ReaperSmokeFX.ParentTo(sim, Sim.FXJoints.Pelvis);
            ReaperSmokeFX.SetAutoDestroy(false);
            ReaperSmokeFX.Start();
            */
            sim.Motives.SetValue(CommodityKind.Bladder, 100f);
            sim.Motives.SetValue(CommodityKind.Hunger, 100f);
            sim.Motives.SetValue(CommodityKind.Hygiene, 100f);
            sim.Motives.RemoveMotive(CommodityKind.Bladder);
            sim.Motives.RemoveMotive(CommodityKind.Hunger);
            sim.Motives.RemoveMotive(CommodityKind.Hygiene);
            return ListenerAction.Keep;

        }
        /*static ListenerAction OnEventSimSelected(Event e)
        {
            var sim = e.TargetObject as Sim;
            if (!HasGrimReaperName(sim))
            {
                if (!IsGrimReaperService(sim.SimDescription))
                    return ListenerAction.Keep;
            }
            // DEBUG NOTIF
            StyledNotification.Show(new StyledNotification.Format(string.Format("{0} has been selected.", sim.SimDescription.FullName),
                ObjectGuid.InvalidObjectGuid, sim.ObjectId, StyledNotification.NotificationStyle.kSimTalking));
            OverrideServiceNPCProperties(sim.SimDescription);
            return ListenerAction.Keep;
        }*/
    }
}
