using System.Collections.Generic;
using UnityEngine;

namespace RuntimeUndo
{
    public class AddGameObjects : IMemento<RemoveGameObjects>
    {
        private List<GameObject> addedObjects;

        public AddGameObjects(List<GameObject> objects)
        {
            addedObjects = objects;
        }

        public RemoveGameObjects Revert()
        {
            //TODO: temporarily uses first index for testing purposes. However it should be changed to a list of parents so that every gameobject can have a unique parent if that will ever become necessary
            RemoveGameObjects removedObject = new RemoveGameObjects(addedObjects,addedObjects[0].transform.parent); 
            foreach (GameObject addedObject in addedObjects)
            {
                Object.Destroy(addedObject);
            }
            addedObjects.Clear();
            return removedObject;
        }
    }
}