using System.Linq;
using UnityEngine;

public class CollectResource : MonoBehaviour
{
    int x;
    int y;
    int level;
    public int index;

    public void Init(int x, int y, int level, int index) {
        this.x = x;
        this.y = y;
        this.level = level;
        this.index = index;
    }

    // Update is called once per frame
    public void OnCollect() {
        string key = $"{x}_{y}_{level}_spawn_points";
        SpawnPoint point = JsonUtility.FromJson<SpawnPoint>(PlayerPrefs.GetString(key));

        Vector3[] newCoords = point.coords.Select(c => c == point.coords[index] ? Vector3.zero : c).ToArray();
        point.coords = newCoords;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(point));
    }

    public void OnCollectFish() {
        string key = $"{x}_{y}_{level}_fish";

        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.SetInt($"{level}_fishs", PlayerPrefs.GetInt($"{level}_fishs", 0) + 1);
    }
}
