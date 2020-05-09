using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsMovementManager : MonoBehaviour
{
    public float footHeight = 6f;
    public float maxTargetDistance = 4f;
    public float targetMoveSpeed = 10f;
    public float moveSpeed = 2f;
    public float rotSpeed = 2f;
    public float groundOffset = 3f;

    public bool removeSpheres = false;

    public LegManager[] legManagers;
    public float upwardForce = 100f;

    private Vector3 targetVel;
    private float moveZ;
    private float rotY;
    private bool jump;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        legManagers = GetComponentsInChildren<LegManager>();
        for (int i = 0; i < legManagers.Length; i++)
        {
            legManagers[i].footHeight = footHeight;
            legManagers[i].maxTargetDistance = maxTargetDistance;
            legManagers[i].targetMoveSpeed = targetMoveSpeed;
            legManagers[i].oppositeLeg = i % 2 == 0 ? legManagers[i + 1] : legManagers[i - 1];
        }

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveZ = Input.GetAxis("Vertical");
        rotY = Input.GetAxis("Horizontal");
        jump = Input.GetButton("Jump");
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
        bool touchingGround = false;
        foreach (LegManager lm in legManagers)
        {
            Debug.DrawRay(lm.transform.position, -lm.transform.up * groundOffset, Color.red);
            if (Physics.Raycast(lm.transform.position, Vector3.down, out RaycastHit hit, groundOffset))
            {
                rb.AddForceAtPosition(Vector3.up * (groundOffset - hit.distance) * upwardForce * Time.fixedDeltaTime, lm.transform.position);
                touchingGround = true;
            }
        }

        if (touchingGround)
        {
            if (moveZ > .1f || moveZ < -.1f)
            {
                float forceMagnitude = moveSpeed * moveZ * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f);
                
                targetVel = transform.forward * forceMagnitude;

            }
            else
                targetVel = Vector3.zero;
            if (jump)
                rb.AddForce(transform.up * 2f, ForceMode.VelocityChange);
            targetVel.y = rb.velocity.y;
            rb.velocity = Vector3.Lerp(rb.velocity, targetVel, 5f * Time.fixedDeltaTime);
        }

        if (rotY > .1f || rotY < -.1f)
        {
            //rb.AddTorque(transform.up * rotY * rotSpeed, ForceMode.VelocityChange);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, transform.up * rotY * rotSpeed, 5f * Time.fixedDeltaTime);
        }

        Vector3 uprightForce = transform.up * -4000f;
        uprightForce.y = 0f;

        Debug.DrawRay(transform.position + transform.up * 2f, uprightForce, Color.cyan);

        rb.AddForceAtPosition(uprightForce, transform.position + transform.up * 2f);
    }
}
