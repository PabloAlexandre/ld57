using System;
using System.Collections.Generic;
using System.Linq;

public class CaveUtils {
    public static List<CaveCell> PickRandomWithDistance(List<CaveCell> cells, int count, double minDistance) {
        var random = new Random();
        var shuffled = cells.OrderBy(_ => random.Next()).ToList();

        var selected = new List<CaveCell>();

        foreach (var cell in shuffled) {
            if (selected.All(s => CaveUtils.DistanceTo(s, cell) >= minDistance)) {
                selected.Add(cell);
                if (selected.Count == count)
                    break;
            }
        }

        if(selected.Count < count) {
            // fill with random cells not in selected
            selected.AddRange(shuffled
                .Where(c => !selected.Contains(c))
                .Take(count - selected.Count));
        }

        return selected;
    }

    public static double DistanceTo(CaveCell from, CaveCell to) {
        // Euclidean distance
        return Math.Sqrt(Math.Pow(from.x - to.x, 2) + Math.Pow(from.y - to.y, 2));

        // If you prefer Manhattan distance, use:
        // return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }
}
