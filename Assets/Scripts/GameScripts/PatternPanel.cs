using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PatternPanel : MonoBehaviour
{
    [Header("타일 프리팹과 부모")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform boardParent;

    [Header("색상 매핑")]
    public string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    public Color[] colorValues;

    private Tile[] patternTiles = new Tile[9];

    private void Start()
    {
        
    }

    public void CreateTiles()
    {
        for (int i = 0; i < 9; i++)
        {
            Tile tile = Instantiate(tilePrefab, boardParent);
            tile.SetTMPNumber(0); // 초기화

            TextMeshProUGUI tmp = tile.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.enabled = false; // 텍스트 숨기기

            Image img = tile.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }

            patternTiles[i] = tile;
        }
    }


    public void GenerateRandomPattern()
    {
        if (patternTiles == null || patternTiles.Length != 9)
        {
            Debug.LogError("❌ 패턴 타일이 생성되지 않았습니다. CreateTiles()가 먼저 호출되어야 합니다.");
            return;
        }

        for (int i = 0; i < patternTiles.Length; i++)
        {
            if (patternTiles[i] == null)
            {
                Debug.LogError($"❌ patternTiles[{i}]가 null입니다.");
                continue;
            }

            int index = Random.Range(0, colorValues.Length);
            Color color = colorValues[index];

            patternTiles[i].SetColor(color);
            patternTiles[i].SetTMPNumber(index + 1);

            TextMeshProUGUI tmp = patternTiles[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.enabled = false;
            else
                Debug.LogWarning($"⚠️ patternTiles[{i}]에 TMP가 없습니다.");
        }

        Debug.Log("🎨 새로운 랜덤 클리어 패턴 생성 완료");
    }


    public int[] GetPattern()
    {
        int[] patternNumbers = new int[patternTiles.Length];
        for (int i = 0; i < patternTiles.Length; i++)
        {
            patternNumbers[i] = patternTiles[i].TMPNumber;
        }
        return patternNumbers;
    }

    public Tile[] GetPatternTiles()
    {
        return patternTiles;
    }
}
