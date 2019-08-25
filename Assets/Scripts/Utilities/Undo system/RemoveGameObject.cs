using UnityEngine;

namespace RuntimeUndo
{
    public class RemoveGameObject : IMemento<AddGameObject>
    {
        private GameObject removedObject;
        private Transform parent;

        public RemoveGameObject(GameObject obj, Transform parent)
        {
            GameObject copy = Object.Instantiate(obj,GameManager.garbage.transform);
            removedObject = copy;
            this.parent = parent;
        }

        public AddGameObject Revert()
        {
            GameObject addedObject = Object.Instantiate(removedObject, parent);
            HitObject hitobject = addedObject.GetComponent<HitObject>();
            if (hitobject != null)
                hitobject.Init(hitobject.transform.position - Grid.Instance.transform.position); //TODO: Clean up dirty code
            return new AddGameObject(addedObject);
        }
    }
}