using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CellUnity : MonoBehaviour {
    public CellType type;
    public int x;
    public int y;
    public bool isPath = false;
    public bool isIntermediare = false;
    public bool[] walls;
    public CellUnity owner;
    public SpawnType spawnType = SpawnType.NONE;
    public CaveCell cell;

    public Transform miningPrefab;
    public Transform fishPrefab;

    private GameManager manager;

    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Initialize(CellType type) {
        this.type = type;
        this.ActivateWalls();
        if(type != CellType.STONE) {
            transform.Find("Wall-Back").gameObject.SetActive(true);
        } else {

        }
        //transform.Find("bg").GetComponent<Renderer>().material.color = Color.gray;
    }

    public void Initialize(int x, int y, CellType type, bool isIntermediare = false, CaveCell cell = null) {
        this.x = x;
        this.y = y;
        this.type = type;
        this.isIntermediare = isIntermediare;
        this.cell = cell;
        this.manager = FindFirstObjectByType<GameManager>();

        if (type == CellType.STONE) {
            transform.Find("Wall-Back").gameObject.SetActive(false);
        }

        if (cell != null && cell.isEnd) {
            transform.Find("Tunnel").gameObject.SetActive(true);
        }

        if(cell != null && cell.isStart) {
            transform.Find("CaveStart").gameObject.SetActive(true);
        }

        this.ActivateWalls();
        if (this.spawnType != SpawnType.NONE) {
            this.Spawn();
        }
    }

    void ActivateWalls() {
        float y = UnityEngine.Random.Range(95.0f, 110.0f);
        Transform backStone = transform.Find("Wall-Back");
        Quaternion rotation = Quaternion.Euler(new Vector3(-91, y, -12));
        transform.Find("Wall-Back").transform.localRotation = rotation;

        string[] names = new string[] { "Wall-T", "Wall-R", "Wall-D", "Wall-L" };

        for (int k = 0; k < walls.Length; k++) {
            if (walls[k]) {
                transform.Find(names[k]).gameObject.SetActive(true);
            }
        }
    }

    void Spawn() {
        Transform prefab = this.miningPrefab;
        Vector3[] spawnPoints = new Vector3[0];
        string key = $"{cell.x}_{cell.y}_{manager.currentLevel}_spawn_points";
        bool hasKey = PlayerPrefs.HasKey(key);

        if (PlayerPrefs.HasKey(key)) {
            spawnPoints = JsonUtility.FromJson<SpawnPoint>(PlayerPrefs.GetString(key)).coords;
        }

        if (this.spawnType == SpawnType.BLUE_ITEM) {
            prefab = this.miningPrefab;
            if(!hasKey) {
                spawnPoints = getSpawnPoint("MiningSpawnPoints", 3);
                SpawnPoint point = new SpawnPoint() {
                    coords = spawnPoints
                };

                PlayerPrefs.SetString(key, JsonUtility.ToJson(point));
            }
        } else if (this.spawnType == SpawnType.RED_ITEM) {
            if(PlayerPrefs.HasKey($"{cell.x}_{cell.y}_{manager.currentLevel}_fish")) {
                return;
            }

            prefab = this.fishPrefab;
            
            if (!hasKey) {
                spawnPoints = getSpawnPoint("FishSpawnPoints", 1);
                SpawnPoint point = new SpawnPoint() {
                    coords = spawnPoints
                };
                PlayerPrefs.SetString(key, JsonUtility.ToJson(point));
            }
        }

        
        for (int i = 0; i < spawnPoints.Length; i++) {
            if(spawnPoints[i] == Vector3.zero) {
                continue;
            }
            Transform t = Instantiate(prefab, spawnPoints[i], Quaternion.identity);
            t.AddComponent<CollectResource>().Init(this.cell.x, this.cell.y, manager.currentLevel, i);
        }
    }


    Vector3[] getSpawnPoint(string type, int count) {
        var random = new System.Random();

        return transform
            .Find(type)
            .GetComponentsInChildren<Transform>()
            .OrderBy(_ => random.Next())
            .Select(t => t.position)
            .Take(count)
            .ToArray();


    }

    private void OnDrawGizmosSelected() {
        if (this.owner != null && false) {
            // Ray to the owner
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(this.owner.transform.position, 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.05f);

            Gizmos.color = Color.green;

            var p1 = transform.position;
            var p2 = this.owner.transform.position;
            var thickness = 3;
            Handles.DrawBezier(p1, p2, p1, p2, Color.red, null, thickness);

        }
        if(this.owner) {

        }
    }

}

[Serializable]
public class SpawnPoint {
    public Vector3[] coords;
}

public enum CellType {
    PATH,
    STONE,
}
