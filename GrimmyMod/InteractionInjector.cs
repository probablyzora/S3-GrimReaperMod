using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace probablyzora.GrimmyMod
{
    public static class InteractionInjector
    {
        static EventListener s_simInstantiatedEventListener;
        static List<InteractionDefinition> s_interactionsToInject = new List<InteractionDefinition>();

        /// <summary>
        /// Call this from entrypoint.
        /// </summary>
        public static void Initialize()
        {
            World.sOnWorldLoadFinishedEventHandler += OnWorldLoad;
            World.sOnWorldQuitEventHandler += OnWorldQuit;
        }

        public static void RegisterSimInteraction(InteractionDefinition interaction)
        {
            if (s_interactionsToInject.Contains(interaction))
                return;
            s_interactionsToInject.Add(interaction);
        }

        public static void UnregisterSimInteraction(InteractionDefinition interaction)
        {
            if (!s_interactionsToInject.Contains(interaction))
                return;
            s_interactionsToInject.Remove(interaction);
        }

        static void OnWorldLoad(object sender, System.EventArgs e)
        {
            s_simInstantiatedEventListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, OnSimInstantiated);
            foreach (var sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                AddInteractions(sim);
            }
        }

        static void OnWorldQuit(object sender, System.EventArgs e)
        {
            if (s_simInstantiatedEventListener != null)
            {
                EventTracker.RemoveListener(s_simInstantiatedEventListener);
                s_simInstantiatedEventListener = null;
            }
        }

        static ListenerAction OnSimInstantiated(Event e)
        {
            var sim = e.TargetObject as Sim;
            AddInteractions(sim);
            return ListenerAction.Keep;
        }

        static void AddInteractions(Sim sim)
        {
            foreach (var interaction in s_interactionsToInject)
            {
                foreach (var pair in sim.Interactions)
                {
                    if (pair.InteractionDefinition.GetType() == interaction.GetType())
                    {
                        return;
                    }
                }
                sim.AddInteraction(interaction);
            }
        }
    }
}
