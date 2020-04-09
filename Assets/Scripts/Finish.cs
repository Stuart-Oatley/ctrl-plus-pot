using UnityEngine;

public class Finish : MonoBehaviour { 
    private void OnCollisionEnter(Collision collision) {
        if(transform.parent.gameObject.activeSelf == true && collision.collider.CompareTag("Index")) {
            AnimationStateManager.MoveCamera(Position.painting);
        }
    }
}
