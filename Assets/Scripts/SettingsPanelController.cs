using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [Header("패널")]
    public GameObject settingsPanel;

    [Header("아이콘")]
    public Image soundIcon;
    public Image musicIcon;
    public Image vibrationIcon;

    [Header("아이콘 스프라이트")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite vibrationOnSprite;
    public Sprite vibrationOffSprite;

    private bool isSoundOn = true;
    private bool isMusicOn = true;
    private bool isVibrationOn = true;

    // 설정창 열기
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    // 설정창 닫기
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        soundIcon.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        AudioManager.Instance.SetSFXEnabled(isSoundOn);
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        musicIcon.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
        AudioManager.Instance.SetMusicEnabled(isMusicOn);
    }


    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;
        vibrationIcon.sprite = isVibrationOn ? vibrationOnSprite : vibrationOffSprite;
        // 📳 실제 진동 처리: DeviceVibrator.SetVibration(isVibrationOn);
    }
}
