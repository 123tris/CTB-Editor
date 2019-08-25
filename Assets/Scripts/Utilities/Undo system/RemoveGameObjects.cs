using System.Collections.Generic;
using UnityEngine;

namespace RuntimeUndo
{
    public class RemoveGameObjects : IMemento<AddGameObjects>
    {
        private List<GameObject> removedObjects = new List<GameObject>();
        private Transform parent;

        public RemoveGameObjects(List<GameObject> objects, Transform parent)
        {
            foreach (GameObject gameObject in objects)
            {
                removedObjects.Add(Object.Instantiate(gameObject,GameManager.garbage.transform));
            }
            this.parent = parent;
        }

        public AddGameObjects Revert()
        {
            List<GameObject> addedObjects = new List<GameObject>();
            foreach (GameObject removedObject in removedObjects)
            {
                GameObject addedObject = Object.Instantiate(removedObject, parent);
                HitObject hitobject = addedObject.GetComponent<HitObject>();
                if (hitobject != null)
                    hitobject.Init(hitobject.transform.position - Grid.Instance.transform.position); //TODO: Clean up dirty code
                addedObjects.Add(addedObject);
            }
            return new AddGameObjects(addedObjects);
        }
    }
}