using UnityEngine;

public class Dice : MonoBehaviour
{
    [Header("면 오브젝트 (Top, Bottom, Left, Right, Front, Back 순)")]
    public Transform[] faces;

    [Header("면 이름 (faces[]와 순서 일치)")]
    public string[] faceNames = { "Top", "Bottom", "Left", "Right", "Front", "Back" };

    [Header("머티리얼")]
    public Material highlightMaterial;
    public Material defaultMaterial;

    private string topFace = "Top";
    private Quaternion setRotation;

    void Start()
    {
        ResetFaceMaterials();
    }

    /// <summary>
    /// 원하는 면이 위로 오도록 회전 설정
    /// </summary>
    public void SetTopFace(string faceName)
    {
        Quaternion rot = Quaternion.identity;

        switch (faceName)
        {
            case "Top": rot = Quaternion.identity; break;
            case "Bottom": rot = Quaternion.Euler(180f, 0f, 0f); break;
            case "Left": rot = Quaternion.Euler(0f, 0f, -90f); break;
            case "Right": rot = Quaternion.Euler(0f, 0f, 90f); break;
            case "Front": rot = Quaternion.Euler(-90f, 0f, 0f); break;
            case "Back": rot = Quaternion.Euler(90f, 0f, 0f); break;
            default: Debug.LogWarning($"❗ Unknown face: {faceName}"); break;
        }

        transform.rotation = rot;
        topFace = faceName;
        setRotation = rot;

        Debug.Log($"🎲 [Dice] Set top face to {faceName}");
    }

    public string GetTopFace() => topFace;

    private void ResetFaceMaterials()
    {
        foreach (Transform face in faces)
        {
            Renderer rend = face.GetComponent<Renderer>();
            if (rend != null && defaultMaterial != null)
                rend.material = defaultMaterial;
        }
    }

    /// <summary>
    /// 나중에 연출용으로 사용 가능 (지금은 구현 X)
    /// </summary>
    public void FakeRollToPosition(Vector3 targetPos)
    {
        // DOTween 연출용 자리 확보
    }
}
