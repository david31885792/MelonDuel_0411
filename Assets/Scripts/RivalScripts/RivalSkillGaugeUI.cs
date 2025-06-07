using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AI 전용 스킬 게이지 UI 표시 (기존 SkillGaugeUI 확장)
/// </summary>
public class RivalSkillGaugeUI : MonoBehaviour
{
    [Header("게이지 UI")]
    public Slider gaugeSlider;
    public RectTransform fillRect;           // Slider Fill 이미지
    public RectTransform skillbarPoint;      // 삼각형 장식

    [Header("연동 매니저")]
    [SerializeField] private RivalGaugeManager gaugeManager;

    private void Update()
    {
        if (gaugeManager == null) return;

        float value = gaugeManager.GetGaugeNormalized();
        gaugeSlider.value = value;

        // 🎯 Fill 끝 좌표 계산 → skillbarPoint 위치 이동
        if (fillRect != null && skillbarPoint != null)
        {
            Vector3[] corners = new Vector3[4];
            fillRect.GetWorldCorners(corners); // 0=BL, 1=TL, 2=TR, 3=BR

            Vector3 targetWorldPos = corners[2]; // 오른쪽 상단
            Vector2 uiPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)skillbarPoint.parent,
                Camera.main.WorldToScreenPoint(targetWorldPos),
                Camera.main,
                out uiPos
            );

            skillbarPoint.anchoredPosition = uiPos;
        }
    }
}
