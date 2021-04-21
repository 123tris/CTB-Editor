using UnityEngine;

namespace RuntimeUndo
{
    public class AddGameObject : IMemento<RemoveGameObject>
    {
        private GameObject addedObject;

        public AddGameObject(GameObject obj)
        {
            addedObject = obj;
        }

        public RemoveGameObject Revert()
        {
            RemoveGameObject removedObject = new RemoveGameObject(addedObject,addedObject.transform.parent);
            Object.Destroy(addedObject);
            return removedObject;
        }
    }
}