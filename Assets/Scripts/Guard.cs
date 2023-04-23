using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Guard : MonoBehaviour
{
    public GameObject pickPocketTrigger;
    public GameObject enemyManager;
    private Animator animator;
    public Transform pathHolder;
    public float speed = 4.5f;
    public float waitTime = .3f;
    public float turnSpeed = 90f;
    public Light spotLight;
    public float viewDistance;
    public LayerMask viewMask;
    Color originalSpotlightColour;
    public float timeToSpotPlayer = 1.5f;
    private float OGTimeScale;
    private static int combatStarted = 0;
    float playerVisibleTimer;
    public GameObject obstacle;

    private float viewAngle;
    Transform player;
    public CombatScript combat;
    //private IEnumerator coroutine;
    private bool guardCanMove = true;

    void Start() {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle =spotLight.spotAngle;
        originalSpotlightColour = spotLight.color;
        OGTimeScale = Time.timeScale;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        //coroutine = FollowPath(waypoints);
        StartCoroutine(FollowPath(waypoints));
    }
    public void lightsOff() {
        spotLight.enabled = false;
    }
    void Update() {
        if(CanSeePlayer()) {
            playerVisibleTimer+= Time.deltaTime;
            //spotLight.color = Color.red;
        } else {
            playerVisibleTimer -= Time.deltaTime;
            //spotLight.color = originalSpotlightColor;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotLight.color = Color.Lerp(originalSpotlightColour, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if(playerVisibleTimer >= timeToSpotPlayer) {
            initiateCombat();
        }
    }
    public void startGuard() {
        guardCanMove = true;
    }
    public void stopGuard() {
        guardCanMove = false;
    }
    private void OnDrawGizmos() {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder) {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
    
    public void Freeze(bool value) {
        if(value) {
            //Time.fixedDeltaTime = 0;
        } else {
            //Time.fixedDeltaTime = OGTimeScale;
        }
    }

    IEnumerator FollowPath(Vector3[] waypoints) {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while(true) {
            if(guardCanMove) {
                if(combatStarted == 1) {
                    break;
                }
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
                //animator.SetFloat("InputMagnitude", speed / 5, 0.2f, Time.deltaTime);
                if(Vector3.Distance(transform.position, targetWaypoint) < 0.1f) {
                    //Debug.Log("ahhhhhhh");
                    animator.SetFloat("InputMagnitude", 0f, 0f, Time.deltaTime);
                    targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                    targetWaypoint = waypoints[targetWaypointIndex];
                    yield return new WaitForSeconds(waitTime);
                    yield return StartCoroutine(TurnToFace(targetWaypoint));
                    
                } else {
                    animator.SetFloat("InputMagnitude", speed / 5, 0.2f, Time.deltaTime);
                }
            }
                yield return null;
            //}
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget) {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    bool CanSeePlayer() {
        if(Vector3.Distance(transform.position, player.position) < viewDistance) {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndPlayer < viewAngle/2f) {
                if(!Physics.Linecast(transform.position, player.position, viewMask)) {
                    return true;
                }
            }
        }
        return false;
    }

    public void initiateCombat() {        
        if(Interlocked.CompareExchange(ref combatStarted, 1, 0) == 0) {
            //Debug.Log("aaaaahhh");
            enemyManager.SetActive(false);
            pickPocketTrigger.SetActive(false);
            combat.begin("GuardManager");
            GetComponentInParent<EnemyManager>().initiateGuardCombat();
            StartCoroutine(MoveUpwardsCoroutine(20, 5));
        }
    }

    private IEnumerator MoveUpwardsCoroutine(float distance, float duration)
    {
        Vector3 startPosition = obstacle.transform.position;
        Vector3 endPosition = obstacle.transform.position + Vector3.up * distance;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            yield return null;
        }

        obstacle.transform.position = endPosition;
    }
}
