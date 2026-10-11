using UnityEngine;

public class PixelUiFont : MonoBehaviour
{
    [SerializeField] private Font _font;

    private void OnEnable()
    {
        Font.textureRebuilt += OnTextureRebuilt;
        ApplyPointFiltering();
    }

    private void OnDisable()
    {
        Font.textureRebuilt -= OnTextureRebuilt;
    }

    private void OnTextureRebuilt(Font font)
    {
        if (font == _font) ApplyPointFiltering();
    }

    private void ApplyPointFiltering()
    {
        if (_font == null || _font.material == null || _font.material.mainTexture == null) return;
        _font.material.mainTexture.filterMode = FilterMode.Point;
    }
}
