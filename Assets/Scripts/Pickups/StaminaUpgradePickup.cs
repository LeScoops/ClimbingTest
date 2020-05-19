using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaUpgradePickup : MonoBehaviour
{
    [SerializeField] int staminaIncrease = 25;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Stamina>().SetBaseStamina(staminaIncrease);
            Destroy(this.gameObject);
        }
    }
}
