using UnityEngine;

public class RedDevilAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.healthController.takeDame(GetComponentInParent<RedDevil>().damage);

        }
    }
}
