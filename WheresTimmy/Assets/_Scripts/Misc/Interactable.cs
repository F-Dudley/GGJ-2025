using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool interactOnce;

    [SerializeField] private UnityEvent _event;

    public void Interact(PlayerInteraction player)
    {
        InteractStart(player);

        _event.Invoke();

        InteractEnd(player);

        if (interactOnce)
            enabled = false;
    }

    public virtual void InteractStart(PlayerInteraction player) { }

    public virtual void InteractEnd(PlayerInteraction player) { }
}