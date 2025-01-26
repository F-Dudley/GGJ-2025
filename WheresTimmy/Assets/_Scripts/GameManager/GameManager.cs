using BT;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Core Game")]

    [Header("Pick-Ups")]
    [SerializeField] private float aggressionMinIncrease = 17.5f;

    [Header("Cuddles")]
    [SerializeField] private Agent cuddlesAgent;

    public UnityEvent PartPickedUpEvent;

    private void Awake()
    {
        instance = this;
    }

    #region Parts

    public void PartPicked(BearPart partID)
    {
        cuddlesAgent.MinAggression += aggressionMinIncrease;

        PartPickedUpEvent?.Invoke();
    }

    #endregion
}
