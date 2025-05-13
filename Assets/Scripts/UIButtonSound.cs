using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Tooltip("버튼 클릭 시 재생할 사운드 클립 (비워두면 AudioManager의 기본 클릭음 사용)")]
    public AudioClip customClip;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            // 직접 지정된 사운드가 있으면 그것 재생, 없으면 기본 클릭음 사용
            if (customClip != null)
                AudioManager.Instance.PlaySFX(customClip);
            else
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClip);
        }
    }
}
