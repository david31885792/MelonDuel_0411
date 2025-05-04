using UnityEngine;
using DG.Tweening;
using System;

public class DiceVisual : MonoBehaviour
{
    [Header("회전 각도 설정 (Top~Back 순서)")]
    public Vector3[] faceRotations;

    [Header("애니메이션 설정")]
    public float rollDuration = 1.2f;
    public Ease rollEase = Ease.OutBack;

    [Header("카메라 흔들림")]
    public Transform cameraTransform;
    public float shakeIntensity = 0.2f;
    public float shakeDuration = 0.3f;

    [Header("사운드")]
    public AudioSource audioSource;
    public AudioClip rollSound;

    private Tween activeTween;

    void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    public void RollToFace(int faceIndex, Action onComplete)
    {
        if (faceIndex < 0 || faceIndex >= faceRotations.Length)
        {
            Debug.LogWarning("[DiceVisual] Invalid face index.");
            onComplete?.Invoke();
            return;
        }

        if (activeTween != null && activeTween.IsActive())
            activeTween.Kill();

        // 사운드
        if (audioSource && rollSound)
            audioSource.PlayOneShot(rollSound);

        // 카메라 흔들림
        if (cameraTransform != null)
            cameraTransform.DOShakePosition(shakeDuration, shakeIntensity).SetEase(Ease.OutQuad);

        // 회전 애니메이션
        activeTween = transform.DORotate(faceRotations[faceIndex], rollDuration, RotateMode.FastBeyond360)
            .SetEase(rollEase)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    void OnDestroy()
    {
        if (activeTween != null && activeTween.IsActive())
            activeTween.Kill();
    }
}
