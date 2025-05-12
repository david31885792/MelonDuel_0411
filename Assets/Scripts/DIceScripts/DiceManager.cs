using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [Header("프리팹 및 부모")]
    public GameObject dicePrefab;
    public Transform diceParent;

    [Header("주사위 위치 (3x3 순서)")]
    public Transform[] targetPositions;

    [Header("면 정보")]
    public string[] faceNames = { "Top", "Bottom", "Left", "Right", "Front", "Back" };
    public int maxPerFace = 4;

    [Header("사운드 및 시간 설정")]
    [SerializeField] private float stopSoundDelay = 1.0f;
    [SerializeField] private float showPatternDuration = 3.0f; // 👈 플레이어 인식 시간 (Inspector에서 조절)
    [SerializeField] private AudioClip stopSound;
    [SerializeField] private AudioSource audioSource;

    private string[,] clearPattern = new string[3, 3];
    private List<Dice> diceList = new List<Dice>();

    public event Action OnDiceBoardReady;

    public void InitializeDiceBoard()
    {
        GenerateClearPattern();
        SpawnDiceGrid();
        SaveClearPattern();
        DebugClearPattern();

        OnDiceBoardReady?.Invoke();
    }

    private string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };
    private int maxPerColor = 4;

    private void GenerateClearPattern()
    {
        Dictionary<string, int> usageCount = new Dictionary<string, int>();
        foreach (string color in colorNames)
            usageCount[color] = 0;

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                string selected = null;
                int safety = 100;

                while (safety-- > 0)
                {
                    int index = UnityEngine.Random.Range(0, colorNames.Length);
                    string candidate = colorNames[index];

                    if (usageCount[candidate] < maxPerColor)
                    {
                        selected = candidate;
                        usageCount[candidate]++;
                        break;
                    }
                }


                clearPattern[row, col] = selected ?? "Red"; // fallback 기본값
            }
        }
    }


    private void SpawnDiceGrid()
    {
        ClearDiceGrid();

        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;
            Transform target = targetPositions[i];

            Quaternion initialRotation = Quaternion.Euler(-90f, 0f, 0f);
            GameObject go = Instantiate(dicePrefab, target.position, initialRotation, diceParent);
            go.name = $"Dice_{row}_{col}_{clearPattern[row, col]}";

            // ✅ 모든 MeshRenderer 끄기 (본체 + 자식 면들)
            foreach (var renderer in go.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }




            Dice dice = go.GetComponent<Dice>();
            if (dice != null)
            {
                dice.SetTopFace(clearPattern[row, col]); // 회전 방향 설정
                diceList.Add(dice);
            }
        }
    }


    private void ClearDiceGrid()
    {
        foreach (var dice in diceList)
        {
            if (dice != null)
                Destroy(dice.gameObject);
        }
        diceList.Clear();
    }

    private void SaveClearPattern()
    {
        PlayerPrefsPatternLoader.SavePattern(clearPattern);
    }

    private void DebugClearPattern()
    {
        string debug = "\n[ClearPattern]\n";
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                debug += clearPattern[row, col].PadRight(8);
            }
            debug += "\n";
        }
        Debug.Log(debug);
    }

    public void RollAllDicesAndThen(Action onFinished)
    {
        if (diceList == null || diceList.Count == 0)
        {
            Debug.LogWarning("⚠️ DiceManager: diceList 비어 있음");
            onFinished?.Invoke();
            return;
        }

        int completed = 0;

        foreach (Dice dice in diceList)
        {
            if (dice == null) continue;

            dice.Roll();

            void OnDiceRolled(int result)
            {
                completed++;
                dice.OnRollCompleted -= OnDiceRolled;

                if (completed >= diceList.Count)
                {
                    StartCoroutine(DelayedStopAndCallback(onFinished));
                }
            }

            dice.OnRollCompleted += OnDiceRolled;
        }
    }

    private IEnumerator DelayedStopAndCallback(Action onFinished)
    {
        yield return new WaitForSeconds(stopSoundDelay);

        if (audioSource && stopSound)
            audioSource.PlayOneShot(stopSound);

        yield return new WaitForSeconds(showPatternDuration); // 플레이어에게 패턴 인식 시간 제공

        onFinished?.Invoke();
    }
}
