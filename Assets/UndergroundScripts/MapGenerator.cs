//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor.Callbacks;
//using UnityEngine;
//using UnityEngine.UIElements;

//public class MapGenerator : MonoBehaviour {
//    public int GridSize;
//    public GameObject obj;
//    public float offset = 0.5f;
//    public float margin = 2f;
//    private MazeManager mazeManager;
//    void Start() {
//        mazeManager = new MazeManager(GridSize);
       
//        //mazeManager.generateMaze();
//        //mazeManager.solveMaze();

//        List<MapCell> path = new List<MapCell>(mazeManager.initMaze().Where((mp) => mp.isPath));

//        for (int i = 0; i < GridSize; i++) {
//            float nextI = i + 1;
//            for (int j = 0; j < GridSize; j++) {
//                MapCell cell = mazeManager.map[i][j];
//                Vector3 position = new Vector3(i*(margin), 0, (j* (margin)));
//                GameObject cellObject = CreateCellInstance(position, $"Cell_{i}_{j}", cell.walls, cell.isPath ? Color.gray : Color.red);
//                cell.obj = cellObject;

//                if(j < GridSize -1) {
//                    Color color = Color.yellow;

//                    if (cell.walls[((int)Walls.Down)]) {
//                        color = Color.magenta;    
//                    } else {
//                        color = Color.blue;
//                    }
                    
//                    Vector3 nextPosition = new Vector3(i * (margin), 0, (j + 1) * (margin));
//                    Vector3 connectorPosition = Vector3.Lerp(position, nextPosition, 0.5f);

//                    GameObject connector = CreateCellInstance(connectorPosition, $"Connector_{i}_{j}", new bool[4], color);
//                }

//                if(i < GridSize - 1) {
//                    Color color = Color.yellow;

//                    if (cell.walls[((int)Walls.Right)]) {
//                        color = Color.magenta;
//                    } else {
//                        color = Color.blue;
//                    }

//                        Vector3 nextPosition = new Vector3((i + 1) * (margin), 0, j * (margin));
//                    Vector3 connectorPosition = Vector3.Lerp(position, nextPosition, 0.5f);
//                    GameObject connector = CreateCellInstance(connectorPosition, $"Connector_{i}_{j}", new bool[4], color);
//                }

//                if(j < GridSize -1 && i < GridSize - 1) {
//                    Vector3 nextPosition = new Vector3((i + 1) * (margin), 0, (j + 1) * (margin));
//                    Vector3 connectorPosition = Vector3.Lerp(position, nextPosition, 0.5f);
//                    GameObject connector = CreateCellInstance(connectorPosition, $"Connector_{i}_{j}", new bool[4], Color.magenta);
//                }

//                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//                sphere.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
//                sphere.transform.parent = cellObject.transform;
//                sphere.transform.localPosition = Vector3.zero;
//                sphere.name = "Sphere";

//                Material mat = cellObject.GetComponent<Renderer>().material;
//                mat.color = Color.blue; 
//            }
//        }

        

//        int threeshould = path.Count / 6;
//        List<MapCell> pathList = new List<MapCell>(path.Where((mp) => mp.isPath));
//        int total = pathList.Count - (threeshould * 2);

//        Debug.Log($"{total} Total | count {pathList.Count}");
        
//        foreach (MapCell cell in path) {
//            GameObject sphere = cell.obj.transform.Find("Sphere").gameObject;
//            if (path.IndexOf(cell) < threeshould || path.IndexOf(cell) > path.Count - threeshould) {
//                sphere.GetComponent<Renderer>().material.color = new Color(0, 0.7f, 0);
//                pathList.Remove(cell);
//            } else {
//                sphere.GetComponent<Renderer>().material.color = Color.green;
//                pathList.Add(cell);
//            }
//        }

//        int randomItems = Mathf.Max(3, Mathf.Min(8, total / 4));
//        Debug.Log($"{randomItems} random items | {total} total items");
//        int[] randomIndex = new int[total];

//        for (int i = 0; i < total; i++) {
//            randomIndex[i] = i;
//        }
//            for (int i = 0; i < total; i++) {
//            int rand = UnityEngine.Random.Range(i, randomIndex.Length);
//            int temp = randomIndex[i];
//            randomIndex[i] = randomIndex[rand];
//            randomIndex[rand] = temp;
//        }

//        for(int i = 0; i < randomItems; ++i) {
//            MapCell cell = pathList[randomIndex[i]];
//            GameObject sphere = cell.obj.transform.Find("Sphere").gameObject;
//            sphere.GetComponent<Renderer>().material.color = Color.blue;
//        }
//        Debug.Log(total);

//    }
//    [ContextMenu("Generate Cave")]
//    private void UpdateCave() {
//        mazeManager = new MazeManager(GridSize);
//        mazeManager.initMaze();
//    }

//    public GameObject CreateCellInstance(Vector3 position, string name, bool[] walls, Color? color = null) {
//        GameObject cellObject = Instantiate(obj, position, Quaternion.identity);
//        cellObject.name = name;

//        string[] names = new string[] { "bottom", "right", "top", "left" };

//        for (int k = 0; k < walls.Length; k++) {
//            if (walls[k]) {
//                cellObject.transform.Find(names[k]).gameObject.SetActive(true);
//            }
//        }

