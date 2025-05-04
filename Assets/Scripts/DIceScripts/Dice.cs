using System;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [Header("면 이름 (Top, Bottom, Left, Right, Front, Back 순)")]
    public string[] faceNames = { "Top", "Bottom", "Left", "Right", "Front", "Back" };

    private DiceVisual diceVisual;
    private int faceIndex = 0;
    private bool isRolling = false;

    public bool IsRolling => isRolling;
    public event Action<int> OnRollCompleted;

    void Awake()
    {
        diceVisual = GetComponent<DiceVisual>();
    }

    /// <summary>
    /// 회전 없이 패턴만 설정
    /// </summary>
    public void SetTopFace(string faceName)
    {
        faceIndex = Array.IndexOf(faceNames, faceName);
        if (faceIndex < 0)
        {
            Debug.LogWarning($"[Dice] Invalid face name: {faceName}");
        }
    }

    /// <summary>
    /// 설정된 패턴대로 애니메이션 회전
    /// </summary>
    public void Roll()
    {
        if (isRolling || faceIndex < 0) return;

        isRolling = true;

        diceVisual.RollToFace(faceIndex, () =>
        {
            isRolling = false;
            OnRollCompleted?.Invoke(faceIndex);
        });
    }
}
