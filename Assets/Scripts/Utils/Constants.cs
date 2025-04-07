using UnityEngine;

public class Constants {
    public static string CACHE_KEY = "demo-game";
    public static float Y_OFFSET = 90f;
    public static int[][] WALL_DIRECTIONS = new int[][] {
        new int[] { 0, -1 }, // up
        new int[] { 1, 0 }, // right
        new int[] { 0, 1 }, // down
        new int[] { -1, 0 } // left
    };

    public static CavePredefinition[] CAVE_GENERATION_DEFS = new CavePredefinition[] {
        // LVL 1
        new CavePredefinition() {
            gridSize = 3,
            interval = 1,
            numberOfFishs = 1,
            minSteps = 6,
            minHiddenSpots = 2,
            percentageOfSpawnsInPath = 0.8f,
            startCell = new Vector2(0, 0),
            endCell = new Vector2(2, 2)
        },
        // LVL 2
        new CavePredefinition() {
            gridSize = 4,
            interval = 1,
            numberOfFishs = 3,
            minSteps = 9,
            minHiddenSpots = 5,
            percentageOfSpawnsInPath = 0.8f,
            startCell = new Vector2(0, 0),
            endCell = new Vector2(1, 3),
        },
        // LVL 3
        new CavePredefinition() {
            gridSize = 4,
            interval = 1,
            numberOfFishs = 3,
            minSteps = 9,
            minHiddenSpots = 5,
            percentageOfSpawnsInPath = 0.8f,
            startCell = new Vector2(0, 0),
            endCell = new Vector2(0, 3),
        },
        // LVL 4
        new CavePredefinition() {
            gridSize = 6,
            interval = 1,
            numberOfFishs = 5,
            minSteps = 22,
            minHiddenSpots = 5,
            percentageOfSpawnsInPath = 0.6f,
            startCell = new Vector2(0, 0),
            endCell = new Vector2(2, 5),
        },
        // LVL 5
        new CavePredefinition() {
            gridSize = 6,
            interval = 2,
            numberOfFishs = 5,
            minSteps = 17,
            minHiddenSpots = 5,
            percentageOfSpawnsInPath = 0.6f,
            startCell = new Vector2(0, 0),
            endCell = new Vector2(5, 5),
        }
    };

}


public class CavePredefinition {
    public int gridSize;
    public int interval;
    public int numberOfFishs;
    public int minSteps;
    public int minHiddenSpots;
    public float percentageOfSpawnsInPath;
    public Vector2 startCell;
    public Vector2 endCell;
}