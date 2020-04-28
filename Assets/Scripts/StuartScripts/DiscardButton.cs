using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// triggers the discard event
/// </summary>
public class DiscardButton : MonoBehaviour
{
    public delegate void DiscardEventHandler();
    public static event DiscardEventHandler Discard;
    private void OnCollisionEnter(Collision collision) {
        if (transform.parent.gameObject.activeSelf == true && collision.collider.CompareTag("Index")) {
            Discard?.Invoke();
        }
    }
}
