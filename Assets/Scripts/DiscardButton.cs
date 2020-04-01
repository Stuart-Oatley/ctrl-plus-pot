using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardButton : MonoBehaviour
{
    public delegate void DiscardEventHandler();
    public static event DiscardEventHandler Discard;
    private void OnCollisionEnter(Collision collision) {
        Discard?.Invoke();
    }
}
