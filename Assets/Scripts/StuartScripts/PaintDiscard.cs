using UnityEngine;

/// <summary>
/// discards the pot when at the painting stage
/// </summary>
public class PaintDiscard : MonoBehaviour
{
    public delegate void DiscardEventHandler();
    public static event DiscardEventHandler Discard;
    private void OnCollisionEnter(Collision collision) {
        if (transform.parent.gameObject.activeSelf == true && collision.collider.CompareTag("Index")) {
            Discard?.Invoke();
            AnimationStateManager.MoveCamera(Position.menu);
        }
    }
}
