using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Triggers the current pot to be saved
/// </summary>
public class SaveButton : MonoBehaviour {
    [SerializeField]
    GameObject pot;
    public delegate void SavePotEventHandler(SavePotEventArgs e);
    public static event SavePotEventHandler SavePot;

    private void OnCollisionEnter(Collision collision) {
        if (transform.parent.gameObject.activeSelf == true && collision.collider.CompareTag("Index")) {
            pot.GetComponent<Painter>().SaveTexture();
            SavePot?.Invoke(new SavePotEventArgs(pot));
            AnimationStateManager.MoveCamera(Position.menu);
        }
    }
}
