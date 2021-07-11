using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class Cell : MonoBehaviour, ICell, IPoolableObject
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _pointText;

    private int _value;
    private bool _isMerged;
    private bool _isMoveing;

    private IField _field;
    private Action _onMerged;
    private Vector2Int _position;
    private Styles _styleManager;

    private const float PUNCH_VALUE = 0.2f;

    public int Value => _value;
    public int Point => (int)Math.Pow(2, _value);
    public bool IsMerged => _isMerged;
    public bool IsMoveing => _isMoveing;
    public GameObject Root => gameObject;
    public Vector2Int Position => _position;
    public Action OnMoveComplete { get; set; }

    private void UpdateCell()
    {
        _pointText.text = $"{Point}";

        var textColor = _styleManager.TextColorLibary.GetColorByIndex(_value);
        _pointText.DOColor(textColor, _styleManager.ScaleDuration).SetEase(_styleManager.Ease);

        var backgroundColor = _styleManager.BackgroundColorLibary.GetColorByIndex(_value);
        _backgroundImage.DOColor(backgroundColor, _styleManager.ScaleDuration).SetEase(_styleManager.Ease);
    }

    private void IncreaceValue()
    {
        ++_value;
        _isMerged = true;
    }

    public void Init(IField field)
    {
        _field = field;
        _styleManager = Styles.Instance;
    }

    public void Merge(ICell other, Vector2 movePoint)
    {
        IncreaceValue();

        SetPosition(other.Position);

        _onMerged = () =>
        {
            _isMerged = false;

            UpdateCell();

            PoolManager.Instance.DeSpawnPool("Cell", other.Root);

            transform.
                     DOPunchScale(Vector3.one * PUNCH_VALUE,
                     _styleManager.ScaleDuration).
                     SetEase(_styleManager.Ease);

            _onMerged = null;
        };

        Move(movePoint);
    }

    public void Move(Vector2 movePoint)
    {
        if (_isMoveing) return;

        _isMoveing = true;

        transform.
                 DOLocalMove(movePoint, _styleManager.MoveDuration).
                 SetEase(_styleManager.Ease).
                 OnComplete(() => 
                 {
                     _onMerged?.Invoke();
                     OnMoveComplete?.Invoke();

                     _isMoveing = false;
                 });
    }

    public void SetPosition(Vector2Int position)
    {
        if (_position != -Vector2Int.one) _field.Grid[_position.y, _position.x] = null;
        _position = position;
        _field.Grid[_position.y, _position.x] = this;
    }

    public void SetValue(int value)
    {
        _value = value;
        UpdateCell();
    }

    public void OnSpawn()
    {
        _value = 0;
        _isMerged = false;
        _position = -Vector2Int.one;
    }

    public void OnDeSpawn()
    {
        OnMoveComplete = null;
    }
}
