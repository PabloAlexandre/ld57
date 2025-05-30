using System;
using UnityEngine;

public class GameManager: MonoBehaviour {
    public Transform submarine;
    public string cacheKey;
    public bool deleteOnStart = false;
    public bool isScene = false;
    public int currentLevel = 0;
    public int numberOfFishs;
    public float percentageOfSpawnsInPath;
    public ScriptableObject obj;

    void Awake() {
        if (deleteOnStart) {
            PlayerPrefs.DeleteAll();
        }

        this.cacheKey = Constants.CACHE_KEY;


        if (isScene) {
            this.currentLevel = PlayerPrefs.GetInt("desired_level", 0);
        }


    }

    private void Start() {
        if (!submarine && GameObject.FindGameObjectWithTag("Player")) {
            submarine = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void CreateGame() {
        PlayerPrefs.DeleteAll();



        for (int i = 0; i < Constants.CAVE_GENERATION_DEFS.Length; i++) {
            if (PlayerPrefs.GetInt($"{cacheKey}_level_{i}") == 1) {
                continue;
            }

            CreateLevel(i);
        }
    }

    public void SavePosition() {
        PlayerPrefs.SetFloat($"{cacheKey}_last_y", submarine.position.y);
        PlayerPrefs.SetFloat($"{cacheKey}_last_x", submarine.position.x);
        PlayerPrefs.SetInt($"{cacheKey}_upgrade", 1);
    }

    public bool IsBackFromUpgrade() {
        bool isBack = PlayerPrefs.GetInt($"{cacheKey}_upgrade", 0) == 1;
        if (isBack) {
            PlayerPrefs.DeleteKey($"{cacheKey}_upgrade");
            this.ResetLastPosition();
        }

        return isBack;
    }

    public void ResetLastPosition() {
        float y = PlayerPrefs.GetFloat($"{cacheKey}_last_y", 0);
        float x = PlayerPrefs.GetFloat($"{cacheKey}_last_x", 0);

        if (y != 0 || x != 0) {
            submarine.position = new Vector3(x, y, submarine.position.z);
            PlayerPrefs.DeleteKey($"{cacheKey}_last_y");
            PlayerPrefs.DeleteKey($"{cacheKey}_last_x");
        }
    }

    public void ResetPlayer() {
        PlayerPrefs.DeleteKey($"{cacheKey}_last_y");
        PlayerPrefs.DeleteKey($"{cacheKey}_last_x");
        PlayerPrefs.DeleteKey($"{cacheKey}_upgrade");
    }

    public void CreateLevel(int level) {
        if (PlayerPrefs.GetInt($"{cacheKey}_level_{level}") == 1) {
            return;
        }

        CavePredefinition def = Constants.CAVE_GENERATION_DEFS[level];
        CaveConditions conditions = new CaveConditions() {
            minSteps = def.minSteps,
            minHiddenSpots = def.minHiddenSpots,
        };

        CaveGenerator cave = new CaveGenerator(def.gridSize, 1, new CaveOptions() {
            conditions = conditions,
            numberOfFishs = def.numberOfFishs,
            percentageOfSpawnsInPath = def.percentageOfSpawnsInPath,
            startCell = def.startCell,
            endCell = def.endCell,
        });

        var mazeSolved = cave.GenerateCaves();

        var saveLevel = new SaveLevel {
            level = level,
            cells = this.convertTo1D(cave.baseMap),
            solvedPath = mazeSolved
        };

        string levelData = JsonUtility.ToJson(saveLevel);

        PlayerPrefs.SetString($"{cacheKey}_level_{level}_data", levelData);
        PlayerPrefs.SetInt($"{cacheKey}_level_{level}", 1);
    }

    // When Collect Items save them (must change in the map spawnType before saving)
    public void SaveLevel(CaveCell[][] map, int level) {
        if (PlayerPrefs.GetInt($"{cacheKey}_level_{level}", 0) == 0) {
            return;
        }

        var currentSave = this.LoadLevel(level);
        var saveLevel = new SaveLevel {
            level = level,
            cells = this.convertTo1D(map),
            solvedPath = currentSave.Value.solvedPath
        };

        string levelData = JsonUtility.ToJson(saveLevel);
        PlayerPrefs.SetString($"{cacheKey}_level_{level}_data", levelData);
    }

    CaveCell[] convertTo1D(CaveCell[][] map) {
        int size = map.Length;
        CaveCell[] result = new CaveCell[size * size];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                result[i * size + j] = map[i][j];
            }
        }
        return result;
    }

    CaveCell[][] convertTo2D(CaveCell[] map) {
        int size = (int)Math.Sqrt(map.Length);
        CaveCell[][] result = new CaveCell[size][];
        for (int i = 0; i < size; i++) {
            result[i] = new CaveCell[size];
            for (int j = 0; j < size; j++) {
                result[i][j] = map[i * size + j];
            }
        }
        return result;
    }

    public LevelGame? LoadLevel(int level) {
        if (PlayerPrefs.GetInt($"{cacheKey}_level_{level}", 0) == 0) {
            return null;
        }

        string levelData = PlayerPrefs.GetString($"{cacheKey}_level_{level}_data", "");
        Debug.Log(levelData);
        if (string.IsNullOrEmpty(levelData)) {
            return null;
        }

        var res = JsonUtility.FromJson<SaveLevel>(levelData);
        return new LevelGame {
            level = res.level,
            cells = convertTo2D(res.cells),
            solvedPath = res.solvedPath
        };
    }
}

[Serializable]
public struct SaveLevel {
    public int level;
    public CaveCell[] cells;
    public CaveCell[] solvedPath;
}

public struct LevelGame {
    public int level;
    public CaveCell[][] cells;
    public CaveCell[] solvedPath;
}