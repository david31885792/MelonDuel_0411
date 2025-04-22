using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    [Header("주사위 설정")]
    public GameObject dicePrefab;
    public float spawnHeight = 10f;

    [Header("스폰 위치 (3x3)")]
    public Transform[] spawnPoints;

    [Header("정렬 위치 (3x3)")]
    public Transform[] targetPoints;

    [Header("클리어 패턴 UI")]
    public Transform patternPanelParent;
    public Color[] faceColors;

    [Header("GameManager")]
    public GameManager gameManager;

    private List<Dice> diceList = new List<Dice>();
    private string[,] clearPattern = new string[3, 3];

    public void SpawnDice()
    {
        diceList.Clear();

        for (int i = 0; i < 9; i++)
        {
            Vector3 pos = spawnPoints[i].position;
            pos.y = spawnHeight;

            GameObject die = Instantiate(dicePrefab, pos, Random.rotation);
            Dice dice = die.GetComponent<Dice>();

            for (int j = 0; j < dice.faces.Length; j++)
            {
                var rend = dice.faces[j].GetComponent<Renderer>();
                if (rend != null)
                    rend.material.color = faceColors[j % faceColors.Length];
            }

            diceList.Add(dice);
        }
    }

    public void StartRoll()
    {
        StartCoroutine(RollAndDetect());
    }

    private IEnumerator RollAndDetect()
    {
        foreach (Dice d in diceList)
            d.Roll();

        yield return new WaitUntil(() => AllDiceStopped());

        SaveClearPattern();

        if (!IsPatternValid())
        {
            Debug.Log("🚫 잘못된 패턴 → 재굴림");
            yield return new WaitForSeconds(1f);
            StartRoll();
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < diceList.Count; i++)
        {
            diceList[i].MoveTo(targetPoints[i].position);
        }

        yield return new WaitForSeconds(1.2f);
        ShowClearPatternUI();

        gameManager?.OnDiceComplete();
    }

    private bool AllDiceStopped()
    {
        foreach (Dice d in diceList)
            if (!d.HasStopped()) return false;
        return true;
    }

    private void SaveClearPattern()
    {
        Debug.Log("📦 SaveClearPattern()");

        for (int i = 0; i < diceList.Count; i++)
        {
            int row = i / 3;
            int col = i % 3;
            clearPattern[row, col] = diceList[i].GetTopFace();
        }
    }

    private void ShowClearPatternUI()
    {
        int index = 0;
        foreach (Transform child in patternPanelParent)
        {
            if (index >= diceList.Count) break;

            Dice dice = diceList[index];
            string face = dice.GetTopFace();
            int faceIndex = System.Array.IndexOf(dice.faceNames, face);

            if (faceIndex >= 0)
            {
                var rend = dice.faces[faceIndex].GetComponent<Renderer>();
                var image = child.GetComponent<Image>();
                if (rend != null && image != null)
                    image.color = rend.material.color;
            }

            index++;
        }
    }

    private bool IsPatternValid()
    {
        Dictionary<string, int> count = new Dictionary<string, int>();

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                string f = clearPattern[i, j];
                if (!count.ContainsKey(f)) count[f] = 0;
                count[f]++;
            }

        foreach (var kvp in count)
        {
            if (kvp.Value >= 5)
            {
                Debug.LogWarning($"🚫 색상 {kvp.Key}이 {kvp.Value}개 → 무효");
                return false;
            }
        }

        return true;
    }

    public string[,] GetClearPattern() => clearPattern;
}