//        Material mat = cellObject.GetComponent<Renderer>().material;
//        mat.color = color.GetValueOrDefault(Color.gray);

//        return cellObject;
//    }
//}

//public class MazeManager {
//    public MapCell[][] map;
//    protected int gridSize = 8;

//    private MapCell currentCell;
//    private Stack<MapCell> cellStack = new Stack<MapCell>();
//    public bool solved;

//    public MazeManager(int gridSize = 8) {
//        this.gridSize = gridSize;

//        map = new MapCell[gridSize][];
//        for (int i = 0; i < gridSize; i++) {
//            map[i] = new MapCell[gridSize];
//            for (int j = 0; j < gridSize; j++) {
//                map[i][j] = new MapCell(i, j);
//            }
//        }
//    }

//    void cleanupMaze() {
//        this.cellStack.Clear();

//        for (int i = 0; i < gridSize; i++) {
//            for (int j = 0; j < gridSize; j++) {
//                map[i][j].reset();
//            }
//        }
//    }

//    public MapCell[] initMaze() {
//        this.solved = false;

//        //while (!this.solved) {
//        this.cleanupMaze();

//        this.currentCell = map[0][0];
//            this.currentCell.visited = true;
//            this.cellStack.Push(this.currentCell);

//            this.generateMaze();
//           return this.solveMaze();
//        //}
//    }

//    public void generateMaze() {
//        while (cellStack.Count > 0) {
//            this.currentCell = cellStack.Pop();
//            cellStack.Push(this.currentCell);

//            CaveCellNeighbor[] neighbors = getNeighbors(currentCell);

//            if (neighbors.Length > 0) {
//                int randIndex = UnityEngine.Random.Range(0, neighbors.Length);
//                CaveCellNeighbor neighbor = neighbors[randIndex];

                
//                this.currentCell.removeWall(neighbor.wall);
//                neighbor.cell.removeOppositeWall(neighbor.wall);

//                neighbor.cell.visited = true;

//                cellStack.Push(neighbor.cell);
//            } else {
//                cellStack.Pop();
//            }
//        }
//    }

//    public MapCell[] solveMaze() {
//        MapCell start = map[0][0];
//        MapCell end = map[gridSize - 1][gridSize - 1];
//        HashSet<MapCell> visited = new HashSet<MapCell>();
//        this.solved = findPath(start, end, ref visited);
//        return visited.ToArray();
//    }

//    public bool findPath(MapCell current, MapCell end, ref HashSet<MapCell> visited) {

//        if (current == null || end == null) {
//            return false;
//        }

//        if(current == end) {
//            current.isPath = true;
//            return true;
//        }

//        visited.Add(current);
//        int[][] dirs = new int[][] {
//            new int[] { 0, -1 }, // up
//            new int[] { 1, 0 }, // right
//            new int[] { 0, 1 }, // down
//            new int[] { -1, 0 } // left
//        };

//        for(int i = 0; i < dirs.Length; i++) {
//            if(current.walls[i]) {
//                continue;
//            }

//            int x = current.x + dirs[i][0];
//            int y = current.y + dirs[i][1];
//            if (x >= 0 && x < this.gridSize && y >= 0 && y < this.gridSize) {
//                MapCell neighbor = map[x][y];
//                if (!visited.Contains(neighbor) && this.findPath(neighbor, end, ref visited)) {
//                    current.isPath = true;
//                    return true;
//                }
//            }
//        }

//        return false;
//    }

//    public CaveCellNeighbor[] getNeighbors(MapCell cell) {
//        int[][] dirs = new int[][] {
//            new int[] { 0, -1 }, // up
//            new int[] { 1, 0 }, // right
//            new int[] { 0, 1 }, // down
//            new int[] { -1, 0 } // left
//        };

//        List<CaveCellNeighbor> neighbors = new List<CaveCellNeighbor>();

//        for (int i = 0; i < dirs.Length; i++) {
//            int x = cell.x + dirs[i][0];
//            int y = cell.y + dirs[i][1];
//            if (x >= 0 && x < this.gridSize && y >= 0 && y < this.gridSize && map[x][y].visited == false) {
//                CaveCellNeighbor neighbor = new CaveCellNeighbor() {
//                    cell = map[x][y],
//                    wall = i,
//                };

//                neighbors.Add(neighbor);
//            }
//        }

//        return neighbors.ToArray();
//    }
//}
////public enum Walls {
////    Up = 0,
////    Right = 1,
////    Down = 2,
////    Left = 3
////}
////public struct CaveCellNeighbor {
////    public MapCell cell;
////    public int wall;
////}

////public class MapCell {
////    public int x;
////    public int y;
////    public GameObject obj;

////    public bool[] walls = new bool[4] {
////        true, true, true, true
////    };

////    public bool visited = false;
////    public bool isPath = false;

////    public MapCell(int x, int y) {
////        this.x = x;
////        this.y = y;
////    }

////    public void reset() {
////        this.visited = false;
////        this.isPath = false;
////        this.walls = new bool[] { true, true, true, true };
////    }

////    public void removeWall(int wallIndex) {
////        if (wallIndex >= 0 && wallIndex < walls.Length) {
////            walls[wallIndex] = false;
////        }
////    }

////    public void removeOppositeWall(int wallIndex) {
////        int[] opposite = new int[] { 2, 3, 0, 1 };
////        walls[opposite[wallIndex]] = false;
////    }
////}