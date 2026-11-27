using UnityEngine;

namespace FoodHunt
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class BoardCamera : MonoBehaviour
    {
        private Camera view;
        private Bounds boardBounds;
        private Bounds levelBounds;
        private float framedAspect;

        public void Frame(Bounds bounds, Bounds playArea)
        {
            view = GetComponent<Camera>();
            boardBounds = bounds;
            levelBounds = playArea;
            view.orthographic = true;
            ApplyFraming();
        }

        private void LateUpdate()
        {
            if (view && !Mathf.Approximately(view.aspect, framedAspect))
                ApplyFraming();
        }

        private void ApplyFraming()
        {
            framedAspect = view.aspect;
            Vector3 extent = boardBounds.extents;
            float halfWidth = ProjectedExtent(extent, transform.right);
            float halfHeight = ProjectedExtent(extent, transform.up);
            // Fit the reference board (1052 x 928 on a 1190 x 2576 portrait screen).
            float size = Mathf.Max(halfWidth / (0.884f * framedAspect), halfHeight / 0.360f);
            view.orthographicSize = size;
            Vector3 offset = levelBounds.center - boardBounds.center;
            float nearest = ProjectedExtent(levelBounds.extents, transform.forward) - Vector3.Dot(offset, transform.forward);
            float distance = nearest + view.nearClipPlane + 2f;
            // Board center is 24.5% down from the top.
            transform.position = boardBounds.center - transform.up * (0.51f * size) - transform.forward * distance;
        }

        private static float ProjectedExtent(Vector3 extent, Vector3 axis)
        {
            return extent.x * Mathf.Abs(axis.x) + extent.y * Mathf.Abs(axis.y) + extent.z * Mathf.Abs(axis.z);
        }
    }
}
