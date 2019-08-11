using UnityEngine;

namespace RuntimeUndo
{
    public class RemoveGameObject : IMemento<AddGameObject>
    {
        private GameObject removedObject;
        private Transform parent;

        public RemoveGameObject(GameObject obj, Transform parent)
        {
            removedObject = obj;
            this.parent = parent;
        }

        public AddGameObject Revert()
        {
            GameObject addedObject = Object.Instantiate(removedObject, parent);
            return new AddGameObject(addedObject);
        }
    }
}