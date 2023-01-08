using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public GameObject weaponPrefab;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat = other.GetComponent<PlayerCombat>();
            playerCombat.EquipWeapon(weaponPrefab);
        }
    }
}
