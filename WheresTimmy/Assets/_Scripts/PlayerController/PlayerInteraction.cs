using UnityEngine;
using StarterAssets;
using UnityEditor.Build.Pipeline;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Iteraction Settings")]
    [SerializeField] Camera playerCamera;
    [SerializeField] private float interactDistance;
    [SerializeField] private float interactSpread;

    [SerializeField] private LayerMask interactLayer;

    private RaycastHit rhit;
    private Interactable fInteractable;

    private StarterAssetsInputs _input;

    [Header("Interaction Debug")]
    private Vector3 camCentrePos;

    [Header("Bubble Gun")]
    [SerializeField] private Transform bgunHeldLocation;
    [SerializeField] private BubbleGun bgun;

    private void Awake()
    {
        playerCamera = Camera.main;

        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (playerCamera == null)
        {
            Debug.Log("Player Camera not Found.");
            return;
        }

        if (Physics.SphereCast(camCentrePos, interactSpread / 2, playerCamera.transform.forward, out rhit, interactDistance, interactLayer))
        {
            InteractableInteraction();
        }

        if (bgun != null)
            BubbleGunInteraction();
    }

    #region Interactable Interaction

    private void InteractableInteraction()
    {
        if (rhit.collider.TryGetComponent<Interactable>(out fInteractable) && fInteractable.enabled)
        {
            fInteractable.Interact(this);
        }

        fInteractable = null;
    }

    #endregion

    #region Bubble Gun

    public Transform GetBubbleHeldLocation()
    {
        return bgunHeldLocation;
    }
    private void BubbleGunInteraction()
    {
        if (_input != null && _input.shoot)
            bgun.FireGun();
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 centreCam = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, playerCamera.nearClipPlane));

        Vector3 sphereForwardOffset = playerCamera.transform.forward * interactDistance;

        Gizmos.DrawWireSphere(centreCam, interactSpread / 2);
        Gizmos.DrawWireSphere(centreCam + sphereForwardOffset, interactSpread / 2);

        // Wires
        Vector3 sphereHalfOffset = (playerCamera.transform.up * interactSpread / 2);

        // Top
        Gizmos.DrawLine(centreCam + sphereHalfOffset, (centreCam + sphereHalfOffset) + sphereForwardOffset);

        // Bot
        Gizmos.DrawLine(centreCam - sphereHalfOffset, (centreCam - sphereHalfOffset) + sphereForwardOffset);
    }
}
