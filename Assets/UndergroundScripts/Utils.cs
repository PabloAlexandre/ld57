using UnityEditor;
using UnityEngine;

public class Utils {
    public static void DrawLine(Vector3 start, Vector3 end, Color color, int thickness = 3) {
        Handles.DrawBezier(start, end, start, end, color, null, thickness);
    }
}
