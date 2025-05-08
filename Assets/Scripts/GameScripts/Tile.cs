using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("Ÿ�� ��ġ")]
    public int x;
    public int y;

    [Header("Ÿ�� �̸�")]
    public string tileName;

    [Header("Ÿ�� �̹���")]
    public Image tileImage;

    [Header("���� ���� (�̸��� �� ���� ��ġ)")]
    [SerializeField]
    private string[] colorNames = { "Red", "Blue", "Yellow", "Green", "Orange", "White" };

    [SerializeField]
    private Color[] colorValues;

    /// <summary>
    /// Ÿ�� �ʱ�ȭ (��ǥ, �̸� ����)
    /// </summary>
    public void Initialize(int x, int y, string name)
    {
        this.x = x;
        this.y = y;
        UpdateColor(name);
    }

    /// <summary>
    /// ���� �̸��� ���� �̹��� ���� ����
    /// </summary>
    public void UpdateColor(string name)
    {
        tileName = name;

        for (int i = 0; i < colorNames.Length && i < colorValues.Length; i++)
        {
            if (colorNames[i].Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                if (tileImage != null)
                    tileImage.color = colorValues[i];
                return;
            }
        }

        // ��Ī ���� �� �⺻��
        if (tileImage != null)
            tileImage.color = Color.black;
    }
}
