using UnityEngine;

namespace RuntimeUndo
{
    public class FruitSnapshot : IMemento<FruitSnapshot>
    {
        private Fruit fruit;
        private Vector3 position;

        public FruitSnapshot(Fruit fruit)
        {
            this.fruit = fruit;
            position = fruit.transform.position;
        }

        public FruitSnapshot Revert()
        {
            FruitSnapshot snapshotForRevert = new FruitSnapshot(fruit);
            fruit.SetPosition(position - Grid.Instance.transform.position);
            return snapshotForRevert;
        }
    }
}