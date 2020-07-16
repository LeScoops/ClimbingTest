using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [SerializeField] Text staminaText;
    [SerializeField] float baseStamina = 100.0f;
    [SerializeField] float currentStamina;

    private void Start()
    {
        currentStamina = baseStamina;
        UpdateText();
    }

    public void SetBaseStamina(int changeAmount)
    {
        baseStamina += changeAmount;
        ResetStamina();
    }

    public void ResetStamina()
    {
        currentStamina = baseStamina;
        UpdateText();
    }

    // Returns true if there is stamina available and can use. Returns false if no stamina left. 
    public bool ApplyStaminaChangeIfAvailable(float changeAmount)
    {
        if (currentStamina + changeAmount > 0)
        {
            currentStamina += changeAmount;
            UpdateText();
            return true;
        }
        else
            return false;
    }

    public void RechargeStamina(float rechargeRate)
    {
        if (currentStamina < baseStamina)
            currentStamina += rechargeRate;
        else
            currentStamina = baseStamina;
        UpdateText();
    }

    void UpdateText() { staminaText.text = string.Format("Stamina: {0:#}", currentStamina); }
}
