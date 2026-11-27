using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodHunt
{
    [DisallowMultipleComponent]
    public sealed class AntColony : MonoBehaviour
    {
        [SerializeField] private GameObject workerModel;
        [SerializeField] private RuntimeAnimatorController walkController;
        [SerializeField] private Transform nest;

        private const int WorkersPerBox = 3;
        private const float Speed = 4.8f;
        private readonly Stack<Transform> idle = new Stack<Transform>();
        private HarvestBoard board;

        internal bool IsConfigured => workerModel && walkController && nest;
        internal void SetBoard(HarvestBoard value) => board = value;

        internal void BeginHarvest(HarvestBox box)
        {
            box.Open();
            StartCoroutine(Dispatch(box));
        }

        private IEnumerator Dispatch(HarvestBox box)
        {
            var delay = new WaitForSeconds(0.17f);
            var route = new List<Vector3>();
            while (box && box.Remaining > 0 && board.RemainingPixels > 0)
            {
                if (box.InFlight < Mathf.Min(WorkersPerBox, box.Remaining) && box.BlockedAtRevision != board.Revision)
                {
                    if (board.TryReserve(box.ColorId, box.transform.position, route, out int target))
                    {
                        box.InFlight++;
                        StartCoroutine(Collect(box, route, target));
                        route = new List<Vector3>();
                        yield return delay;
                        continue;
                    }
                    box.BlockedAtRevision = board.Revision;
                }
                yield return null;
            }
        }

        private Transform Spawn(HarvestBox box)
        {
            Transform ant;
            if (idle.Count > 0) ant = idle.Pop();
            else
            {
                ant = new GameObject("Worker").transform;
                ant.SetParent(transform, false);
                ant.localScale = Vector3.one * 0.52f;
                var model = Instantiate(workerModel, ant, false);
                var animator = model.GetComponentInChildren<Animator>();
                animator.runtimeAnimatorController = walkController;
                animator.applyRootMotion = false;
            }
            var renderer = ant.GetComponentInChildren<SkinnedMeshRenderer>(true);
            var materials = renderer.sharedMaterials;
            // Material slot 0 is the Colony Kit body; the other two are eyes and pupils.
            materials[0] = box.BodyColor;
            renderer.sharedMaterials = materials;
            ant.position = box.transform.TransformPoint(new Vector3(0f, 0.08f, 0f));
            ant.rotation = transform.rotation;
            ant.gameObject.SetActive(true);
            return ant;
        }

        private IEnumerator Collect(HarvestBox box, List<Vector3> route, int target)
        {
            Transform ant = Spawn(box);
            var mouth = box.transform.TransformPoint(new Vector3(0f, 0.32f, 0.34f));
            yield return Walk(ant, mouth);
            yield return Walk(ant, box.transform.TransformPoint(new Vector3(0f, -0.06f, 0.69f)));

            // Pass beside the nest on the outward trip. The central slot uses the right side.
            Vector3 nestPos = nest.position;
            float side = Vector3.Dot(ant.position - nestPos, transform.right);
            if (Mathf.Abs(side) < 0.95f)
                yield return Walk(ant, nestPos + transform.right * (side < 0f ? -0.95f : 0.95f));
            foreach (var point in route) yield return Walk(ant, point);

            var cargo = board.PickUp(target).transform;
            Vector3 facing = Vector3.ProjectOnPlane(cargo.position - ant.position, transform.up);
            if (facing.sqrMagnitude > 0.0001f) ant.rotation = Quaternion.LookRotation(facing, transform.up);
            cargo.SetParent(ant, true);
            cargo.localPosition = new Vector3(0f, 0.43f, 0.23f);
            for (int i = route.Count - 2; i >= 0; i--) yield return Walk(ant, route[i]);

            var approach = Vector3.ProjectOnPlane(ant.position - nestPos, transform.up).normalized;
            yield return Walk(ant, nestPos + approach * 0.62f);
            yield return Walk(ant, nestPos + approach * 0.24f);
            yield return Walk(ant, nestPos - transform.up * 0.55f);
            Destroy(cargo.gameObject);
            ant.gameObject.SetActive(false);
            idle.Push(ant);
            box.Deliver();
        }

        private IEnumerator Walk(Transform ant, Vector3 destination)
        {
            Vector3 direction = destination - ant.position;
            if (direction.sqrMagnitude < 0.000001f) yield break;
            var rotation = Quaternion.LookRotation(direction, transform.up);
            while (ant.position != destination)
            {
                ant.rotation = Quaternion.RotateTowards(ant.rotation, rotation, 850f * Time.deltaTime);
                ant.position = Vector3.MoveTowards(ant.position, destination, Speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
