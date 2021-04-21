using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RuntimeUndo
{
    public static class Undo
    {
        private static Stack<IMemento<object>> changes = new Stack<IMemento<object>>();
        private static Stack<IMemento<object>> revertedChanges = new Stack<IMemento<object>>();


        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            changes.Clear();
            revertedChanges.Clear();
        }

        public static void PerformUndo()
        {
            if (changes.Count == 0) return;
            IMemento<object> memento = changes.Pop();
            revertedChanges.Push((IMemento<object>)memento.Revert());
        }

        public static void PerformRedo()
        {
            if (revertedChanges.Count == 0) return;
            IMemento<object> memento = revertedChanges.Pop();
            changes.Push((IMemento<object>)memento.Revert());
        }

        public static void RegisterCreatedObject(GameObject obj)
        {
            changes.Push(new AddGameObject(obj));
            revertedChanges.Clear();
        }

        public static void RecordObject(IParsable parsable)
        {
            changes.Push(new Snapshot(parsable));
            revertedChanges.Clear();
        }

        public static void RecordHitObject(HitObject hitObject)
        {
            changes.Push(new HitObjectSnapshot(hitObject));
            revertedChanges.Clear();
        }

        public static void RecordHitObjects(List<HitObject> hitObjects)
        {
            changes.Push(new HitObjectsSnapshot(hitObjects));
            revertedChanges.Clear();
        }

        public static void DestroyObject(GameObject objectToUndo)
        {
            changes.Push(new RemoveGameObject(objectToUndo, objectToUndo.transform.parent));
            Object.Destroy(objectToUndo);
            revertedChanges.Clear();
        }

        public static void DestroyObjects(List<GameObject> objectsToUndo)
        {
            Assert.IsTrue(objectsToUndo.Count > 0);
            changes.Push(new RemoveGameObjects(objectsToUndo, objectsToUndo[0].transform.parent));
            objectsToUndo.ForEach(Object.Destroy);
            revertedChanges.Clear();
        }

        public static void RegisterCreatedObjects(List<GameObject> gameObjects)
        {
            changes.Push(new AddGameObjects(gameObjects));
            revertedChanges.Clear();
        }
    }
}