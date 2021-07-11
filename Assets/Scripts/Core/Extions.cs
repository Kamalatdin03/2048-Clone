using UnityEngine;
using System.Collections.Generic;

public static class Extions
{
    public static T GetRandomItem<T>(this List<T> items)
    {
        int index = Random.Range(0, items.Count);
        return items[index];
    }

    public static List<Vector2Int> GetEmptyPositions(this ICell[,] grid)
    {
        List<Vector2Int> emptyCell = new List<Vector2Int>();

        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                if (grid[i, j] == null) emptyCell.Add(new Vector2Int(j, i));

        return emptyCell;
    }
}
