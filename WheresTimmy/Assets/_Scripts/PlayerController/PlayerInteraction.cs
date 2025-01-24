using UnityEngine;
using StarterAssets;

public class PlayerInteraction : MonoBehaviour
{
    Camera playerCamera;

    [Header("Iteraction Settings")]
    [SerializeField] private float interactDistance;
    [SerializeField] private float interactWidth;

    [SerializeField] private LayerMask interactLayer;

    private RaycastHit rhit;
    private Interactable fInteractable;

    private StarterAssetsInputs _input;

    private void OnAwake()
    {
        playerCamera = Camera.main;

        _input = GetComponent<StarterAssetsInputs>();
    }

    private void OnUpdate()
    {
        if (playerCamera == null)
        {
            Debug.Log("Player Camera not Found.");
            return;
        }

        Vector3 camCentre = playerCamera.ScreenToWorldPoint(new Vector3(playerCamera.pixelWidth / 2, playerCamera.pixelHeight / 2, playerCamera.nearClipPlane));

        if (Physics.Raycast(camCentre, playerCamera.transform.forward, out rhit, interactDistance, interactLayer))
        {
            if (rhit.collider.gameObject.TryGetComponent<Interactable>(out fInteractable))
            {
                // Something with the object.
            }
        }

        if (_input != null && _input.shoot)
        {
            Debug.Log("Shoot The Thing.");
        }
    }
}
