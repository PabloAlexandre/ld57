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

    public void Initialize(int x, int y, CellType type, bool isIntermediare = false) {
        this.x = x;
        this.y = y;
        this.type = type;
        this.isIntermediare = isIntermediare;
        if(type == CellType.STONE) {
            transform.Find("Wall-Back").gameObject.SetActive(false);
        }
        this.ActivateWalls();
    }

    void ActivateWalls() {
        float y = Random.Range(95.0f, 110.0f);
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

public enum CellType {
    PATH,
    STONE,
}
