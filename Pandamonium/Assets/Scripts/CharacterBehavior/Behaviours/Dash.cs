using System.Collections;
using UnityEngine;

public class Dash: MonoBehaviour
{

    public float dashCooldown = 6;
    public float dashSpeed = 12;
    public float maxDashRange = 4;
    //protected bool dashed = false;
    protected float timeToDash;
    protected Transform dashingAt = null;
    protected float maxRaycastDistance = 50;
    private float stopDashingAt;

    public void Start()
    {
        timeToDash = dashCooldown;
    }
    
    public void Update()
    {
        if (timeToDash < dashCooldown)
        {
            timeToDash += Time.deltaTime;
        }
    }

    /*IEnumerator Dash(Vector3 to)
    {
        if (playerState == PlayerState.DASHING || timeToDash < dashCooldown)
            yield break;

        StopAttacking();
        stopDashingAt = 0;
        MoveToPosition(to);

        while (path.pathPending)
            yield return new WaitForEndOfFrame();

        if (playerState != PlayerState.WALKING)
            yield return null;

        playerState = PlayerState.DASHING;

        if (path.remainingDistance > maxDashRange)
        {
            stopDashingAt = path.remainingDistance - maxDashRange;
        }

        path.maxSpeed = dashSpeed;

        timeToDash = 0;
        yield return null;
    }
    IEnumerator Dash(Transform at)
    {
        yield return StartCoroutine(Dash(at.position + (transform.position - at.position).normalized * 1.5f));
        dashingAt = at;
        yield return null;
    }*/
}