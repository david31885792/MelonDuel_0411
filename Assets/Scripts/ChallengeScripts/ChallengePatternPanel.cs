using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengePatternPanel : MonoBehaviour
{
    public static ChallengePatternPanel Instance { get; private set; }

    [Header("타일 프리팹과 부모")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform boardParent;

    [Header("색상 매핑")]
    public string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    public Color[] colorValues;

    private Tile[] patternTiles = new Tile[9];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CreateTiles();
        GenerateNewPattern(); // 시작 시 첫 패턴 생성
    }

    public void GenerateNewPattern()
    {
        for (int i = 0; i < 9; i++)
        {
            int randomIndex = Random.Range(0, colorValues.Length);
            Color color = colorValues[randomIndex];
            patternTiles[i].SetColor(color);
            patternTiles[i].SetTMPNumber(randomIndex + 1); // TMP에는 색상 번호 저장

            // TMP 비가시화
            var tmp = patternTiles[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.enabled = false;
        }
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

    private void CreateTiles()
    {
        for (int i = 0; i < 9; i++)
        {
            Tile tile = Instantiate(tilePrefab, boardParent);
            tile.SetTMPNumber(0); // 초기화

            var tmp = tile.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.enabled = false;

            // 알파 강제
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
}
