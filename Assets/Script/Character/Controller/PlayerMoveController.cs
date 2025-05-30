using System.Collections;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private float baseSpeed;
    private Rigidbody2D rd;
    private Coroutine currentSpeedBoost;

    private void Start()
    {
        baseSpeed = GetComponent<Player>().speed;
        speed = baseSpeed;
        rd = GetComponent<Rigidbody2D>();
    }

    public bool MoveControl()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (ShopUI.Instance != null)
        {
            if (ShopUI.Instance.isOpenShop) return false;
        }
        Vector2 moveInput = new Vector2(horizontal, vertical);
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        rd.linearVelocity = moveInput.normalized * speed;

        return isMoving;
    }

    public void ApplySpeedBoost(float percentBoost, float duration)
    {
        if (currentSpeedBoost != null)
        {
            StopCoroutine(currentSpeedBoost);
        }

        currentSpeedBoost = StartCoroutine(SpeedBoostRoutine(percentBoost, duration));
    }

    private IEnumerator SpeedBoostRoutine(float percentBoost, float duration)
    {
        float multiplier = 1f + (percentBoost / 100f);
        speed = baseSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        speed = baseSpeed;
        currentSpeedBoost = null;
    }

    public void ResetSpeed()
    {
        if (currentSpeedBoost != null)
        {
            StopCoroutine(currentSpeedBoost);
            currentSpeedBoost = null;
        }
        speed = baseSpeed;
    }


}