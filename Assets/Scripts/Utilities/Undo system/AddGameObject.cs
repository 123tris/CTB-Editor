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
            GameObject copy = Object.Instantiate(addedObject,GameManager.garbage.transform);
            RemoveGameObject removedObject = new RemoveGameObject(copy,addedObject.transform.parent);
            Object.Destroy(addedObject);
            return removedObject;
        }
    }
}