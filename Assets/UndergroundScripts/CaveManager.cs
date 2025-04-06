using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class CaveManager : MonoBehaviour {

    public GameObject cellObject;
    public int GridSize = 4;
    public int Interval = 1;
    public MapCell[] mazeSolved;
    public CellUnity[][] nodes;
    public Vector3 cellSize = new Vector3(1, 1, 1);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        MazeManager mazeManager = new MazeManager(GridSize);
        mazeManager.initMaze();

        List<MapCell> tmp = new List<MapCell>();
        for (int i = 0; i < GridSize; i++) {
            for (int j = 0; j < GridSize; j++) {
                if(mazeManager.map[i][j].isPath) {
                    tmp.Add(mazeManager.map[i][j]);
                }
            }
        }

        this.mazeSolved = tmp.ToArray();

       ;
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
                cellUnity.walls = mazeManager.map[i][j].walls;
                cellUnity.isPath = mazeManager.map[i][j].isPath;
                cellUnity.Initialize(i, j, CellType.PATH);
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
        if(this.mazeSolved == null) return;
        for (int i = 0; i < this.mazeSolved.Length - 1; i++) {
            int next = i + 1;

            MapCell current = this.mazeSolved[i];
            MapCell nextCell = this.mazeSolved[next];

            Vector3 start = this.nodes[current.x][current.y].transform.position;
            Vector3 end = this.nodes[nextCell.x][nextCell.y].transform.position;
            float thickness = 3;

            Handles.DrawBezier(start, end, start, end, Color.red, null, thickness);
        }
    }
}
