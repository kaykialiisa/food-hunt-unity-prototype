using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoodHunt
{
    [DisallowMultipleComponent]
    public sealed class LevelLoader : MonoBehaviour
    {
        [Serializable]
        private sealed class LevelData
        {
            public int width;
            public int height;
            public int[] cells;
        }

        [Serializable]
        private struct PaletteEntry
        {
            public int id;
            public Material material;
        }

        [SerializeField] private TextAsset levelFile;
        [SerializeField] private MeshRenderer voxelModel;
        [SerializeField] private MeshRenderer boardModel;
        [SerializeField] private Transform voxels;
        [SerializeField] private PaletteEntry[] palette;
        [SerializeField] private BoardCamera boardCamera;

        private const float CellSpacing = 0.21f;
        // Dimensions of the Colony Kit board's inset and its upper surface.
        private const float InsetWidth = 5.30f;
        private const float SurfaceHeight = 0.035f;
        private const float InsetMargin = 0.08f;

        private void Start()
        {
            if (!levelFile || !voxelModel || !boardModel || !voxels || !boardCamera)
            {
                Debug.LogError("Assign the level file, scene board, voxel model, voxel parent and camera before loading.", this);
                return;
            }

            LevelData level;
            try
            {
                level = JsonUtility.FromJson<LevelData>(levelFile.text);
            }
            catch (ArgumentException exception)
            {
                Debug.LogError($"Invalid level JSON: {exception.Message}", this);
                return;
            }

            if (level == null || level.width <= 0 || level.height <= 0 ||
                level.cells == null || level.cells.LongLength != (long)level.width * level.height)
            {
                Debug.LogError("Level dimensions must match the number of cells.", this);
                return;
            }

            var materials = new Dictionary<int, Material>();
            foreach (var entry in palette ?? Array.Empty<PaletteEntry>())
            {
                if (entry.id <= 0 || !entry.material || !materials.TryAdd(entry.id, entry.material))
                {
                    Debug.LogError($"Invalid or duplicate palette entry: {entry.id}.", this);
                    return;
                }
            }

            foreach (int id in level.cells)
            {
                if (id != 0 && !materials.ContainsKey(id))
                {
                    Debug.LogError($"Level uses unmapped color ID {id}.", this);
                    return;
                }
            }

            // Size the scene board in level axes, preserving the FBX's imported transform.
            boardModel.transform.parent.localScale = new Vector3(
                (level.width * CellSpacing + 2 * InsetMargin) / InsetWidth, 1f,
                (level.height * CellSpacing + 2 * InsetMargin) / InsetWidth);
            var bounds = boardModel.bounds;

            for (int index = 0; index < level.cells.Length; index++)
            {
                int id = level.cells[index];
                if (id == 0) continue;

                int column = index % level.width;
                int row = index / level.width;
                MeshRenderer voxel = Instantiate(voxelModel, voxels, false);
                voxel.name = $"Voxel_{column}_{row}";
                voxel.transform.localPosition = new Vector3(
                    (column - (level.width - 1) * 0.5f) * CellSpacing, SurfaceHeight,
                    (row - (level.height - 1) * 0.5f) * CellSpacing);
                voxel.sharedMaterial = materials[id];
                bounds.Encapsulate(voxel.bounds);
            }

            boardCamera.Frame(bounds);
        }
    }
}
