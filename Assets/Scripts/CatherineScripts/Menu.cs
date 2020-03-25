using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField]
    GameObject menuButtons;

    [SerializeField]
    Animator camAnimator;

    private void Start()
    {
        if(gameObject.name == "Menu")
        {
            StartCoroutine(menuWait(3));
        }
    }

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        switch(gameObject.name)
        {
            case "StartButton":
                if (collision.collider.CompareTag("Hands"))
                {
                    menuButtons.SetActive(false);
                    camAnimator.SetBool("GoFromMenuToWheel", true);
                }
                break;
            case "PotsButton":
                if (collision.collider.CompareTag("Hands"))
                {
                    menuButtons.SetActive(false);
                    Debug.Log("pots");
                    // Load pots
                }
                break;
            case "QuitButton":
                if (collision.collider.CompareTag("Hands"))
                {
                    Debug.Log("quit");
                    Application.Quit();
                }
                break;
            default:
                break;
        }

    }

    IEnumerator menuWait(float time)
    {
        yield return new WaitForSeconds(time);
        menuButtons.SetActive(true);
    }
}
