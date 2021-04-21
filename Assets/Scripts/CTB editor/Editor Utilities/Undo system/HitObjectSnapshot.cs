using UnityEngine;

namespace RuntimeUndo
{
    public class HitObjectSnapshot : IMemento<HitObjectSnapshot>
    {
        private HitObject hitObject;
        private Vector3 position;

        public HitObjectSnapshot(HitObject hitObject)
        {
            this.hitObject = hitObject;
            position = hitObject.transform.position;
        }

        public HitObjectSnapshot Revert()
        {
            HitObjectSnapshot snapshotForRevert = new HitObjectSnapshot(hitObject);
            hitObject.SetPosition(position - Grid.Instance.transform.position);
            return snapshotForRevert;
        }
    }
}