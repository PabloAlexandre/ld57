using System;
using UnityEngine;

public class GameManager: MonoBehaviour {
    public Transform submarine;
    public int levels = 1;
    public string cacheKey;
    public bool deleteOnStart = false;
    public bool isScene = false;
    public int currentLevel = 0;

    void Awake() {
        if (deleteOnStart) {
            PlayerPrefs.DeleteAll();
        }

        if(isScene) {
            this.currentLevel = PlayerPrefs.GetInt("desired_level", 0);
        }
        
        for(int i = 0; i < levels; i++) {
            if(PlayerPrefs.GetInt($"{cacheKey}_level_{i}") == 1) {
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

        CaveGenerator cave = new CaveGenerator(4, 1, new CaveConditions() {
            minSteps = 9,
            minHiddenSpots = 3,
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