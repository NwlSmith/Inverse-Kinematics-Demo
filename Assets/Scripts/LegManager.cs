using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegManager : MonoBehaviour
{
    public float footHeight = 6f;
    public float maxTargetDistance = 4f;
    public float targetMoveSpeed = 10f;
    public Transform stationaryTarget;
    public Transform movingTargetOrigin;
    public Transform debugSphere;
    public LegManager oppositeLeg;
    public bool moving = false;
    private Vector3 trueStatTarget;
    private IKBasic iKManager;

    private void Start()
    {
        iKManager = GetComponentInChildren<IKBasic>();
        if (iKManager == null)
            Debug.LogError("ERROR: " + name + " does not have a child IKBasic");
        if (stationaryTarget == null)
            stationaryTarget = new GameObject().transform;

        GetComponentInChildren<IKBasic>().target = stationaryTarget;
        trueStatTarget = stationaryTarget.position;

        if (debugSphere == null)
            debugSphere = new GameObject().transform;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    public void CalculateMovement()
    {

        Debug.DrawRay(movingTargetOrigin.position, -movingTargetOrigin.up * footHeight, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(movingTargetOrigin.position, -movingTargetOrigin.up, out hit, footHeight))
        {
            debugSphere.position = hit.point;
            if (Vector3.Distance(trueStatTarget, hit.point) >= maxTargetDistance)
            {
                trueStatTarget = hit.point + (hit.point - stationaryTarget.position).normalized * maxTargetDistance * Random.Range(0f, .5f);
            }
        }

        if (Vector3.Distance(trueStatTarget, stationaryTarget.position) < .1f || oppositeLeg.moving)
            moving = false;
        else
        {
            moving = true;
            stationaryTarget.position = Vector3.Slerp(stationaryTarget.position, trueStatTarget, targetMoveSpeed * Time.deltaTime);
        }
    }

    public float LeafHeight()
    {
        return iKManager.transform.position.y;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR

        if (movingTargetOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(movingTargetOrigin.position, .5f);
        }

        if (debugSphere != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(debugSphere.position, .5f);
        }

        if (trueStatTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(trueStatTarget, .5f);
        }

        Gizmos.DrawLine(trueStatTarget, stationaryTarget.position);
#endif
    }
}
