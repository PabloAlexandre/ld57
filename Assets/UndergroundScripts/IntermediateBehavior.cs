using UnityEngine;

public class IntermediateBehavior : MonoBehaviour {
    public CellUnity cell;
    public void Initialize(CellUnity cell) {
        this.cell = cell;
        this.isInValidPathToOwner();
    }


    bool isInValidPathToOwner() {
        if (cell.owner.x != cell.x && cell.owner.y != cell.y) return false;
        string side = "";

        if (cell.owner.x != cell.x && cell.owner.x < cell.x) {
            side = "right";
        } else if (cell.owner.y < cell.y) {
            side = "down";
        }

        if (side == "") return false;
        
        switch(side) {
            case "right":
                if (!cell.owner.walls[(int)Walls.Right]) {
                    cell.walls = new bool[4] {
                        true, false, true, false
                    };
                    cell.isPath = cell.owner.isPath;
                    cell.Initialize(CellType.PATH);
                }
                break;
            case "down":
                if (!cell.owner.walls[(int)Walls.Down]) {
                    cell.walls = new bool[4] {
                        false, true, false, true
                    };
                    cell.isPath = cell.owner.isPath;
                    cell.Initialize(CellType.PATH);
                }
                break;
            default:
                break;
        }
        
        return true;
    }
}
