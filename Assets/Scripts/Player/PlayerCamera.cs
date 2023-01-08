using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Setup")]
    public GameObject positionPivot;
    public GameObject rotationPivot;

    [Header("Standard Behaviour")]
    public float positionLerp = 0.5f;
    public float rotationLerp = 0.5f;
    public float sensitivity = 2000f;
    public Vector3 standardOffset;
    [Range(-90, 0)] public float minAngle = -70f;
    [Range(0, 90)] public float maxAngle = 70f;

    private float rotatedX;
    private float rotatedY;

    [Header("Focus Behaviour")]
    public float lockOnRange = 100f;
    public Vector3 lockedOffset;
    public LayerMask layerMask;

    private bool lockedCamera;
    private GameObject currentTarget;

    // Common variables

    private Vector3 currentOffset;


    private void Start()
    {
        transform.position = positionPivot.transform.position;
        transform.LookAt(rotationPivot.transform);

        rotatedX = 0;
        rotatedY = 0;
        currentOffset = Vector3.zero;
        lockedCamera = false;
    }

    private void Update()
    {
        // Camera focus input

        if (Input.GetButtonDown("LockCamera"))
        {
            lockedCamera = !lockedCamera;
            LockClosestTarget(rotationPivot.transform, lockOnRange);
        }

        transform.position = Vector3.Lerp(
            transform.position, 
            positionPivot.transform.position + currentOffset, 
            positionLerp * Time.deltaTime);

        if (lockedCamera)
        {
            LockOnRotation();
        }
        else
        {
            LockOffRotation();
        }
    }

    private void LockOnRotation()
    {
        // Change target while locked

        if (Input.GetButtonDown("LockSwap"))
        {
            LockClosestTarget(rotationPivot.transform, lockOnRange);
        }

        // Computing target locked offset

        Vector3 xComponent = positionPivot.transform.right * lockedOffset.x;
        Vector3 yComponent = positionPivot.transform.up * lockedOffset.y;
        Vector3 zComponent = positionPivot.transform.forward * lockedOffset.z;
        currentOffset = xComponent + yComponent + zComponent;

        // Setting locked off rotation for correct mode swap translation

        rotatedX = rotationPivot.transform.localEulerAngles.y;
        rotatedY = rotationPivot.transform.localEulerAngles.x;

        // Look at focused enemy to focus

        Transform target = currentTarget.transform;

        Vector3 lookTarget = new Vector3(
            (target.position.x + rotationPivot.transform.position.x) / 2,
            (target.position.y + rotationPivot.transform.position.y) / 2,
            (target.position.z + rotationPivot.transform.position.z) / 2);

        rotationPivot.transform.LookAt(lookTarget);

        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerp * Time.deltaTime);
    }


    private void LockOffRotation()
    {
        // Computing standard camera offset

        Vector3 xComponent = positionPivot.transform.right * standardOffset.x;
        Vector3 yComponent = positionPivot.transform.up * standardOffset.y;
        Vector3 zComponent = positionPivot.transform.forward * standardOffset.z;
        currentOffset = xComponent + yComponent + zComponent;

        // Computing rotation angles

        float rotX = Input.GetAxis("Mouse X") * sensitivity * sensitivity * Time.deltaTime;
        float rotY = Input.GetAxis("Mouse Y") * sensitivity * sensitivity * Time.deltaTime;
        rotatedX += rotX;
        rotatedY -= rotY;
        rotatedX %= 360;
        rotatedY %= 360;

        // Limit up-down rotation

        if (rotatedY > maxAngle)
            rotatedY = maxAngle;
        if (rotatedY < minAngle)
            rotatedY = minAngle;

        rotationPivot.transform.localRotation = Quaternion.Euler(rotatedY, rotatedX, 0f);
        transform.LookAt(rotationPivot.transform);
    }

    // Functions that searches for the closest target in a given range from a given position to set in "currentTarget"
    private void LockClosestTarget(Transform start, float range)
    {
        float minDistance = range;
        GameObject bestMatch = null;

        Collider[] colliders = Physics.OverlapSphere(start.transform.position, range, layerMask);
        foreach (Collider collider in colliders)
        {
            float currentDistance = Vector3.Distance(start.transform.position, collider.transform.position);

            if (currentDistance < minDistance && collider.gameObject != start.gameObject)
            {
                minDistance = currentDistance;
                bestMatch = collider.gameObject;
            }
        }

        currentTarget = bestMatch;
    }

    public bool IsLocked()
    {
        return lockedCamera;
    }
}
