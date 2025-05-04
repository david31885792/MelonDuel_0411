using UnityEngine;

public class DiceInputController : MonoBehaviour
{
    public Dice dice;

    [Header("입력 감도")]
    public bool enableTouch = true;
    public bool enableShake = true;
    public float shakeThreshold = 3.0f;

    private Vector3 acceleration;
    private bool inputBlocked = false;

    void Update()
    {
        if (dice == null || dice.IsRolling || inputBlocked) return;

        if (enableTouch && Input.GetMouseButtonDown(0))
        {
            TryRoll();
        }

        if (enableShake)
        {
            acceleration = Input.acceleration;
            if (acceleration.sqrMagnitude > shakeThreshold * shakeThreshold)
            {
                TryRoll();
            }
        }
    }

    private void TryRoll()
    {
        if (dice != null && !dice.IsRolling)
        {
            inputBlocked = true;
            dice.Roll();
            // 입력 차단 해제는 dice에서 OnRollCompleted 받아서 해도 됨
            dice.OnRollCompleted += (_) => inputBlocked = false;
        }
    }
}
