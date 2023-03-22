using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Setup")]
    public GameObject cameraTarget;
    public GameObject cameraPosition;
    public AnimationRigs rigs;

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
    public float focusRange = 100f;
    public Vector3 focusOffset;
    public LayerMask layerMask;
    public GameObject currentTarget;

    private bool focusActive;
    public bool FocusActive
    {
        get { return focusActive;}
    }

    // Common variables

    private Vector3 currentOffset;


    private void Start()
    {
        transform.position = cameraPosition.transform.position;
        transform.LookAt(cameraTarget.transform);

        rotatedX = 0;
        rotatedY = 0;
        currentOffset = Vector3.zero;
        focusActive = false;
    }

    private void Update()
    {
        // Camera focus input

        if (Input.GetButtonDown("LockCamera"))
        {
            focusActive = !focusActive;
            LockClosestTarget(cameraTarget.transform, focusRange);

            if (focusActive)
                rigs.BeginLookTarget(currentTarget.transform.position);
            else
                rigs.EndLookTarget();
        }

        transform.position = Vector3.Lerp(
            transform.position,
            cameraPosition.transform.position + currentOffset,
            positionLerp * Time.deltaTime);

        if (focusActive)
        {
            FocusBehaviour();
        }
        else
        {
            StandardBehaviour();
        }
    }

    private void FocusBehaviour()
    {
        // Change target while locked

        if (Input.GetButtonDown("LockSwap"))
        {
            LockClosestTarget(cameraPosition.transform, focusRange);
        }

        // Computing focus camera offset

        Vector3 xComponent = cameraPosition.transform.right * focusOffset.x;
        Vector3 yComponent = cameraPosition.transform.up * focusOffset.y;
        Vector3 zComponent = cameraPosition.transform.forward * focusOffset.z;
        currentOffset = xComponent + yComponent + zComponent;

        // Look at focused enemy to focus

        Transform target = currentTarget.transform;

        Vector3 lookTarget = new Vector3(
            (target.position.x + cameraPosition.transform.position.x) / 2,
            (target.position.y + cameraPosition.transform.position.y) / 2,
            (target.position.z + cameraPosition.transform.position.z) / 2);

        cameraTarget.transform.LookAt(lookTarget);
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerp * Time.deltaTime);
    }


    private void StandardBehaviour()
    {
        // Computing standard camera offset

        Vector3 xComponent = cameraPosition.transform.right * standardOffset.x;
        Vector3 yComponent = cameraPosition.transform.up * standardOffset.y;
        Vector3 zComponent = cameraPosition.transform.forward * standardOffset.z;
        currentOffset = xComponent + yComponent + zComponent;

        // Computing rotation angles

        float rotX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float rotY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotatedX += rotX;
        rotatedY -= rotY;
        rotatedX %= 360;
        rotatedY %= 360;

        // Limit up-down rotation

        if (rotatedY > maxAngle)
            rotatedY = maxAngle;
        if (rotatedY < minAngle)
            rotatedY = minAngle;

        cameraTarget.transform.rotation = Quaternion.Euler(rotatedY, rotatedX, 0f);

        transform.LookAt(cameraTarget.transform);
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
}
