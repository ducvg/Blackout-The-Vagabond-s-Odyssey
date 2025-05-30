using System.Collections;
using UnityEngine;

public class KnightSkill : BaseSkill
{
    public Animator animator;
    public Animator shieldAnimator;

    //clone
    public GameObject clonePrefab;
    public float cloneCoolDown;
    public int NumberSpawn;
    public float ultimateCooldown;
    public LayerMask obstacleMask;

    //shield
    public float normalCoolDown;
    public float shieldCoolDown;
    public int shieldDame = 5;

    public bool isUsingNormal = false;
    public bool isUsingUltimate = false;
    public bool canUseNormal = true;
    public bool canUseUltimate = true;

    public PlayerWeaponController weaponController;

    private void Start()
    {
        clonePrefab.GetComponent<CloneKnight>().knightSkill = this;
        animator = GetComponent<Animator>();

    }
    public override bool CanUseSkill1()
    {
        return canUseNormal;
    }

    public override bool CanUseSkill2()
    {
        return canUseUltimate;
    }

    public override bool IsUsingSkill()
    {
        return isUsingNormal || isUsingUltimate;
    }

    //public override void NormalSkill()
    //{
    //    canUseNormal = false;
    //    animator.SetBool("isSkill1", false);
    //    float radius = 2f;
    //    Vector3 center = transform.root.position; 

    //    float angle = Random.Range(0f, 2f * Mathf.PI);

    //    Vector3 spawnPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

    //    GameObject cloneKnight = PoolManagement.Instance.GetBullet(clonePrefab);
    //    cloneKnight.transform.position = center + spawnPosition;
    //    StartCoroutine(ResetNormalSkill());
    //}

    public override void NormalSkill()
    {
        GameManager.Instance.UpdateNormalSkillUsed();
        //isUsingNormal = true; 
        canUseNormal = false;
        animator.SetBool("isSkill1", false);
        shieldAnimator.SetTrigger("isUsingShield");
        transform.root.GetComponent<PlayerArmorController>().ShieldSkill();
        //bat dau cooldown
        SkillCooldownUI.Instance.TriggerCooldown_E(normalCoolDown);

        StartCoroutine(ResetShield());

    }


    IEnumerator ResetNormalSkill()
    {

        yield return new WaitForSeconds(normalCoolDown);
        canUseNormal = true;
    }

    IEnumerator ResetShield()
    {
        StartCoroutine(ResetNormalSkill());
        yield return new WaitForSeconds(shieldCoolDown);
        GetComponentInChildren<ShieldHitBox>().shieldDame = transform.root.GetComponent<PlayerArmorController>().DameTakenDuringShieldSkill();
        shieldAnimator.SetTrigger("isShieldDone");
        transform.root.GetComponent<PlayerArmorController>().EndShieldSkill();
        
    }

    IEnumerator ResetUltimateSkill()
    {
        yield return new WaitForSeconds(ultimateCooldown);
        canUseUltimate = true;
    }

    public override void UltimmateSkill()
    {
        GameManager.Instance.UpdateUltimateSkillUsed();
        canUseUltimate = false;
        animator.SetBool("isSkill2", false);
        //bat dau cooldown
        SkillCooldownUI.Instance.TriggerCooldown_Q(ultimateCooldown);

        float radius = 2f;
        Vector3 center = transform.position;
        int maxAttempts = 10;

        Vector3 spawnPosition = center;
        for (int spawn = 0; spawn < NumberSpawn; spawn++)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                float angle = Random.Range(0f, 1f * Mathf.PI);
                Vector3 tempPosition = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;


                if (!Physics2D.OverlapCircle(tempPosition, 0.5f, obstacleMask))
                {
                    spawnPosition = tempPosition;

                    break;
                }
            }
            GameObject cloneKnight = PoolManagement.Instance.GetBullet(clonePrefab);
            cloneKnight.transform.position = spawnPosition;
            cloneKnight.GetComponent<CloneKnight>().SetWeaponForClone(weaponController.baseWeapon.gameObject);
        }
        StartCoroutine(ResetUltimateSkill());
    }

    public override void SetActiveWeapon()
    {
        weaponController.gameObject.SetActive(true);
    }
    public override void setActiveFalseWeapon()
    {
        weaponController.gameObject.SetActive(false);
    }
    public override void SetIsUsingNormalFalse()
    {
        isUsingNormal = false;
    }
    public override void SetIsUsingUltimateFalse()
    {
        isUsingUltimate = false;
    }

    public override void SetIsUsingUltimate()
    {
        isUsingUltimate = true;
    }

    public override void SetIsUsingNormal()
    {
        isUsingNormal = true;
    }
}
