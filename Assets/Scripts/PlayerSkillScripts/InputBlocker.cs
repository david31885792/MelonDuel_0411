using UnityEngine;

public class InputBlocker : MonoBehaviour
{
    public static InputBlocker Instance;
    private bool isBlocked = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Block()
    {
        isBlocked = true;
    }

    public void Unblock()
    {
        isBlocked = false;
    }

    public bool IsBlocked()
    {
        return isBlocked;
    }
}
