
using UnityEngine;

/// <summary>
/// returns the camera from the Pots position to the main menu
/// </summary>
public class BackFromPotsButton : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        if (transform.parent.gameObject.activeSelf && collision.collider.CompareTag("Index")) {
            AnimationStateManager.MoveCamera(Position.menu);
        }
    }
}
