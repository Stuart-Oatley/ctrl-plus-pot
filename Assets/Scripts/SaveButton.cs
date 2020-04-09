using UnityEngine;

public class SaveButton : MonoBehaviour {
    [SerializeField]
    GameObject pot;
    public delegate void SavePotEventHandler(SavePotEventArgs e);
    public static event SavePotEventHandler SavePot;

    private void OnCollisionEnter(Collision collision) {
        if (transform.parent.gameObject.activeSelf == true && collision.collider.CompareTag("Index")) {
            SavePot?.Invoke(new SavePotEventArgs(pot));
            AnimationStateManager.MoveCamera(Position.menu);
        }
    }
}
