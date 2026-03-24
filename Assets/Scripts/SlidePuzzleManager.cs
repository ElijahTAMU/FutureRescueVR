using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SlidePuzzleManager : MonoBehaviour
{
    public int size = 3;

    private PuzzleTile[,] grid;
    private Vector2Int emptyPos;
    public Vector3[,] slotPositions;

    public GameObject puzzleBase;

    private void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new PuzzleTile[size, size];
        slotPositions = new Vector3[size, size];

        PuzzleTile[] tiles = puzzleBase.GetComponentsInChildren<PuzzleTile>();

        List<Vector3> positions = tiles.Select(t => t.transform.localPosition).ToList();

        float minX = positions.Min(p => p.x);
        float maxX = positions.Max(p => p.x);
        float minZ = positions.Min(p => p.z);
        float maxZ = positions.Max(p => p.z);

        float stepX = (maxX - minX) / (size - 1);
        float stepZ = (maxZ - minZ) / (size - 1);

        // Generate slot positions
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                slotPositions[x, y] = new Vector3(
                    minX + x * stepX,
                    0,
                    maxZ - y * stepZ
                );
            }
        }

        // Assign tiles to grid using direct calculation (FIXED)
        foreach (var tile in tiles)
        {
            Vector3 pos = tile.transform.localPosition;

            int x = Mathf.RoundToInt((pos.x - minX) / stepX);
            int y = Mathf.RoundToInt((maxZ - pos.z) / stepZ);

            // Clamp to avoid out of bounds due to float precision
            x = Mathf.Clamp(x, 0, size - 1);
            y = Mathf.Clamp(y, 0, size - 1);

            Vector2Int gridPos = new Vector2Int(x, y);

            tile.gridPos = gridPos;
            tile.manager = this;

            if (grid[x, y] != null)
            {
                Debug.LogWarning($"Slot already occupied at {gridPos} by {grid[x, y].name}. Overwriting with {tile.name}");
            }

            grid[x, y] = tile;

            Debug.Log($"Tile {tile.name} -> Grid {gridPos} | Pos {pos}");
        }

        // Find empty slot
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] == null)
                {
                    emptyPos = new Vector2Int(x, y);
                    Debug.Log($"EMPTY SLOT FOUND at {emptyPos} | World Pos {slotPositions[x, y]}");
                }
            }
        }
    }

    public bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    public bool IsValidMove(PuzzleTile tile)
    {
        return IsAdjacent(tile.gridPos, emptyPos);
    }

    public void TryMove(PuzzleTile tile)
    {
        if (!IsValidMove(tile)) return;

        Vector2Int oldPos = tile.gridPos;

        grid[emptyPos.x, emptyPos.y] = tile;
        grid[oldPos.x, oldPos.y] = null;

        tile.gridPos = emptyPos;
        emptyPos = oldPos;

        StartCoroutine(SmoothMove(tile, slotPositions[tile.gridPos.x, tile.gridPos.y]));
    }

    IEnumerator SmoothMove(PuzzleTile tile, Vector3 target)
    {
        target.y = 1;

        float time = 0f;
        Vector3 start = tile.transform.localPosition;

        while (time < 1f)
        {
            time += Time.deltaTime * 6f;
            tile.transform.localPosition = Vector3.Lerp(start, target, time);
            yield return null;
        }

        tile.transform.localPosition = target;
    }
}