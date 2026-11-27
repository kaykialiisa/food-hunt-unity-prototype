using UnityEngine;

namespace FoodHunt
{
    public sealed class HarvestBox : MonoBehaviour
    {
        internal int ColorId { get; private set; }
        internal Material BodyColor { get; private set; }
        internal int Remaining { get; private set; }
        internal int InFlight { get; set; }
        // A moving or newly activated box has not exhausted any version of the board yet.
        internal int BlockedAtRevision { get; set; } = -1;
        private TextMesh counter;
        private Transform lid;

        internal void Initialize(QueueBoxData data, Material material, TextMesh label, Transform hinge)
        {
            ColorId = data.color;
            BodyColor = material;
            Remaining = data.count;
            counter = label;
            lid = hinge;
        }

        internal void Open()
        {
            counter.transform.SetParent(lid, true);
            lid.RotateAround(lid.position, transform.right, -78f);
        }

        internal void Deliver()
        {
            InFlight--;
            Remaining--;
            counter.text = Remaining.ToString();
            if (Remaining == 0) Destroy(gameObject);
        }
    }
}
