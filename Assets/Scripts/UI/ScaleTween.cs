using DG.Tweening;
using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    [SerializeField] private Ease _ease;
    [SerializeField] private float _duration;

    private Tween _tween;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        _tween = transform.DOScale(Vector3.one, _duration).SetEase(_ease);
    }

    private void OnDisable()
    {
        _tween?.Kill(true);
    }
}
