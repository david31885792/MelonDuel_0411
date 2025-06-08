using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    [Header("타일 위치")]
    public int x;
    public int y;

    [Header("타일 속성")]
    public string tileName;
    public Image tileImage;
    public TextMeshProUGUI tmpText;

    [Header("색상 매핑")]
    [SerializeField] private string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    [SerializeField] private Color[] colorValues;

    [Header("만능 타일 스프라이트")]
    [SerializeField] private Sprite wildTileSprite;

    private int tmpNumber;
    public int TMPNumber => tmpNumber;

    public bool isWildTile = false;

    private Sprite originalSprite;

    private void Awake()
    {
        if (tileImage != null)
            originalSprite = tileImage.sprite;
    }

    public void Initialize(int x, int y, string name)
    {
        this.x = x;
        this.y = y;
        UpdateColor(name);
    }

    public void UpdateColor(string name)
    {
        tileName = name;
        isWildTile = false;

        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                SetColor(colorValues[i]);
                SetTMPNumber(i + 1);
                ResetSpriteToOriginal();
                return;
            }
        }

        SetColor(Color.black);
        SetTMPNumber(0);
        ResetSpriteToOriginal();
    }

    public void SetColor(Color color)
    {
        if (tileImage != null)
        {
            color.a = 1f;
            tileImage.color = color;
        }
    }

    public void SetTMPNumber(int number)
    {
        tmpNumber = number;
        if (tmpText != null)
        {
            tmpText.text = number.ToString();
            tmpText.enabled = false;
        }
    }

    public void SetTileNumber(int number)
    {
        SetTMPNumber(number);
    }


    public void SetAsWildTile()
    {
        isWildTile = true;

        if (tileImage != null && wildTileSprite != null)
            tileImage.sprite = wildTileSprite;

        if (tmpText != null)
        {
            tmpText.text = "★";
            tmpText.enabled = true;
        }
    }

    private void ResetSpriteToOriginal()
    {
        if (tileImage != null && originalSprite != null)
            tileImage.sprite = originalSprite;
    }

    public int GetTileNumber()
    {
        return tmpNumber; // Assuming tmpNumber holds the tile's number.
    }

    public TileData ToData()
    {
        return new TileData(GetTileNumber(), name);
    }

    void OnDestroy()
    {
        transform.DOKill(); // 애니메이션 깨끗이 제거
    }

}
