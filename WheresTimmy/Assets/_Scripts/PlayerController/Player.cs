using UnityEngine;
using StarterAssets;
using UnityEditor.Build.Pipeline;
using UnityEditor.UI;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int health;

    [SerializeField] private int maxHealth;

    [SerializeField] private UnityEvent<int> OnHealthChange;

    public void AddHealth(int healthIncr)
    {
        health = Mathf.Max(0, health + healthIncr);
        OnHealthChange?.Invoke(health);
    }
}