using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    private MovementInput movementInput;
    private CombatScript combatScript;

    public LayerMask enemyLayerMask;
    public LayerMask obstacleLayerMask;

    [SerializeField] Vector3 inputDirection;
    [SerializeField] private EnemyScript currentTarget;

    public GameObject cam;

    private void Start()
    {
        movementInput = GetComponentInParent<MovementInput>();
        combatScript = GetComponentInParent<CombatScript>();
    }

    private void Update()
    {
        var camera = Camera.main;
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        inputDirection = forward * movementInput.moveAxis.y + right * movementInput.moveAxis.x;
        inputDirection = inputDirection.normalized;

        // Check for obstacles in the way
        if (Physics.Raycast(transform.position, inputDirection, out RaycastHit obstacleInfo, Mathf.Infinity, obstacleLayerMask))
        {
            currentTarget = null; // Reset current target if there's an obstacle in the way
        }
        else if (Physics.SphereCast(transform.position, 3f, inputDirection, out RaycastHit enemyInfo, 10, enemyLayerMask))
        {
            if (enemyInfo.transform.GetComponent<EnemyScript>().IsAttackable())
            {
                currentTarget = enemyInfo.transform.GetComponent<EnemyScript>();
            }
        }
    }

    public EnemyScript CurrentTarget()
    {
        return currentTarget;
    }

    public void SetCurrentTarget(EnemyScript target)
    {
        currentTarget = target;
    }

    public float InputMagnitude()
    {
        return inputDirection.magnitude;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, inputDirection);
        Gizmos.DrawWireSphere(transform.position, 1);
        if (CurrentTarget() != null)
        {
            Gizmos.DrawSphere(CurrentTarget().transform.position, .5f);
        }
    }
}