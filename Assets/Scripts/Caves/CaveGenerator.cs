using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveGenerator {
    int gridSize;
    int spawnThreeshold;
    CaveOptions options;

    public CaveCell[][] baseMap;
    private CaveCell currentCell;
    private Stack<CaveCell> cellStack = new Stack<CaveCell>();
    public bool solved;

    public CaveGenerator(int gridSize, int spawnThreeshold = 1, CaveOptions opts = new CaveOptions()) {
        this.gridSize = gridSize;
        this.spawnThreeshold = spawnThreeshold;
        this.options = opts;

        baseMap = new CaveCell[gridSize][];
        for (int x = 0; x < gridSize; x++) {
            baseMap[x] = new CaveCell[gridSize];
            for (int y = 0; y < gridSize; y++) {
                baseMap[x][y] = new CaveCell(x, y);
            }
        }
    }

    void CleanupCaves() {
        this.cellStack.Clear();

        for (int i = 0; i < this.gridSize; i++) {
            for (int j = 0; j < this.gridSize; j++) {
                baseMap[i][j].reset();
            }
        }
    }

    public CaveCell[] GenerateCaves(int count = 0) {
        this.solved = false;
        this.CleanupCaves();

        this.currentCell = this.baseMap[0][0];
        this.currentCell.visited = true;
        this.cellStack.Push(this.currentCell);

        // Fill Maze
        while (cellStack.Count > 0) {
            this.currentCell = cellStack.Pop();
            cellStack.Push(this.currentCell);

            CaveCellNeighbor[] neighbors = this.GetCellNeighbors(currentCell);

            if (neighbors.Length > 0) {
                int randIndex = UnityEngine.Random.Range(0, neighbors.Length);
                CaveCellNeighbor neighbor = neighbors[randIndex];


                this.currentCell.removeWall(neighbor.wall);
                neighbor.cell.removeOppositeWall(neighbor.wall);
                neighbor.cell.visited = true;

                cellStack.Push(neighbor.cell);
            } else {
                cellStack.Pop();
            }
        }

        CaveCell[] path = this.SolveMaze();

        if(this.solved) {

            bool hasConditions = new bool[] {
                this.options.conditions.Value.minSteps.HasValue ? path.Length >= this.options.conditions.Value.minSteps.Value : true,
                this.options.conditions.Value.minHiddenSpots.HasValue ? (gridSize * gridSize) - path.Length >= this.options.conditions.Value.minHiddenSpots.Value : true,
            }.All(it => it == true);


            if (hasConditions) {
                return this.AddSpawnPoints(path);
            }
        }
        

        if (count < 10) {
            Debug.LogWarning($"Cave not solved: Trying again {count + 1}/10");
            return this.GenerateCaves(count + 1);
        } 


        Debug.LogError($"Cave not solved");
        return new CaveCell[0];
    }

    public CaveCell[] AddSpawnPoints(CaveCell[] mainPoints) {
        // Create Points in Path

        // ignore start and end points between threeshold
        CaveCell[] relevantPoints = mainPoints
            .Skip(this.spawnThreeshold)
            .Take(mainPoints.Length - (2 * this.spawnThreeshold))
            .ToArray();

        // shuffle points
        relevantPoints = this.randomizeArray(relevantPoints);

        Debug.Log($"Relevant points: {relevantPoints.Length} - percentage : {Mathf.FloorToInt(this.options.percentageOfSpawnsInPath * relevantPoints.Length)}");
        // add spawn points
        int spawnItems = Mathf.FloorToInt(this.options.percentageOfSpawnsInPath * relevantPoints.Length);
        for (int i = 0; i < spawnItems; i++) {
            if(i >= relevantPoints.Length) break;
            // find mainPoints index of this point
            int index = Array.IndexOf(mainPoints, relevantPoints[i]);
            if(index < 0) continue;
            mainPoints[index].SetSpawnType(SpawnType.BLUE_ITEM);
            this.baseMap[mainPoints[index].x][mainPoints[index].y].SetSpawnType(SpawnType.BLUE_ITEM);
        }

        // add hidden points
        List<CaveCell> hiddenPoints = new List<CaveCell>();

        for (int x = 0; x < this.gridSize; x++) {
            for (int y = 0; y < this.gridSize; y++) {
                if (baseMap[x][y].isPath) continue;
                hiddenPoints.Add(baseMap[x][y]);
            }
        }

        int hiddenItems = this.options.numberOfFishs;
        List<CaveCell> selectedHidden = CaveUtils.PickRandomWithDistance(hiddenPoints, hiddenItems, 3);

        foreach(CaveCell cell in selectedHidden) {
            this.baseMap[cell.x][cell.y].SetSpawnType(SpawnType.RED_ITEM);
        }

        return mainPoints;
    }

    T[] randomizeArray<T>(T[] array) {
        for (int i = 0; i < array.Length; i++) {
            int randomIndex = UnityEngine.Random.Range(i, array.Length);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
        return array;
    }

    CaveCell[] SolveMaze() {
        Vector2 startIndex = this.options.startCell.GetValueOrDefault(Vector2.zero);
        Vector2 endIndex = this.options.endCell.GetValueOrDefault(new Vector2(UnityEngine.Random.Range(0, gridSize), this.gridSize - 1));

        CaveCell start = baseMap[(int)startIndex.x][(int)startIndex.y];
        CaveCell end = baseMap[(int) endIndex.x][(int) endIndex.y];

        start.isStart = true;
        end.isEnd = true;

        HashSet<CaveCell> visited = new HashSet<CaveCell>();

        this.solved = this.FindPath(start, end, ref visited);
        if(this.solved) return visited.Where(it => it.isPath).ToArray();

        return new CaveCell[0];
    }

    bool FindPath(CaveCell current, CaveCell end, ref HashSet<CaveCell> visited) {

        if (current == null || end == null) {
            return false;
        }

        if (current == end) {
            current.isPath = true;
            visited.Add(current);
            return true;
        }

        visited.Add(current);
        int[][] dirs = Constants.WALL_DIRECTIONS;

        for (int i = 0; i < dirs.Length; i++) {
            if (current.walls[i]) {
                continue;
            }

            int x = current.x + dirs[i][0];
            int y = current.y + dirs[i][1];
            if (x >= 0 && x < this.gridSize && y >= 0 && y < this.gridSize) {
                CaveCell neighbor = baseMap[x][y];
                if (!visited.Contains(neighbor) && this.FindPath(neighbor, end, ref visited)) {
                    current.isPath = true;
                    return true;
                }
            }
        }

        return false;
    }

    CaveCellNeighbor[] GetCellNeighbors(CaveCell cell) {
        int[][] dirs = Constants.WALL_DIRECTIONS;

        List<CaveCellNeighbor> neighbors = new List<CaveCellNeighbor>();

        for (int i = 0; i < dirs.Length; i++) {
            int x = cell.x + dirs[i][0];
            int y = cell.y + dirs[i][1];
            if (x >= 0 && x < this.gridSize && y >= 0 && y < this.gridSize && baseMap[x][y].visited == false) {
                CaveCellNeighbor neighbor = new CaveCellNeighbor() {
                    cell = baseMap[x][y],
                    wall = i
                };

                neighbors.Add(neighbor);
            }
        }

        return neighbors.ToArray();
    }
}

public struct CaveConditions {
    public int? minSteps;
    public int? minHiddenSpots;
}

public struct CaveOptions {
    public CaveConditions? conditions;
    public int numberOfFishs;
    public float percentageOfSpawnsInPath;
    public Vector2? startCell;
    public Vector2? endCell;
}