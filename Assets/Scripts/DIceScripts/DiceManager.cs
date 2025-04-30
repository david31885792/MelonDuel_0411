using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [Header("Dice 프리팹")]
    public GameObject dicePrefab;

    [Header("Dice 배치 위치")]
    public Transform[] targetPositions; // 9개의 타겟 포지션

    private List<Dice> diceList = new List<Dice>();
    private string[,] clearPattern = new string[3, 3];

    [Header("면 이름 리스트")]
    public string[] faceNames = { "Top", "Bottom", "Left", "Right", "Front", "Back" };

    void Start()
    {
        GenerateClearPattern();
        SpawnDiceGrid();
        SaveClearPattern();
    }

    private void GenerateClearPattern()
    {
        Debug.Log("📦 [DiceManager] Generating clear pattern...");

        // 6개의 색상(면 이름)이 각각 4개까지만 사용 가능
        Dictionary<string, int> faceCount = new Dictionary<string, int>();
        foreach (var face in faceNames) faceCount[face] = 0;

        int maxPerFace = 4;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                string face = "";

                do
                {
                    face = faceNames[Random.Range(0, faceNames.Length)];
                } while (faceCount[face] >= maxPerFace);

                faceCount[face]++;
                clearPattern[y, x] = face;
            }
        }
    }

    private void SpawnDiceGrid()
    {
        Debug.Log("🎲 [DiceManager] Spawning dice...");

        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;

            Vector3 spawnPos = targetPositions[i].position;
            GameObject go = Instantiate(dicePrefab, spawnPos, Quaternion.identity, transform);
            Dice dice = go.GetComponent<Dice>();

            if (dice != null)
            {
                dice.SetTopFace(clearPattern[row, col]);
                diceList.Add(dice);
            }
        }
    }

    private void SaveClearPattern()
    {
        Debug.Log("💾 [DiceManager] Saving clear pattern to PlayerPrefs...");

        PlayerPrefsPatternLoader.SavePattern(clearPattern);

        // 디버그 출력
        string debugPattern = "\n";
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                debugPattern += clearPattern[y, x] + "\t";
            }
            debugPattern += "\n";
        }

        Debug.Log($"📊 전체 3x3 패턴:\n{debugPattern}");
    }

    public void InitializeDiceBoard()
    {
        GenerateClearPattern();   // 6색 제한을 지켜서 3x3 패턴 생성
        SpawnDiceGrid();         // 주사위 9개 배치 및 SetTopFace()
        SaveClearPattern();      // PlayerPrefs에 저장
    }

}
