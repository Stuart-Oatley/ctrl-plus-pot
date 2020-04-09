using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFromPottery : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        if (transform.parent.gameObject.activeSelf && collision.collider.CompareTag("Index")) {
            AnimationStateManager.MoveCamera(Position.menu);
        }
    }
}
