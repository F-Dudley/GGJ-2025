using System.Collections.Generic;
using UnityEngine;

public class TriggerWinBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            GameManager.instance.SendToMainMenu();
        }
    }
}