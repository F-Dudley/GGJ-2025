using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class GunInteractable : Interactable
{
    [SerializeField] private bool interactOnce;

    [SerializeField] private UnityEvent _event;

    [SerializeField] private BubbleGun bgun;

    public override void InteractStart(PlayerInteraction player)
    {

    }

    public override void InteractEnd(PlayerInteraction player)
    {
        bgun.transform.SetParent(player.GetBubbleHeldLocation());

        //bgun.transform.DOMove(new Vector3(0, 0, 0), 3).OnComplete();
    }
}