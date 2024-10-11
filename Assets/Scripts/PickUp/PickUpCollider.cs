using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUpCollider : MonoBehaviour
{
    public UnityEvent onPlayerInteractPickUp;

    public SelectCardMenu selectCardMenu;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerInteractPickUp.Invoke();
            Destroy(gameObject);
        }
    }
}
