using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private float horizontal;
    private float forward;
    private bool rollKey;
    private bool blockKey;
    private bool attackPrimaryKey;
    private bool attackSecondaryKey;
    private bool aimKey;


    public float Horizontal
    {
        get { return horizontal; }
    }
    public float Forward
    {
        get { return forward; }
    }
    public bool BlockKey
    {
        get { return blockKey; }
    }
    public bool AttackKey
    {
        get { return attackPrimaryKey; }
    }
    public bool AimKey
    {
        get { return aimKey; }
    }
    public bool RollKey
    {
        get { return rollKey; }
    }

    public bool canMove = true;
    public bool canRoll = true;
    public bool isRolling = false;


    void LateUpdate()
    {
        if (canMove)
        {
            horizontal = Input.GetAxis("Horizontal");
            forward = Input.GetAxis("Vertical");
        }
        else
        {
            horizontal = 0;
            forward = 0;
        }
        
        if (canRoll)
            rollKey = Input.GetButtonDown("Roll");
        else
            rollKey = false;

        blockKey = Input.GetButton("Block");
        attackPrimaryKey = Input.GetButtonDown("Attack");
        //attackSecondaryKey = Input.GetButton("Attack2");
        aimKey = Input.GetButton("Aim");
    }
}
