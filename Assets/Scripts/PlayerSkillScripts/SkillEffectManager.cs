using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillEffectManager : MonoBehaviour
{
    [Header("Player1 Effects")]
    [SerializeField] private Image player1UserEffect;
    [SerializeField] private Image player1AIEffect;

    [Header("Player2 Effects")]
    [SerializeField] private Image player2UserEffect;
    [SerializeField] private Image player2AIEffect;

    [Header("Player3 Effects")]
    [SerializeField] private Image player3UserEffect;
    [SerializeField] private Image player3AIEffect;

    [SerializeField] private float effectDuration = 3f;

    public enum SkillType { Player1, Player2, Player3 }

    public void ShowEffect(SkillType type, bool isAI)
    {
        Image target = null;

        switch (type)
        {
            case SkillType.Player1:
                target = isAI ? player1AIEffect : player1UserEffect;
                break;
            case SkillType.Player2:
                target = isAI ? player2AIEffect : player2UserEffect;
                break;
            case SkillType.Player3:
                target = isAI ? player3AIEffect : player3UserEffect;
                break;
        }

        if (target != null)
            StartCoroutine(EffectCoroutine(target));
    }

    private IEnumerator EffectCoroutine(Image img)
    {
        img.gameObject.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        img.gameObject.SetActive(false);
    }
}
