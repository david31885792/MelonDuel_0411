using UnityEngine;

public class UIBlocker : MonoBehaviour
{
    public static UIBlocker Instance;

    [SerializeField] private GameObject blindPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        blindPanel.SetActive(true);
    }

    public void Hide()
    {
        blindPanel.SetActive(false);
    }
}
