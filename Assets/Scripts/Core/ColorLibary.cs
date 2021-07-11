using UnityEngine;

[CreateAssetMenu(fileName = "Color Libary", menuName = "Game/ColorLibary")]
public class ColorLibary : ScriptableObject
{
    [SerializeField] private Color[] _colors;

    public Color[] Colors => _colors;

    public Color GetColorByIndex(int index)
    {
        if (index > _colors.Length - 1 || index < 0) return Color.clear;

        return _colors[index];
    }
}
