using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    [SerializeField] private Field _field;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _bestScoreText;
    [SerializeField] private int _grirSize;
    [SerializeField] private float _spaceing;
    [SerializeField] private GameObject _losePanel;

    private int score;
    private bool isGameEnd;

    private int BestScore
    {
        get => PlayerPrefs.GetInt("Best Score", 0);
        set => PlayerPrefs.SetInt("Best Score", value);
    }

    private void Awake()
    {
        DG.Tweening.DOTween.Init();
    }

    private void OnEnable()
    {
        _field.GameEnd += OnGameEnd;
        _field.ScoreChanged += OnScoreChanged;
    }

    private void Start()
    {
        _bestScoreText.text = $"{BestScore}";

        StartGame();
    }

    private void Update()
    {
        if (isGameEnd)
            return;

        if (SwipeInput.SwipedUp)
            _field.Move(Vector2Int.up);
        else if (SwipeInput.SwipedDown)
            _field.Move(Vector2Int.down);
        else if (SwipeInput.SwipedLeft)
            _field.Move(Vector2Int.left);
        else if (SwipeInput.SwipedRigth)
            _field.Move(Vector2Int.right);
    }

    private void OnScoreChanged(int value)
    {
        score += value;

        if (score > BestScore)
        {
            BestScore = score;
            _bestScoreText.text = $"{score}";
        }

        _scoreText.text = $"{score}";
    }

    private void OnGameEnd()
    {
        isGameEnd = true;
        _losePanel.SetActive(true);
    }

    public void StartGame()
    {
        score = 0;
        isGameEnd = false;

        _losePanel.SetActive(false);

        _field.Reset();
        _field.Init(_grirSize, _spaceing);
    }

    public void ChangeGridSize(int gridSize)
    {
        this._grirSize = gridSize;
    }

    private void OnDisable()
    {
        _field.GameEnd -= OnGameEnd;
        _field.ScoreChanged -= OnScoreChanged;
    }
}
