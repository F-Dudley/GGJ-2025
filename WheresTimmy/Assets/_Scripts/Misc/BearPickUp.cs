using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using StarterAssets;

enum BearPart
{
    HEAD,
    L_ARM, BODY, R_ARM,
    L_LEG, R_LEG
}

public class BearPickUp : MonoBehaviour, IInteractable
{
    [Header("Pick-Up Settings")]
    [SerializeField] private BearPart partID = BearPart.HEAD;

    [SerializeField] private Transform partItem;

    [Header("Interaction Settings")]
    [SerializeField] private bool interactOnce;

    [SerializeField] private UnityEvent _onCollection;

    #region Unity Methods

    private void Awake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerInteraction>(out PlayerInteraction pInteraction))
        {
            Interact(pInteraction);
        }
    }

    public void Interact(PlayerInteraction player)
    {
        ApplyBearPickUp();
        PickUpAnimation(player);
    }

    #endregion

    #region Pick-Up Methods

    private void ApplyBearPickUp()
    {
        _onCollection.Invoke();

        // GameManager Add Bear Part.
        // Propogate Bear Part Event.
    }

    private void PickUpAnimation(PlayerInteraction player)
    {
        partItem.SetParent(player.GetBubbleHeldLocation());
        gameObject.SetActive(false);

        partItem.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() =>
        {
            DestoryItems();
        });
    }

    private void DestoryItems()
    {
        Destroy(gameObject);
        Destroy(partItem.gameObject);
    }

    #endregion
}