using System;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class SlidePuzzle : MonoBehaviour
{
    public GameObject[] BigCells;

    public GameObject[] SmallCells;

    public CellInfo[][] grid = new CellInfo[3][];

    public CellInfo[] row1;
    public CellInfo[] row2;
    public CellInfo[] row3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        grid[0] = new CellInfo[3];
        grid[1] = new CellInfo[3];
        grid[2] = new CellInfo[3];

        for (int i = 0; i < 3; i++)
        {
            grid[0][i] = new CellInfo();
            grid[1][i] = new CellInfo();
            grid[2][i] = new CellInfo();
        }

        for (int i = 0; i < SmallCells.Length; i++)
        {
            if (i < 2)
            {
                CellInfo ci = grid[0][i];
                ci.Cell = SmallCells[i];
                ci.minX_Y = ci.Cell.transform.localPosition.x;
                ci.maxX_Y = ci.Cell.transform.localPosition.x;
                ci.minZ_X = ci.Cell.transform.localPosition.z;
                ci.maxZ_X = ci.Cell.transform.localPosition.z;
                ci.anchorZ = ci.Cell.transform.localPosition.z;
                ci.anchorX = ci.Cell.transform.localPosition.x;

                ci.Cell.GetComponent<Cell>().cellInfo = ci;
                Debug.Log(SmallCells[i] + " at 0, " + i);
            }
            else if (i < 5)
            {
                CellInfo ci = grid[1][i - 2];
                ci.Cell = SmallCells[i];
                ci.minX_Y = ci.Cell.transform.localPosition.x;
                ci.maxX_Y = ci.Cell.transform.localPosition.x;
                ci.minZ_X = ci.Cell.transform.localPosition.z;
                ci.maxZ_X = ci.Cell.transform.localPosition.z;
                ci.anchorZ = ci.Cell.transform.localPosition.z;
                ci.anchorX = ci.Cell.transform.localPosition.x;

                ci.Cell.GetComponent<Cell>().cellInfo = ci;
                Debug.Log(SmallCells[i] + " at 1, " + (i - 2));

            }
            else
            {
                CellInfo ci = grid[2][i - 5];
                ci.Cell = SmallCells[i];
                ci.minX_Y = ci.Cell.transform.localPosition.x;
                ci.maxX_Y = ci.Cell.transform.localPosition.x;
                ci.minZ_X = ci.Cell.transform.localPosition.z;
                ci.maxZ_X = ci.Cell.transform.localPosition.z;
                ci.anchorZ = ci.Cell.transform.localPosition.z;
                ci.anchorX = ci.Cell.transform.localPosition.x;

                ci.Cell.GetComponent<Cell>().cellInfo = ci;
                Debug.Log(SmallCells[i] + " at 2, " + (i - 5));

            }
        }

        Debug.Log(grid[0][2] + " is the empty cell");

        grid[0][2].hasCell = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int r = Random.Range(0, BigCells.Length);
        MoveBigCellToMatch(SmallCells[r]);

        SetCellLocks();
        RepositionSmallCells();

        row1 = grid[0];
        row2 = grid[1];
        row3 = grid[2];
}

    public void MoveBigCellToMatch(GameObject chosenSmallCell)
    {
        for (int i = 0; i < SmallCells.Length; i++)
        {
            if (SmallCells[i] == chosenSmallCell)
            {
                Vector3 smallPosition = SmallCells[i].transform.localPosition;

                Vector3 newBigPosition = new Vector3(smallPosition.x * (5 / 0.3f), 2.5f, smallPosition.z * (5 / 0.3f));

                BigCells[i].transform.localPosition = newBigPosition;
            }
        }
    }

    public void SetCellLocks()
    {
        foreach (CellInfo[] cellRow in grid)
        {
            foreach (CellInfo cell in cellRow)
            {
                cell.Locked = true;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[i][j].hasCell == false)
                {
                    int left = j - 1;
                    int right = j + 1;
                    int up = i - 1;
                    int down = i + 1;

                    if (left >= 0)
                    {
                        CellInfo cell = grid[left][j];
                        cell.Locked = false;
                        cell.minZ_X = cell.anchorZ;
                        cell.maxZ_X = cell.anchorZ;
                        cell.minX_Y = cell.anchorX - 0.3f;
                        cell.maxX_Y = cell.anchorX;

                    }

                    if (right <= 2)
                    {
                        CellInfo cell = grid[right][j];
                        cell.Locked = false;
                        cell.minZ_X = cell.anchorZ;
                        cell.maxZ_X = cell.anchorZ;
                        cell.minX_Y = cell.anchorX;
                        cell.maxX_Y = cell.anchorX + 0.3f;
                    }

                    if (up >= 0)
                    {
                        CellInfo cell = grid[i][up];
                        cell.Locked = false;
                        cell.minZ_X = cell.anchorZ - 0.3f;
                        cell.maxZ_X = cell.anchorZ;
                        cell.minX_Y = cell.anchorX;
                        cell.maxX_Y = cell.anchorX;
                    }

                    if (down <= 2)
                    {
                        CellInfo cell = grid[i][down];
                        cell.Locked = false;
                        cell.minZ_X = cell.anchorZ;
                        cell.maxZ_X = cell.anchorZ + 0.3f;
                        cell.minX_Y = cell.anchorX;
                        cell.maxX_Y = cell.anchorX;
                    }
                }

            }
        }
    }

    public void RepositionSmallCells()
    {
        for(int j = 0; j < 3; j++)
        {
            for(int i = 0; i < 3; i++)
            {
                if ((grid[i][j].Cell != null) && grid[i][j] != null)
                {
                    CellInfo cell = grid[i][j];
                    Vector3 pos = cell.Cell.gameObject.transform.localPosition;
                    cell.Cell.transform.localPosition = new Vector3(Mathf.Clamp(pos.x, cell.minX_Y, cell.maxX_Y), 0, Mathf.Clamp(pos.z, cell.minZ_X, cell.maxZ_X));
                }
            }
        }
    } 
}

[Serializable]
public class CellInfo
{
    public GameObject Cell = new GameObject();
    public bool hasCell = true;
    public bool Locked;

    public float anchorZ;
    public float anchorX;

    public float minZ_X;
    public float minX_Y;
    public float maxZ_X;
    public float maxX_Y;
}