using System.Collections.Generic;
using UnityEngine;

namespace RuntimeUndo
{
    public class HitObjectsSnapshot : IMemento<HitObjectsSnapshot>
    {
        private List<HitObject> hitObjects;
        private List<Vector3> positions = new List<Vector3>();

        public HitObjectsSnapshot(List<HitObject> hitObjects)
        {
            this.hitObjects = hitObjects;
            foreach (HitObject hitObject in hitObjects)
            {
                positions.Add(hitObject.transform.position);
            }
        }

        public HitObjectsSnapshot Revert()
        {
            HitObjectsSnapshot snapshotForRevert = new HitObjectsSnapshot(hitObjects); //Create snapshot of new changes for redoing them
            for (int i = 0; i < hitObjects.Count; i++)
            {
                hitObjects[i].SetPosition(positions[i]-Grid.Instance.transform.position);
            }
            return snapshotForRevert;
        }
    }
}