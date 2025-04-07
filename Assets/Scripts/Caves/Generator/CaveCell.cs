using System;
using UnityEngine;

[Serializable]
public class CaveCell {
    public int x, y;
    public bool visited = false;
    public bool isPath = false;
    public SpawnType spawnType = SpawnType.NONE;
    public bool isStart = false;
    public bool isEnd = false;
    public Vector3[] spawnPoints = null;

    // 0 = up, 1 = right, 2 = down, 3 = left
    public bool[] walls = new bool[4] {
        true, true, true, true
    };

    public CaveCell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void reset() {
        this.visited = false;
        this.isPath = false;
        this.isStart = false;
        this.isEnd = false;
        this.spawnType = SpawnType.NONE;
        this.walls = new bool[] { true, true, true, true };
    }

    public void removeWall(int wallIndex) {
        int wall = (int)wallIndex;
        if (wall >= 0 && wall < walls.Length) {
            walls[wall] = false;
        }
    }

    public void removeOppositeWall(int wallIndex) {
        int wall = (int)wallIndex;

        // 0 = up, 1 = right, 2 = down, 3 = left
        int[] opposite = new int[] { 2, 3, 0, 1 };
        walls[opposite[wall]] = false;
    }

    public void SetSpawnType(SpawnType type) {
        this.spawnType = type;
    }
}

public enum Walls {
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}
public struct CaveCellNeighbor {
    public CaveCell cell;
    public int wall;
}

[Serializable]
public enum SpawnType {
    NONE,
    BLUE_ITEM,
    RED_ITEM,
}