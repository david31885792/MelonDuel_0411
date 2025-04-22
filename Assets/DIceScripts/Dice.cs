using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dice : MonoBehaviour
{
    [Header("면 오브젝트 (Top, Bottom, Left, Right, Front, Back 순)")]
    public Transform[] faces;

    [Header("면 이름 (faces[]와 일치)")]
    public string[] faceNames = { "Top", "Bottom", "Left", "Right", "Front", "Back" };

    [Header("머티리얼")]
    public Material highlightMaterial;
    public Material defaultMaterial;

    private Rigidbody rb;
    private bool canDetect = false;
    private bool hasDetected = false;

    private string topFace = "";
    private Quaternion detectedRotation;

    private Coroutine blinkRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void Roll()
    {
        Debug.Log("🎲 [Dice] Roll()");

        rb.useGravity = true;
        rb.isKinematic = false;
        canDetect = true;
        hasDetected = false;

        Vector3 randomTorque = new Vector3(
            Random.Range(-500f, 500f),
            Random.Range(-500f, 500f),
            Random.Range(-500f, 500f)
        );
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        Vector3 randomForce = new Vector3(
            Random.Range(-2f, 2f),
            0f,
            Random.Range(-2f, 2f)
        );
        rb.AddForce(Vector3.up * 5f + randomForce, ForceMode.Impulse);
    }

    void Update()
    {
        if (!canDetect || hasDetected) return;

        if (rb.IsSleeping())
        {
            canDetect = false;
            hasDetected = true;
            DetectTopFace();
        }
    }

    private void DetectTopFace()
    {
        Debug.Log("🔍 [Dice] DetectTopFace");

        Vector3 localWorldUp = transform.InverseTransformDirection(Vector3.up);
        float maxDot = -1f;
        int topIndex = -1;

        for (int i = 0; i < faces.Length; i++)
        {
            Vector3 localUp = faces[i].localRotation * Vector3.up;
            float dot = Vector3.Dot(localUp, localWorldUp);

            Debug.DrawRay(faces[i].position, faces[i].up * 0.5f, Color.red, 2f);

            if (dot > maxDot)
            {
                maxDot = dot;
                topIndex = i;
            }
        }

        if (topIndex >= 0)
        {
            topFace = faceNames[topIndex];
            detectedRotation = Quaternion.FromToRotation(faces[topIndex].up, Vector3.up) * transform.rotation;

            rb.useGravity = false;
            rb.isKinematic = true;

            Debug.Log($"✅ TopFace: {topFace}, Index: {topIndex}");
            Debug.Log($"🌀 회전값: {detectedRotation.eulerAngles}");

            BlinkFace(topIndex);
        }
        else
        {
            Debug.LogWarning("❌ 감지 실패");
        }
    }

    public void MoveTo(Vector3 targetPos)
    {
        rb.useGravity = false;
        rb.isKinematic = true;

        ResetFaceMaterials();

        transform.DOMove(targetPos, 1f);
        transform.DORotateQuaternion(detectedRotation, 1f);
    }

    private void ResetFaceMaterials()
    {
        foreach (Transform face in faces)
        {
            Renderer rend = face.GetComponent<Renderer>();
            if (rend != null && defaultMaterial != null)
                rend.material = defaultMaterial;
        }
    }

    private void BlinkFace(int index)
    {
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkRoutine(index));
    }

    private IEnumerator BlinkRoutine(int index)
    {
        Renderer target = faces[index].GetComponent<Renderer>();
        if (target == null) yield break;

        float blinkTime = 0.15f;
        int blinkCount = 6;

        for (int i = 0; i < blinkCount; i++)
        {
            target.material = (i % 2 == 0) ? highlightMaterial : defaultMaterial;
            yield return new WaitForSeconds(blinkTime);
        }

        target.material = defaultMaterial; // 마지막엔 꺼진 상태
    }

    public bool HasStopped() => hasDetected;
    public string GetTopFace() => topFace;
    public Quaternion GetDetectedRotation() => detectedRotation;
}
