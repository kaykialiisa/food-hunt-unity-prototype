using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodHunt
{
    [Serializable]
    internal sealed class QueueColumn
    {
        public QueueBoxData[] boxes;
    }

    [Serializable]
    internal struct QueueBoxData
    {
        public int color;
        public int count;
    }

    [DisallowMultipleComponent]
    public sealed class SlotQueueLayout : MonoBehaviour
    {
        [SerializeField] private Transform slots;
        [SerializeField] private Transform queue;
        [SerializeField] private GameObject boxModel;

        private const float ColumnSpacing = 1.47f;
        private const float RowSpacing = 1.50f;
        private const float BoxScale = 1.44f;
        private const int VisibleRows = 4;
        private const float MoveDuration = 0.2f;

        private Camera inputCamera;
        private MeshRenderer[] slotPads;
        private HarvestBox[] occupants;
        private AntColony colony;

        internal bool CanLoad(QueueColumn[] columns, Dictionary<int, Material> palette)
        {
            if (!slots || !queue || !boxModel)
                return false;

            foreach (var column in columns)
            {
                if (column?.boxes == null) return false;
                foreach (var box in column.boxes)
                    if (box.count <= 0 || !palette.ContainsKey(box.color)) return false;
            }
            return true;
        }

        internal Bounds Load(QueueColumn[] columns, Dictionary<int, Material> palette, AntColony antColony)
        {
            var bounds = new Bounds(slots.position, Vector3.zero);
            inputCamera = Camera.main;
            colony = antColony;
            slotPads = slots.GetComponentsInChildren<MeshRenderer>();
            Array.Sort(slotPads, (left, right) => slots.InverseTransformPoint(left.bounds.center).x
                .CompareTo(slots.InverseTransformPoint(right.bounds.center).x));
            occupants = new HarvestBox[slotPads.Length];
            foreach (var pad in slotPads)
                bounds.Encapsulate(pad.bounds);

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            for (int i = 0; i < columns.Length; i++)
            {
                var column = new GameObject($"Column_{i + 1}").transform;
                column.SetParent(queue, false);
                column.localPosition = Vector3.right * ((i - (columns.Length - 1) * 0.5f) * ColumnSpacing);
                var boxes = columns[i].boxes;
                for (int row = 0; row < boxes.Length; row++)
                {
                    var box = new GameObject($"Box_{row + 1}").transform;
                    box.SetParent(column, false);
                    box.localPosition = Vector3.back * (row * RowSpacing);
                    box.localScale = Vector3.one * BoxScale;
                    var model = Instantiate(boxModel, box, false);
                    var hitArea = box.gameObject.AddComponent<BoxCollider>();
                    // Queue_Box has a 0.80 x 0.80 lid and stands 0.385 units high.
                    hitArea.size = new Vector3(0.80f, 0.385f, 0.80f);
                    hitArea.center = Vector3.up * (hitArea.size.y * 0.5f);
                    hitArea.enabled = row == 0;
                    foreach (var part in model.GetComponentsInChildren<MeshRenderer>())
                    {
                        part.sharedMaterial = palette[boxes[row].color];
                        bounds.Encapsulate(part.bounds);
                    }
                    var label = AddCounter(box, boxes[row].count, font);
                    var lid = model.transform.Find("Box_Lid");
                    box.gameObject.AddComponent<HarvestBox>().Initialize(boxes[row], palette[boxes[row].color], label, lid);
                    box.gameObject.SetActive(row < VisibleRows);
                }
            }
            return bounds;
        }

        private void Update()
        {
            var pointer = Pointer.current;
            if (!inputCamera || pointer == null || !pointer.press.wasPressedThisFrame) return;

            Ray ray = inputCamera.ScreenPointToRay(pointer.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit)) return;

            Transform box = hit.collider.transform;
            Transform column = box.parent;
            if (!column || column.parent != queue || box.GetSiblingIndex() != 0) return;

            int index = Array.FindIndex(occupants, occupant => !occupant);
            if (index < 0) return;

            occupants[index] = box.GetComponent<HarvestBox>();
            hit.collider.enabled = false;
            StartCoroutine(MoveToSlot(box, column, slotPads[index]));
        }

        private IEnumerator MoveToSlot(Transform box, Transform column, MeshRenderer pad)
        {
            Vector3 start = box.position;
            Bounds surface = pad.bounds;
            Vector3 destination = new Vector3(surface.center.x, surface.max.y, surface.center.z);
            box.SetParent(pad.transform, true);

            if (column.childCount >= VisibleRows)
                column.GetChild(VisibleRows - 1).gameObject.SetActive(true);

            float elapsed = 0f;
            while (elapsed < MoveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / MoveDuration);
                box.position = Vector3.Lerp(start, destination, t) + Vector3.up * (0.8f * t * (1f - t));
                for (int row = 0; row < column.childCount; row++)
                    column.GetChild(row).localPosition = Vector3.back * ((row + 1f - t) * RowSpacing);
                yield return null;
            }

            if (column.childCount > 0)
                column.GetChild(0).GetComponent<BoxCollider>().enabled = true;
            colony.BeginHarvest(box.GetComponent<HarvestBox>());
        }

        internal bool AllSlotsBlocked(int boardRevision)
        {
            if (occupants == null) return false;
            foreach (var box in occupants)
            {
                if (!box || box.Remaining == 0 || box.InFlight > 0 || box.BlockedAtRevision != boardRevision)
                    return false;
            }
            return true;
        }

        private static TextMesh AddCounter(Transform box, int count, Font font)
        {
            var label = new GameObject("Count").AddComponent<TextMesh>();
            label.transform.SetParent(box, false);
            label.transform.localPosition = Vector3.up * 0.39f;
            label.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            label.anchor = TextAnchor.MiddleCenter;
            label.font = font;
            label.fontStyle = FontStyle.Bold;
            label.fontSize = 64;
            label.characterSize = 0.057f;
            label.color = new Color32(45, 37, 40, 255);
            label.text = count.ToString();
            label.GetComponent<MeshRenderer>().sharedMaterial = font.material;
            return label;
        }
    }
}
