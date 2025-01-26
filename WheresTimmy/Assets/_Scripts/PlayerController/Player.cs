using UnityEngine;
using StarterAssets;
using UnityEngine.Events;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int health;

    [SerializeField] private int maxHealth;
    [SerializeField] private List<GameObject> livesUI;

    [SerializeField] private UnityEvent<int> OnHealthChange;

    public void AddHealth(int healthIncr)
    {
        health = Mathf.Max(0, health + healthIncr);
        OnHealthChange?.Invoke(health);

        livesUI[health].SetActive(false);
    }
}