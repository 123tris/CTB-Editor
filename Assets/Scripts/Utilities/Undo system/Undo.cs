using System.Collections.Generic;
using UnityEngine;

namespace RuntimeUndo
{
    public static class Undo
    {
        private static Stack<IMemento<object>> changes = new Stack<IMemento<object>>();
        private static Stack<IMemento<object>> revertedChanges = new Stack<IMemento<object>>();

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
            changes.Push((IMemento<object>) memento.Revert());
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

        public static void DestroyObject(GameObject objectToUndo)
        {
            changes.Push(new RemoveGameObject(objectToUndo,objectToUndo.transform.parent));
            Object.Destroy(objectToUndo);
        }

        public static void RegisterCreatedObjects(List<GameObject> gameObjects)
        {
            changes.Push(new AddGameObjects(gameObjects));
            revertedChanges.Clear();
        }
    }
}