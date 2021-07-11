using UnityEngine;
using System;

public interface ICell
{
    int Value { get; }
    int Point { get; }
    bool IsMerged { get; }
    bool IsMoveing { get; }
    GameObject Root { get; }
    Vector2Int Position { get; }
    Action OnMoveComplete { get; set; }

    void Init(IField field);
    void SetValue(int value);
    void SetPosition(Vector2Int position);
    void Move(Vector2 movePoint);
    void Merge(ICell other, Vector2 movePoint);
}
