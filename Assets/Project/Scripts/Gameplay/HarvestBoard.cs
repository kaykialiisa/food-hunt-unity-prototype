using System.Collections.Generic;
using UnityEngine;

namespace FoodHunt
{
    // One ring of empty cells represents the walkable space outside the picture.
    internal sealed class HarvestBoard
    {
        private readonly int width, height, stride;
        private readonly float spacing, surface;
        private readonly Transform grid;
        private readonly MeshRenderer[] pixels;
        private readonly int[] colors, previous, frontier;
        private readonly bool[] reserved;
        public int Revision { get; private set; }
        public int RemainingPixels { get; private set; }

        public HarvestBoard(int width, int height, float spacing, float surface, Transform grid)
        {
            this.width = width;
            this.height = height;
            this.spacing = spacing;
            this.surface = surface;
            this.grid = grid;
            stride = width + 2;
            int length = stride * (height + 2);
            pixels = new MeshRenderer[length];
            colors = new int[length];
            reserved = new bool[length];
            previous = new int[length];
            frontier = new int[length];
        }

        public void Add(int x, int row, int color, MeshRenderer voxel)
        {
            int cell = (row + 1) * stride + x + 1;
            pixels[cell] = voxel;
            colors[cell] = color;
            RemainingPixels++;
        }

        private Vector3 Position(int cell)
        {
            float x = (cell % stride - (width + 1) * 0.5f) * spacing;
            float z = (cell / stride - (height + 1) * 0.5f) * spacing;
            return grid.TransformPoint(new Vector3(x, surface, z));
        }

        public bool TryReserve(int color, Vector3 origin, List<Vector3> route, out int target)
        {
            float x = grid.InverseTransformPoint(origin).x / spacing;
            int entrance = Mathf.Clamp(Mathf.RoundToInt(x + (width + 1) * 0.5f), 0, stride - 1);
            System.Array.Fill(previous, -1);
            frontier[0] = entrance;
            previous[entrance] = entrance;
            int head = 0, tail = 1;
            while (head < tail)
            {
                int cell = frontier[head++];
                for (int direction = 0; direction < 4; direction++)
                {
                    int next;
                    switch (direction)
                    {
                        case 0: next = cell + stride; break;
                        case 1: if (cell % stride == 0) continue; next = cell - 1; break;
                        case 2: if (cell % stride == stride - 1) continue; next = cell + 1; break;
                        default: next = cell - stride; break;
                    }
                    if (next < 0 || next >= pixels.Length || previous[next] != -1) continue;
                    previous[next] = cell;
                    if (colors[next] == 0)
                    {
                        frontier[tail++] = next;
                        continue;
                    }
                    if (colors[next] != color || reserved[next]) continue;

                    target = next;
                    reserved[next] = true;
                    // Keep only corners of the path from the entrance to the pickup face.
                    route.Clear();
                    route.Add(Position(cell));
                    int lastStep = 0;
                    while (cell != entrance)
                    {
                        int step = cell - previous[cell];
                        if (lastStep != 0 && step != lastStep) route.Add(Position(cell));
                        cell = previous[cell];
                        lastStep = step;
                    }
                    if (route.Count > 1 || route[0] != Position(entrance)) route.Add(Position(entrance));
                    route.Reverse();
                    return true;
                }
            }
            target = -1;
            return false;
        }

        public MeshRenderer PickUp(int cell)
        {
            var voxel = pixels[cell];
            pixels[cell] = null;
            colors[cell] = 0;
            reserved[cell] = false;
            RemainingPixels--;
            Revision++;
            return voxel;
        }
    }
}
