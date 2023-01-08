using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Class Setup")]
    public Transform rightHand;
    public Transform leftHand;

    [Header("Weapon Equipment")]
    public float equipmentSpeed = 2f;
    public GameObject currentWeaponObj = null;
    public int currentWeaponLayerIdx = 0;
    public float attackDelay = 1f;
    public float attackMoveSpeed = 1f;

    private PlayerInputManager playerInputManager;
    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private Weapon weapon;
    private bool canAttack;
    private int attackIdx;


    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        canAttack = true;
        attackIdx = 0;

        if (currentWeaponObj != null)
        {
            weapon = currentWeaponObj.GetComponent<Weapon>();
            StartCoroutine(ActivateLayer(weapon.GetLayerIdx()));
        }
    }

    void Update()
    {
        if (weapon == null)
            return;

        if (canAttack && playerInputManager.AttackKey)
        {
            AttackBegin();
        }
    }

    private IEnumerator AttackDelay(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    public void AttackBegin()
    {
        playerMovement.canMove = false;
        canAttack = false;

        playerAnimator.SetInteger("AttackIdx", attackIdx);
        playerAnimator.SetTrigger("Attack");

        attackIdx++;
        if (attackIdx >= weapon.attackCombo)
            attackIdx = 0;

        StartCoroutine(AttackDelay(attackDelay));
    }

    public void AttackEnd()
    {
        attackIdx = 0;
        playerMovement.canMove = true;
    }


    private IEnumerator ActivateLayer(int layerIdx)
    {
        float weight = 0f;
        while(playerAnimator.GetLayerWeight(layerIdx) < 1f)
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

    public void EquipWeapon(GameObject weaponPrefab)
    {
        GameObject weaponObj = Instantiate(weaponPrefab);
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        if (weapon != currentWeaponObj)
        {
            weapon.SetUp(this.gameObject);
            StartCoroutine(DectivateLayer(currentWeaponLayerIdx));
            StartCoroutine(ActivateLayer(weapon.GetLayerIdx()));
        }
        Destroy(currentWeaponObj);
        currentWeaponObj = weaponObj;
        this.weapon = weapon;
    }
}
