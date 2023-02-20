using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public float horizontal;
    public float forward;
    public bool rollKey;
    public bool attackKey;

    public State currentState;


    public enum State
    {
        Stop,           // Player can't do anything, unable to control character
        Standard,       // Player can move and initiate other actions
        Roll,           // Player is performing a roll action
        Attack          // Player is performing an attack action
    }

    private void Start()
    {
        currentState = State.Standard;
    }

    void LateUpdate()
    {
        // Input check

        if (currentState.Equals(State.Standard))
        {
            horizontal = Input.GetAxis("Horizontal");
            forward = Input.GetAxis("Vertical");
            rollKey = Input.GetButtonDown("Roll");
            attackKey = Input.GetButtonDown("Attack");
        }
        else
        {
            horizontal = 0;
            forward = 0;
            rollKey = false;
            attackKey = false;
        }
    }
}
