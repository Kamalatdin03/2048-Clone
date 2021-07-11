using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class Field : MonoBehaviour, IField
{
    private PoolManager _poolManager;
    private RectTransform _rectTransform;
    private ICell[,] _grid;
    private List<GameObject> _fields;

    private int _totalScore;
    private int _gridSize;
    private float _spaceing;
    private bool _onceExcuteFlag;

    private const int START_CELL_COUNT = 2;
    private const string CELL_ID = "Cell";
    private const string FIELD_ID = "Field";

    private float CellSize => (_rectTransform.rect.width - _spaceing * (_gridSize + 1)) / _gridSize;

    public event Action<int> ScoreChanged;
    public event Action GameEnd;
    public ICell[,] Grid => _grid;

    public void Init(int gridSize, float margin)
    {
        ScoreChanged?.Invoke(0);

        _rectTransform = GetComponent<RectTransform>();
        _poolManager = PoolManager.Instance;

        _gridSize = gridSize;
        _spaceing = margin;

        CreateField();

        for (int i = 0; i < START_CELL_COUNT; i++)
            CreateNewCell();
    }

    public void Move(Vector2Int direction)
    {
        if (!CanIMove()) return;

        _onceExcuteFlag = false;

        bool canY = direction.x == 0;
        bool canDigonalUp = direction.x == -1 || direction.y == 1;

        int dir = canDigonalUp ? -1 : 1;

        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = (canDigonalUp ? 1 : _gridSize - 2); j >= 0 && j < _gridSize; j -= dir)
            {
                var curretCell = canY ? _grid[j, i] : _grid[i, j];
                if (curretCell == null) continue;

                var otherCell = FindCell(curretCell, dir, canY);
                if (otherCell != null)
                {                 
                    curretCell.Merge(otherCell, GetCellPosition(otherCell.Position.x, otherCell.Position.y));
                    _totalScore += curretCell.Point;

                    continue;
                }

                var emptyPosition = FindEmptyPositon(curretCell, dir, canY);
                if (emptyPosition != -Vector2Int.one)
                {
                    curretCell.SetPosition(emptyPosition);
                    curretCell.Move(GetCellPosition(emptyPosition.x, emptyPosition.y));
                }
            }
        }

        if (_grid.GetEmptyPositions().Count == 0)
        {
            if (IsLose())
            {
                GameEnd?.Invoke();
            }
        }
    }

    public void Reset()
    {
        _onceExcuteFlag = false;

        if (_grid == null || _fields == null) return;

        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                if (_grid[i, j] != null) _poolManager.DeSpawnPool(CELL_ID, _grid[i, j].Root);
            }
        }

        foreach (var field in _fields)
        {
            _poolManager.DeSpawnPool(FIELD_ID, field);
        }
    }

    private ICell FindCell(ICell cell, int dir, bool flag)
    {
        for (int i = (flag ? cell.Position.y : cell.Position.x) + dir; i >= 0 && i < _gridSize; i += dir)
        {
            var otherCell = flag ? _grid[i, cell.Position.x] : _grid[cell.Position.y, i];

            if (otherCell == null) continue;
            else
            {
                if (cell.Value == otherCell.Value && !otherCell.IsMerged)
                {
                    return otherCell;
                }

                break;
            }
        }

        return null;
    }

    private Vector2Int FindEmptyPositon(ICell cell, int dir, bool flag)
    {
        var position = -Vector2Int.one;

        int targetIndex = -1;

        for (int i = (flag ? cell.Position.y : cell.Position.x) + dir; i >= 0 && i < _gridSize; i += dir)
        {
            var otherCell = flag ? _grid[i, cell.Position.x] : _grid[cell.Position.y, i];

            if (otherCell == null)
            {
                targetIndex = i;
            }
            else break;
        }

        if (targetIndex != -1)
        {
            if (flag)
            {
                position.x = cell.Position.x;
                position.y = targetIndex;
            }
            else
            {
                position.x = targetIndex;
                position.y = cell.Position.y;
            }
        }

        return position;
    }

    private void CreateField()
    {
        _grid = new ICell[_gridSize, _gridSize];
        _fields = new List<GameObject>();

        for (int y = 0; y < _gridSize; y++)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                var field = _poolManager.SpawnPool<RectTransform>(FIELD_ID, transform);
                _fields.Add(field.gameObject);
                field.localScale = Vector3.one;

                InitCell(ref field, x, y);
            }
        }
    }

    private void OnCellMoveCompleted()
    {
        if (_onceExcuteFlag) return;

        CreateNewCell();
        ScoreChanged?.Invoke(_totalScore);
        _totalScore = 0;

        _onceExcuteFlag = true;
    }

    private void CreateNewCell()
    {
        var emptyPositions = _grid.GetEmptyPositions();
        if (emptyPositions.Count == 0)
        {
            throw new InvalidOperationException();
        }

        var value = UnityEngine.Random.Range(0, 10) == 0 ? 2 : 1;
        var position = emptyPositions.GetRandomItem();

        var cell = _poolManager.SpawnPool<ICell>(CELL_ID, transform);
        var cellRect = cell.Root.GetComponent<RectTransform>();

        cell.Init(this);
        InitCell(ref cellRect, position.x, position.y);

        cell.SetValue(value);
        cell.SetPosition(position);
        cell.OnMoveComplete = OnCellMoveCompleted;
    }

    private bool IsLose()
    {
        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                var cell = _grid[i, j];


                if (cell != null)
                {
                    if (HasEqualN(cell)) return false;
                }
            }
        }

        return true;
    }

    private bool CanIMove()
    {
        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                var cell = _grid[i, j];

                if (cell == null) continue;

                if (cell.IsMoveing) return false;
            }
        }

        return true;
    }

    private bool HasEqualN(ICell cell)
    {
        int x = cell.Position.x;
        int y = cell.Position.y;

        if (x < _gridSize - 1 && _grid[y, x + 1] != null && cell.Value == _grid[y, x + 1].Value) return true;
        else if (x > 0 && _grid[y, x - 1] != null && cell.Value == _grid[y, x - 1].Value) return true;
        else if (y < _gridSize - 1 && _grid[y + 1, x] != null && cell.Value == _grid[y + 1, x].Value) return true;
        else if (y > 0 && _grid[y - 1, x] != null && cell.Value == _grid[y - 1, x].Value) return true;

        return false;
    }

    private void InitCell(ref RectTransform cell, int x, int y)
    {
        cell.sizeDelta = Vector2.one * CellSize;
        cell.localPosition = GetCellPosition(x, y);
    }

    private Vector2 GetCellPosition(int x, int y)
    {
        float width = _rectTransform.rect.width;

        float startX = -((width - CellSize) / 2 - _spaceing);
        float startY = (width - CellSize) / 2 - _spaceing;

        return new Vector2
        {
            x = startX + x * (CellSize + _spaceing),
            y = startY - y * (CellSize + _spaceing)
        };
    }
}
