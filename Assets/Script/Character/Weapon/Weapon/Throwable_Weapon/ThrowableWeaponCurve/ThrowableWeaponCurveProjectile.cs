using System.Collections;
using UnityEngine;

public class ThrowableWeaponCurveProjectile : MonoBehaviour
{
    [SerializeField] private ThrowableWeaponCurveVisual projectileVisual;

    private Vector3 target;
    private float moveSpeed;
    private float maxMoveSpeed;
    private float trajectoryMaxRelativeHeight;
    private float distanceToTargetToDestroyProjectile = 1f;

    private AnimationCurve trajectoryAnimationCurve;
    private AnimationCurve axisCorrectionAnimationCurve;
    private AnimationCurve projectileSpeedAnimationCurve;

    private Vector3 trajectoryStartPoint;
    private Vector3 projectileMoveDir;
    private Vector3 trajectoryRange;

    private float nextYTrajectoryPosition;
    private float nextXTrajectoryPosition;
    private float nextPositionYCorrectionAbsolute;
    private float nextPositionXCorrectionAbsolute;

    public ThrowableWeaponCurve curveManager;

    private bool isTurnOn = false;

    private void Start()
    {

        trajectoryStartPoint = transform.position;
    }

    private void Update()
    {
        UpdateProjectilePosition();

        if (Vector3.Distance(transform.position, target) < distanceToTargetToDestroyProjectile)
        {
            moveSpeed = 0f;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
            if (!isTurnOn)
            {
                curveManager.TurnOnSpriteRenderer();
                isTurnOn = true;
            }
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(true);
                StartCoroutine(ReturnToPoolWithDelay(gameObject, curveManager.projectilePrefab, 3f));
            }
            //Destroy(gameObject);
        }
    }

    private IEnumerator ReturnToPoolWithDelay(GameObject gameobject, GameObject gameObjectPrefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        //PoolManagement.Instance.ReturnBullet(gameObject, curveManager.projectilePrefab);
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(false);
            StartCoroutine(ReturnToPoolWithDelay(gameObject, curveManager.projectilePrefab, 3f));
        }
        PoolManagement.Instance.ReturnBullet(gameobject, gameObjectPrefab);
    }


    private void UpdateProjectilePosition()
    {
        trajectoryRange = target - trajectoryStartPoint;

        if (Mathf.Abs(trajectoryRange.normalized.x) < Mathf.Abs(trajectoryRange.normalized.y))
        {
            // Projectile will be curved on the X axis
            if (trajectoryRange.y < 0)
            {
                // Target is located under shooter
                moveSpeed = -moveSpeed;
            }
            UpdatePositionWithXCurve();
        }
        else
        {
            // Projectile will be curved on the Y axis
            if (trajectoryRange.x < 0)
            {
                // Target is located behind shooter
                moveSpeed = -moveSpeed;
            }
            UpdatePositionWithYCurve();
        }
    }

    private void UpdatePositionWithXCurve()
    {
        float nextPositionY = transform.position.y + moveSpeed * Time.deltaTime;
        float nextPositionYNormalized = (nextPositionY - trajectoryStartPoint.y) / trajectoryRange.y;

        float nextPositionXNormalized = trajectoryAnimationCurve.Evaluate(nextPositionYNormalized);
        nextXTrajectoryPosition = nextPositionXNormalized * trajectoryMaxRelativeHeight;

        float nextPositionXCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionYNormalized);
        nextPositionXCorrectionAbsolute = nextPositionXCorrectionNormalized * trajectoryRange.x;

        if (trajectoryRange.x > 0 && trajectoryRange.y > 0)
        {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }
        if (trajectoryRange.x < 0 && trajectoryRange.y < 0)
        {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }

        float nextPositionX = trajectoryStartPoint.x + nextXTrajectoryPosition + nextPositionXCorrectionAbsolute;
        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateNextProjectileSpeed(nextPositionYNormalized);
        projectileMoveDir = newPosition - transform.position;

        transform.position = newPosition;
    }

    private void UpdatePositionWithYCurve()
    {
        float nextPositionX = transform.position.x + moveSpeed * Time.deltaTime;
        float nextPositionXNormalized = (nextPositionX - trajectoryStartPoint.x) / trajectoryRange.x;
        float nextPositionYNormalized = trajectoryAnimationCurve.Evaluate(nextPositionXNormalized);
        nextYTrajectoryPosition = nextPositionYNormalized * trajectoryMaxRelativeHeight;
        float nextPositionYCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionXNormalized);
        nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;
        float nextPositionY = trajectoryStartPoint.y + nextYTrajectoryPosition + nextPositionYCorrectionAbsolute;
        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);
        CalculateNextProjectileSpeed(nextPositionXNormalized);
        projectileMoveDir = newPosition - transform.position;
        transform.position = newPosition;
    }

    private void CalculateNextProjectileSpeed(float normalizedValue)
    {
        float nextMoveSpeedNormalized = projectileSpeedAnimationCurve.Evaluate(normalizedValue);
        moveSpeed = nextMoveSpeedNormalized * maxMoveSpeed;
    }

    public void InitializeProjectile(Vector3 target, float maxMoveSpeed, float trajectoryMaxHeight)
    {
        this.target = target;
        this.maxMoveSpeed = maxMoveSpeed;
        float xDistanceToTarget = target.x - transform.position.x;
        this.trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;

        trajectoryStartPoint = transform.position;

        projectileVisual.SetTarget(target);
    }

    public void InitializeAnimationCurves(AnimationCurve trajectoryAnimationCurve, AnimationCurve axisCorrectionAnimationCurve, AnimationCurve projectileSpeedAnimationCurve)
    {
        this.trajectoryAnimationCurve = trajectoryAnimationCurve;
        this.axisCorrectionAnimationCurve = axisCorrectionAnimationCurve;
        this.projectileSpeedAnimationCurve = projectileSpeedAnimationCurve;
    }

    public Vector3 GetProjectileMoveDir()
    {
        return projectileMoveDir;
    }

    public float GetNextYTrajectoryPosition()
    {
        return nextYTrajectoryPosition;
    }

    public float GetNextPositionYCorrectionAbsolute()
    {
        return nextPositionYCorrectionAbsolute;
    }

    public float GetNextXTrajectoryPosition()
    {
        return nextXTrajectoryPosition;
    }

    public float GetNextPositionXCorrectionAbsolute()
    {
        return nextPositionXCorrectionAbsolute;
    }
}
