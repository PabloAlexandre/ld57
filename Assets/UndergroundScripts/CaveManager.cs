using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CaveManager : MonoBehaviour {

    public GameObject cellObject;
    public int GridSize = 4;
    public int Interval = 1;
    public CaveCell[] mazeSolved;
    public CellUnity[][] nodes;
    public Vector3 cellSize = new Vector3(1, 1, 1);
    public int minSteps = 5;
    public int numberOfSpawnPoints = 5;
    public int minHiddenSpots = 3;

    public GameManager manager;
    public int level = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        //CaveGenerator cave = new CaveGenerator(GridSize, 1, new CaveConditions() {
        //    minSteps = this.minSteps,
        //    minHiddenSpots = this.minHiddenSpots,
        //});

        //this.mazeSolved = cave.GenerateCaves();

        manager = GetComponent<GameManager>();
        LevelGame? currentLevel = manager.LoadLevel(level);

        if(currentLevel == null) {
            Debug.LogError($"Level {level} not found");
            return;
        }

        CaveCell[][] map = currentLevel.Value.cells;
        this.mazeSolved = currentLevel.Value.solvedPath;
        Debug.Log($"Maze solved: {this.mazeSolved.Length}");

        nodes = new CellUnity[GridSize][];
        for (int i = 0; i < GridSize; i++) {
            nodes[i] = new CellUnity[GridSize];
        }

        for (int i = 0; i < GridSize; i++) {
            for (int j = 0; j < GridSize; j++) {
                Vector3 cellPosition = new Vector3(i * ((Interval + 1) * cellSize.x), cellSize.y, j * ((Interval + 1) * cellSize.z));
                GameObject cell = Instantiate(cellObject, Vector3.zero, Quaternion.identity);
                cell.name = $"Cell_{i}_{j}";
                cell.transform.parent = transform;
                cell.transform.localPosition = cellPosition;
                //cell.transform.localRotation = Quaternion.identity;

                CellUnity cellUnity = cell.GetComponent<CellUnity>();
                cellUnity.walls = map[i][j].walls;
                cellUnity.isPath = map[i][j].isPath;
                cellUnity.spawnType = map[i][j].spawnType;
                cellUnity.Initialize(i, j, CellType.PATH, false, map[i][j]);
                nodes[i][j] = cellUnity;

                for (int interval = 0; interval < Interval + 1; interval++) {
                    for(int interval2 = 0; interval2 < Interval + 1; interval2++) {
                        if(interval == 0 && interval2 == 0) continue;
                        if((i + 1 == GridSize && interval2 != 0) || (j + 1 == GridSize && interval != 0)) {
                            break;
                        }

                        int x = interval2 + i;
                        int y = interval + j;

                        if(i < GridSize && j < GridSize) {
                            Vector3 intermediareCellPosition = new Vector3(cellPosition.x + (interval2 * cellSize.x), cellSize.y, cellPosition.z + (interval * cellSize.z));
                            GameObject intermediareCell = Instantiate(cellObject, intermediareCellPosition, Quaternion.identity);
                            intermediareCell.transform.parent = transform;
                            intermediareCell.transform.localPosition = intermediareCellPosition;
                            //intermediareCell.transform.localRotation = Quaternion.identity;

                            CellUnity intermediareCellUnity = intermediareCell.GetComponent<CellUnity>();
                            intermediareCellUnity.owner = cellUnity;
                            intermediareCell.name = $"IntermediareCell_{i}_{j}_{interval}_{interval2}";
                            //intermediareCell.transform.Find("bg").GetComponent<Renderer>().material.color = Color.black;
                            intermediareCellUnity.Initialize(x, y, CellType.STONE, true);

                            IntermediateBehavior beh = intermediareCell.AddComponent<IntermediateBehavior>();
                            beh.Initialize(intermediareCellUnity);
                        }
                    }
                }
            }
        }
    }

    void Update() {

    }

    private void OnDrawGizmosSelected() {
        if (this.mazeSolved == null || this.nodes == null) return;
        for (int i = 0; i < this.mazeSolved.Length - 1; i++) {
            int next = i + 1;

            CaveCell current = this.mazeSolved[i];
            CaveCell nextCell = this.mazeSolved[next];

            Vector3 start = this.nodes[current.x][current.y].transform.position;
            Vector3 end = this.nodes[nextCell.x][nextCell.y].transform.position;
            float thickness = 3;

            Handles.DrawBezier(start, end, start, end, Color.green, null, thickness);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(start, 5.25f);
        }



        for(int x = 0; x < GridSize; x++) {
            for (int y = 0; y < GridSize; y++) {
                if (this.nodes[x][y].spawnType != SpawnType.NONE) {
                    Vector3 center = this.nodes[x][y].transform.position;

                    switch(this.nodes[x][y].spawnType) {
                        case SpawnType.BLUE_ITEM:
                            Gizmos.color = Color.blue;
                            break;
                        case SpawnType.RED_ITEM:
                            Gizmos.color = Color.red;
                            break;
                        default:
                            Gizmos.color = Color.yellow;
                            break;
                    }
                    Gizmos.DrawSphere(center, 8.25f);

                }
            }
        }
    }
}
