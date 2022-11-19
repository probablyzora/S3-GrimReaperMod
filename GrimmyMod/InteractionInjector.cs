using Sims3.Gameplay.Abstracts;
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
        static Dictionary<Type, List<InteractionDefinition>> s_interactionsToInjectPerGameObject = new Dictionary<Type, List<InteractionDefinition>>();

        public static void RegisterInteraction<T>(InteractionDefinition interaction) where T : GameObject
        {
            var interactionList = MakeInteractionToInjectListForType<T>();
            if (interactionList.Contains(interaction))
                return;
            interactionList.Add(interaction);
        }

        public static void UnregisterInteraction<T>(InteractionDefinition interaction) where T : GameObject
        {
            var interactionList = MakeInteractionToInjectListForType<T>();
            if (!interactionList.Contains(interaction))
                return;
            interactionList.Remove(interaction);
        }

        /// <summary>
        /// Call this from entrypoint.
        /// </summary>
        public static void Initialize()
        {
            World.sOnWorldLoadFinishedEventHandler += OnWorldLoad;
            World.sOnWorldQuitEventHandler += OnWorldQuit;
            World.sOnObjectPlacedInLotEventHandler += OnObjectPlaced;
        }

        static void OnWorldLoad(object sender, System.EventArgs e)
        {
            s_simInstantiatedEventListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, OnSimInstantiated);
            foreach (var typeInteraction in s_interactionsToInjectPerGameObject)
            {
                var query = Queries.GetObjects(typeInteraction.Key);
                foreach (GameObject gameObject in query)
                {
                    AddInteractions(gameObject, typeInteraction.Value);
                }
            }
        }

        static void OnObjectPlaced(object sender, EventArgs e)
        {
            var args = e as World.OnObjectPlacedInLotEventArgs;
            var gameObject = args.ObjectId.ObjectFromId<GameObject>();
            if (gameObject == null)
                return;
            AddInteractions(gameObject, GetInteractionInjectListForType(gameObject.GetType()));
        }

        static void OnWorldQuit(object sender, System.EventArgs e)
        {
            if (s_simInstantiatedEventListener != null)
            {
                EventTracker.RemoveListener(s_simInstantiatedEventListener);
                s_simInstantiatedEventListener = null;
            }
        }

        static List<InteractionDefinition> GetInteractionInjectListForType<T>() where T : GameObject
        {
            return GetInteractionInjectListForType(typeof(T));
        }

        static List<InteractionDefinition> GetInteractionInjectListForType(Type type)
        {
            if (s_interactionsToInjectPerGameObject.TryGetValue(type, out var listResult))
                return listResult;
            return null;
        }

        static List<InteractionDefinition> MakeInteractionToInjectListForType<T>() where T : GameObject
        {
            if (s_interactionsToInjectPerGameObject.TryGetValue(typeof(T), out var listResult))
                return listResult;
            listResult = new List<InteractionDefinition>();
            s_interactionsToInjectPerGameObject[typeof(T)] = listResult;
            return listResult;
        }

        static ListenerAction OnSimInstantiated(Event e)
        {
            if (!(e.TargetObject is GameObject gameObject))
                return ListenerAction.Keep;
            AddInteractions(gameObject, GetInteractionInjectListForType(e.TargetObject.GetType()));
            return ListenerAction.Keep;
        }

        static void AddInteractions(GameObject gameObject, List<InteractionDefinition> interactions)
        {
            if (interactions == null)
                return;
            foreach (var interaction in interactions)
            {
                foreach (var pair in gameObject.Interactions)
                {
                    if (pair.InteractionDefinition.GetType() == interaction.GetType())
                    {
                        return;
                    }
                }
                gameObject.AddInteraction(interaction);
            }
        }
    }
}
