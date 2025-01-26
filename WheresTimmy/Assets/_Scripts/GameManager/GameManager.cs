using System.Collections.Generic;
using BT;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Core Game")]

    [Header("Pick-Ups")]
    [SerializeField] private int partsRemaining;
    [SerializeField] private float aggressionMinIncrease = 17.5f;

    [SerializeField] private Sprite headPartSprite;
    [SerializeField] private Sprite armPartSprite;
    [SerializeField] private Sprite bodyPartSprite;

    [Header("Cuddles")]
    [SerializeField] private Agent[] cuddlesAgents;

    [Header("Exit Objects")]
    [SerializeField] private GameObject winTrigggerVolume;

    public UnityEvent PartPickedUpEvent;

    private void Awake()
    {
        instance = this;

        partsRemaining = 0;

        winTrigggerVolume.SetActive(false);
    }

    #region Parts

    public void RegisterPart(BearPart partID)
    {
        partsRemaining++;
    }

    public void PartPicked(BearPart partID)
    {
        partsRemaining--;

        foreach (Agent cuddlesAgent in cuddlesAgents)
        {
            cuddlesAgent.MinAggression += aggressionMinIncrease;

            PartPickedUpEvent?.Invoke();
        }

        if (AllCollected())
        {
            winTrigggerVolume.SetActive(true);
        }
    }

    public bool AllCollected()
    {
        return partsRemaining == 0;
    }

    #endregion

    public void SendToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
