using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [SerializeField] float baseStamina = 100;
    [SerializeField] float currentStamina;
    [SerializeField] Text staminaText;

    private void Start()
    {
        currentStamina = baseStamina;
        UpdateText();
    }

    public void SetBaseStamina(float changeAmount)
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
        currentStamina += changeAmount;
        UpdateText();
        if (currentStamina > 0)
            return true;
        else
            return false;
    }

    void UpdateText() { staminaText.text = "Stamina: " + currentStamina; }
}
