using UnityEngine;
using UnityEngine.UI;

public class SkillGaugeUI : MonoBehaviour
{
    public Slider gaugeSlider;
    public RectTransform fillRect; // Slider Fill 이미지
    public RectTransform skillbarPoint; // 삼각형 장식

    private void Update()
    {
        if (SkillGaugeManager.Instance == null) return;

        float value = SkillGaugeManager.Instance.GetGaugeNormalized();
        gaugeSlider.value = value;

        // 💡 Fill 의 오른쪽 끝 계산 (월드 기준)
        if (fillRect != null && skillbarPoint != null)
        {
            Vector3[] corners = new Vector3[4];
            fillRect.GetWorldCorners(corners); // 0=BL, 1=TL, 2=TR, 3=BR

            // 오른쪽 상단 코너
            Vector3 targetWorldPos = corners[2];

            // SkillbarPoint 를 그 위치로 이동
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
