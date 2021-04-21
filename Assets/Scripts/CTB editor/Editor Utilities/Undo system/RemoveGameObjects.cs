using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
                removedObjects.Add(Object.Instantiate(gameObject, GameManager.garbage.transform));
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

                if (hitobject is Fruit fruit)
                {
                    fruit.SetPosition(fruit.transform.position - Grid.Instance.transform.position);
                    HitObjectManager.AddFruit(fruit);
                }
                else throw new NotImplementedException();
                addedObjects.Add(addedObject);
            }
            return new AddGameObjects(addedObjects);
        }
    }
}