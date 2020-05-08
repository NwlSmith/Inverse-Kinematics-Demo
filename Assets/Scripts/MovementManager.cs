using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public float footHeight = 6f;
    public float maxTargetDistance = 4f;
    public float targetMoveSpeed = 10f;
    public float moveSpeed = 2f;
    public float rotSpeed = 2f;
    public float groundOffset = 3f;

    public bool removeSpheres = false;

    private float moveZ;
    private float rotY;
    public LegManager[] legManagers;

    // Start is called before the first frame update
    void Start()
    {
        legManagers = GetComponentsInChildren<LegManager>();

        foreach (LegManager legManager in legManagers)
        {
            legManager.footHeight = footHeight;
            legManager.maxTargetDistance = maxTargetDistance;
            legManager.targetMoveSpeed = targetMoveSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveZ = Input.GetAxis("Vertical");
        rotY = Input.GetAxis("Horizontal");
        if (removeSpheres)
        {
            foreach (LegManager legManager in legManagers)
            {
                legManager.stationaryTarget.GetComponent<MeshRenderer>().enabled = false;
                legManager.GetComponentInChildren<IKBasic>().pole.GetComponent<MeshRenderer>().enabled = false;
                legManager.movingTargetOrigin.GetComponent<MeshRenderer>().enabled = false;
                legManager.debugSphere.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // Calculate new height
        Vector3 newPos = transform.position + transform.forward * moveZ * moveSpeed * Time.fixedDeltaTime;
        newPos.y = 0f;
        newPos.y += groundOffset;
        foreach (LegManager legManager in legManagers)
        {
            newPos.y += legManager.LeafHeight() / legManagers.Length;
        }

        transform.position = newPos;
        float x, y, z;
        x = ((legManagers[2].LeafHeight()) + (legManagers[3].LeafHeight()) / 2) - ((legManagers[0].LeafHeight()) + (legManagers[1].LeafHeight()) / 2);
        x *= 5f;
        y = transform.rotation.eulerAngles.y + rotY * rotSpeed * Time.fixedDeltaTime;
        z = 0f;
        Vector3 newRot = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(newRot);


    }
}
