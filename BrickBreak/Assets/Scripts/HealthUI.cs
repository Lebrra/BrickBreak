using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    int currentHealth = 0;
    List<GameObject> healthUIs = new List<GameObject>();

    [SerializeField]
    Transform healthContainer;
    [SerializeField]
    GameObject healthPrefab;

    private void Start()
    {
        GameManager.OnGameStart += SetFullHealth;
        GameManager.OnBallLost += DecrementHealth;
    }

    void SetFullHealth()
    {
        for (int i = currentHealth; i < GameProperties.StartingHealthAmount; i++)
        {
            AddHealth();
        }
    }

    void DecrementHealth()
    {
        if (currentHealth > 0)
            RemoveHealth();
        else Debug.LogWarning("Cannot update UI - health is 0!");
    }

    void AddHealth()
    {
        currentHealth++;
        if (healthUIs.Count < currentHealth)
        {
            var newUI = Instantiate(healthPrefab, healthContainer);
            healthUIs.Add(newUI);
        }
        else
        {
            // turn it on
            healthUIs[currentHealth - 1].SetActive(true);
        }
    }

    void RemoveHealth()
    {
        currentHealth--;
        healthUIs[currentHealth].SetActive(false);
    }
}
