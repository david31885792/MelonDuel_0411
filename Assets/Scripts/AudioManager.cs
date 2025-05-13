using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource;

    [Header("SFX")]
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip buttonClip;
    public AudioClip playClip;
    public AudioClip gameStartClip;
    public AudioClip moveTileClip;
    public AudioClip winClip;

    [Header("BGM Clips")]
    public AudioClip mainBGM;
    public AudioClip gameBGM;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (isSFXEnabled && sfxSource && clip)
        {
            sfxSource.PlayOneShot(clip);
        }
    }


    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (!isMusicEnabled)
        {
            Debug.Log("🎵 BGM 꺼져 있음 → 재생 안 함");
            return;
        }

        if (bgmSource && clip)
        {
            if (bgmSource.clip == clip)
            {
                Debug.Log("🎵 이미 이 BGM이 재생 중 → 중복 재생 생략");
                return;
            }

            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
            Debug.Log("🎵 BGM 재생 시작됨: " + clip.name);
        }
    }



    public void StopBGM()
    {
        if (bgmSource)
            bgmSource.Stop();
    }

    private bool isSFXEnabled = true;
    private bool isMusicEnabled = true;

    public void SetSFXEnabled(bool enabled)
    {
        isSFXEnabled = enabled;
    }

    public void SetMusicEnabled(bool enabled)
    {
        isMusicEnabled = enabled;
        if (!enabled)
            bgmSource.Stop();
        else if (!bgmSource.isPlaying && bgmSource.clip != null)
            bgmSource.Play();
    }

}
