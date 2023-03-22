using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationRigs : MonoBehaviour
{
    [Header("Look at Target")]
    public Transform headAimTarget;
    public Rig headAimRig;

    private Vector3 currentTarget = Vector3.zero;

    [Header("Shield Up")]
    public Rig shieldUpRig;
    public BoxCollider shieldCollider;
    public float shieldUpLerp = .5f;

    private bool shieldToggle = false;
    private float timeElapsed = 0f;

    private void Update()
    {
        if (currentTarget != Vector3.zero)
        {
            headAimTarget.position = currentTarget;
        }

        if (shieldToggle)
        {
            if (timeElapsed < shieldUpLerp)
            {
                shieldUpRig.weight = Mathf.Lerp(0f, 1f, timeElapsed / shieldUpLerp);
                timeElapsed += Time.deltaTime;
            }
        }
        else
        {
            if (timeElapsed > 0f)
            {
                shieldUpRig.weight = Mathf.Lerp(0f, 1f, timeElapsed / shieldUpLerp);
                timeElapsed -= Time.deltaTime;
            }
        }
        shieldCollider.enabled = shieldToggle;
    }

    public void BeginLookTarget(Vector3 targetPosition)
    {
        headAimRig.weight = 1f;
        currentTarget = targetPosition;
    }

    public void EndLookTarget()
    {
        headAimRig.weight = 0f;
        currentTarget = Vector3.zero;
    }

    public void ShieldUp(bool val)
    {
        shieldToggle = val;
    }
}
