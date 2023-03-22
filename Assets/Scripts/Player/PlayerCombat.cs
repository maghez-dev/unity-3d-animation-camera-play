using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Class Setup")]
    public Transform rightHand;
    public Transform leftHand;
    public AnimationRigs rigs;
    public GameObject sword;
    public GameObject shield;

    [Header("Weapon Equipment")]
    public WeaponType currentWeapon = WeaponType.None;
    public float equipmentSpeed = 2f;
    public int currentWeaponLayerIdx = 0;
    public float attackDelay = 1f;
    public float attackMoveSpeed = 1f;

    private PlayerInputManager playerInputManager;
    private Animator playerAnimator;
    private bool canAttack;
    private int attackIdx;


    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerInputManager = GetComponent<PlayerInputManager>();
        canAttack = true;
        attackIdx = 0;

        if (currentWeapon != WeaponType.None)
        {
            EquipWeapon(currentWeapon);
        }
    }

    void Update()
    {
        if (currentWeapon == WeaponType.None)
            return;

        if ((playerInputManager.currentState.Equals(PlayerInputManager.State.Standard) || playerInputManager.currentState.Equals(PlayerInputManager.State.Attack)) &&
            canAttack && playerInputManager.attackKey)
        {
            AttackPerform();
        }

        rigs.ShieldUp(playerInputManager.currentState.Equals(PlayerInputManager.State.Standard) && playerInputManager.blockKey);
    }

    public void AttackPerform()
    {
        playerInputManager.currentState = PlayerInputManager.State.Attack;
        canAttack = false;

        playerAnimator.SetInteger("AttackIdx", attackIdx);
        playerAnimator.SetTrigger("Attack");

        // Handling a combo of 3 attacks
        attackIdx++;
        if (attackIdx >= 3)
            attackIdx = 0;

        StartCoroutine(AttackDelay(attackDelay));
    }

    private IEnumerator AttackDelay(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    // Called by animator to reset combo counter
    public void AttackEnd()
    {
        attackIdx = 0;
        playerInputManager.currentState = PlayerInputManager.State.Standard;
    }


    public void EquipWeapon(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.SwordAndShield:
                sword.SetActive(true);
                shield.SetActive(true);
                currentWeapon = weaponType;
                StartCoroutine(ActivateLayer(playerAnimator.GetLayerIndex("Sword And Shield")));
                break;
            case WeaponType.TwoHandSword: 
                break;
            default:
                break;
        }
    }

    private IEnumerator ActivateLayer(int layerIdx)
    {
        float weight = 0f;
        while (playerAnimator.GetLayerWeight(layerIdx) < 1f)
        {
            playerAnimator.SetLayerWeight(layerIdx, weight);
            weight += Time.deltaTime * equipmentSpeed;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private IEnumerator DectivateLayer(int layerIdx)
    {
        float weight = 0f;
        while (playerAnimator.GetLayerWeight(layerIdx) > 0f)
        {
            playerAnimator.SetLayerWeight(layerIdx, weight);
            weight -= Time.deltaTime * equipmentSpeed;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
