using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    #region Variables
    public float height = 5f;
    public float distance = 10f;
    public float angle = 45f;
    public float lookAtHeight = 2f;
    public float smoothSpeed = 0.5f;

    private Vector3 refVelocity;

    public Transform target;
    #endregion Variables

    // Update is called once per frame
    void LateUpdate()
    {
        HandleCamera();
    }

    public void HandleCamera()
    {
        if (!target)
        {
            return;
        }

        // Build world position vector
        Vector3 worldPosition = (Vector3.forward * -distance) + (Vector3.up * height);
        //Debug.DrawLine(target.position, worldPosition, Color.red);

        // Build our rotated vector
        Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;
        //Debug.DrawLine(target.position, rotatedVector, Color.green);

        // Move our position
        Vector3 finalTargetPosition = target.position;
        finalTargetPosition.y += lookAtHeight;

        Vector3 finalPosition = finalTargetPosition + rotatedVector;
        //Debug.DrawLine(target.position, finalPosition, Color.blue);

        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, smoothSpeed);

        transform.LookAt(target.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, .5f);
        if (target)
        {
            Vector3 lookAtPosition = target.position;
            lookAtPosition.y += lookAtHeight;
            Gizmos.DrawLine(transform.position, lookAtPosition);
            Gizmos.DrawSphere(lookAtPosition, .25f);
        }

        Gizmos.DrawSphere(transform.position, .25f);
    }
}
