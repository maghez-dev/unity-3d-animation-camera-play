using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAndShield : Weapon
{
    public GameObject player;
    public string layerName = "Sword And Shield";
    public GameObject sword;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    private PlayerCombat playerCombat;

    private Animator playerAnimator;

    void Start()
    {
        attackCombo = 3;
        if (player != null)
        {
            playerCombat = player.GetComponent<PlayerCombat>();
            playerAnimator = player.GetComponent<Animator>();

            animLayerIdx = playerAnimator.GetLayerIndex(layerName);
            transform.parent = playerCombat.rightHand;
        }
    }

    public override void SetUp(GameObject playerObj)
    {
        player = playerObj;

        playerCombat = player.GetComponent<PlayerCombat>();
        playerAnimator = player.GetComponent<Animator>();

        animLayerIdx = playerAnimator.GetLayerIndex(layerName);
        transform.parent = playerCombat.rightHand;
        transform.localPosition = positionOffset;
        transform.localRotation = Quaternion.Euler(rotationOffset);
    }
}
