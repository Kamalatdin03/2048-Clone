using UnityEngine;
using DG.Tweening;

public class Styles : MonoBehaviour
{
    public static Styles Instance { get; private set; }

    [SerializeField] private Ease _ease;
    [SerializeField] private float _moveDuration;
    [SerializeField] private float _scaleDuration;
    [SerializeField] private ColorLibary _textColorLibary;
    [SerializeField] private ColorLibary _backgroundColorLibary;

    public Ease Ease => _ease;
    public float MoveDuration => _moveDuration;
    public float ScaleDuration => _scaleDuration;
    public ColorLibary TextColorLibary => _textColorLibary;
    public ColorLibary BackgroundColorLibary => _backgroundColorLibary;

    private void Awake()
    {
        Instance = this;
    }
}
