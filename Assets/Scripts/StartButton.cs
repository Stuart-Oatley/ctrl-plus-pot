using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    GameObject menuButtons;

    [SerializeField]
    Animator camAnimator;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Hands"))
        {
            menuButtons.SetActive(false);
            camAnimator.SetBool("GoFromMenuToWheel", true);
        }
    }
}
