using UnityEngine;

namespace FoodHunt
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class BoardCamera : MonoBehaviour
    {
        private Camera view;
        private Bounds boardBounds;
        private float framedAspect;

        public void Frame(Bounds bounds)
        {
            view = GetComponent<Camera>();
            boardBounds = bounds;
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
            float size = Mathf.Max(halfWidth / (0.88f * framedAspect), halfHeight / 0.44f);
            view.orthographicSize = size;
            float distance = ProjectedExtent(extent, transform.forward) + view.nearClipPlane + 2f;
            // Moving down by half the orthographic size places the board at viewport Y = 0.75.
            transform.position = boardBounds.center - transform.up * (0.5f * size) - transform.forward * distance;
        }

        private static float ProjectedExtent(Vector3 extent, Vector3 axis)
        {
            return extent.x * Mathf.Abs(axis.x) + extent.y * Mathf.Abs(axis.y) + extent.z * Mathf.Abs(axis.z);
        }
    }
}
