using UnityEngine;
using StarterAssets;
using UnityEditor.Build.Pipeline;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Iteraction Settings")]
    [SerializeField] Camera playerCamera;
    [SerializeField] private float interactDistance;
    [SerializeField] private float interactWidth;

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

        Vector3 screenPoint = new Vector3(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2, playerCamera.nearClipPlane);

        camCentrePos = playerCamera.ScreenToWorldPoint(screenPoint);

        if (Physics.Raycast(camCentrePos, playerCamera.transform.forward, out rhit, interactDistance, interactLayer))
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
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(camCentrePos, playerCamera.transform.forward * interactDistance);
    }
}
