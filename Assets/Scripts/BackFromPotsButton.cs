
using UnityEngine;

public class BackFromPotsButton : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        if (transform.parent.gameObject.activeSelf && collision.collider.CompareTag("Index")) {
            AnimationStateManager.MoveCamera(Position.menu);
        }
    }
}
