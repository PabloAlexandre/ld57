using UnityEngine;

[CreateAssetMenu(fileName = "SubmarineStats", menuName = "Submarine/Stats")]
public class SubmarineStats : ScriptableObject
{
    [Header("Movimento")]
    public float speed = 10f;

    [Header("Profundidade")]
    public float maxDepth = -10f;

    [Header("Mineracao")]
    public float miningSpeed = 5f;
    public float miningDistance = 5f;
    public float gold = 0f;

    [Header("Luz")]
    public float lightDistance = 10f;
}
