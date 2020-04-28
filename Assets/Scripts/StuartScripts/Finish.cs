using UnityEngine;

/// <summary>
/// Moves the camera from the pottery position to the painting position
/// </summary>
public class Finish : MonoBehaviour { 
    private void OnCollisionEnter(Collision collision) {
        if(transform.parent.gameObject.activeSelf == true && collision.collider.CompareTag("Index")) {
            AnimationStateManager.MoveCamera(Position.painting);
        }
    }
}
