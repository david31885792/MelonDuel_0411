using UnityEngine;
using System.Collections;

public class Player2Skill_Blind : MonoBehaviour, ICharacterSkill
{
    [SerializeField] private float blindDuration = 3f; // Inspector에서 조절 가능

    public void ActivateSkill()
    {
        Debug.Log("루루노: 가림막 발동!");
        StartCoroutine(ApplyBlind());
    }

    public bool IsReady()
    {
        // 간단한 예: 항상 준비 상태로 반환
        return true;
    }

    private IEnumerator ApplyBlind()
    {
        // UI 가림막 및 입력 차단
        UIBlocker.Instance?.Show();
        InputBlocker.Instance?.Block();

        yield return new WaitForSeconds(blindDuration);

        // 해제
        UIBlocker.Instance?.Hide();
        InputBlocker.Instance?.Unblock();
    }
}
