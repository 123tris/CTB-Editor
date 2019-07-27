using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RuntimeUndo
{
    public static class Undo
    {
        public interface IMemento<out T>
        {
            T Revert();
        }

        public class AddGameObject : IMemento<RemoveGameObject>
        {
            private GameObject addedObject;

            public AddGameObject(GameObject obj)
            {
                addedObject = obj;
            }

            public RemoveGameObject Revert()
            {
                GameObject copy = Object.Instantiate(addedObject,GameManager.garbage.transform);
                RemoveGameObject removedObject = new RemoveGameObject(copy,addedObject.transform.parent);
                Object.Destroy(addedObject);
                return removedObject;
            }
        }

        public class RemoveGameObject : IMemento<AddGameObject>
        {
            private GameObject removedObject;
            private Transform parent;

            public RemoveGameObject(GameObject obj,Transform parent)
            {
                removedObject = obj;
                this.parent = parent;
            }

            public AddGameObject Revert()
            {
                GameObject addedObject = Object.Instantiate(removedObject,parent);
                return new AddGameObject(addedObject);
            }
        }

        public class Snapshot : IMemento<Snapshot>
        {
            private IParsable snapshotObject;
            private JObject snapshot; //json data

            public Snapshot(IParsable recordedObject)
            {
                snapshotObject = recordedObject;
                snapshot = JObject.Parse(JsonConvert.SerializeObject(recordedObject));
            }

            public Snapshot Revert()
            {
                Snapshot newSnapshot = new Snapshot(snapshotObject);
                snapshotObject.Parse(snapshot);
                return newSnapshot;
            }
        }

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
        }

        public static void RecordObject(IParsable parsable)
        {
            changes.Push(new Snapshot(parsable));
        }
    }
}