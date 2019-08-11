using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RuntimeUndo
{
    public class Snapshot : IMemento<Snapshot>
    {
        private IParsable snapshotObject;
        private JObject snapshot; //json data

        public Snapshot(IParsable recordedObject)
        {
            snapshotObject = recordedObject;
            snapshot = JObject.Parse(JsonConvert.SerializeObject(recordedObject));
        }

        public Snapshot Revert()
        {
            Snapshot newSnapshot = new Snapshot(snapshotObject);
            snapshotObject.Parse(snapshot);
            return newSnapshot;
        }
    }
}