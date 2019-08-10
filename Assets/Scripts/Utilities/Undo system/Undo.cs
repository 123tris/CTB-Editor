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
            IMemento<object> memento = changes.Pop();
            revertedChanges.Push((IMemento<object>)memento.Revert());
        }

        public static void PerformRedo()
        {
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
    }
}