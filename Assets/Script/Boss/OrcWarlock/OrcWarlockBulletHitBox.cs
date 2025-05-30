using UnityEngine;

public class OrcWarlockBulletHitBox : MonoBehaviour
{
    private int damage;
    public BulletIdentifier BulletIdentifier;
    public OrcWarlock OrcWarlock;
    private void Start()
    {
       // GameObject obj = GameObject.Find("OrcWarlock(Clone)").GetComponent<OrcWarlock>().gameObject;
        

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var player))
        {
            player.takeDame(OrcWarlock.damage);
            PoolManagement.Instance.ReturnBullet(gameObject, BulletIdentifier.bulletPrefabReference);
        }
        PoolManagement.Instance.ReturnBullet(gameObject, BulletIdentifier.bulletPrefabReference);
    }
}
