using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField]
    GameObject menuButtons;

    [SerializeField]
    Animator camAnimator;

    [SerializeField]
    AnimationClip cameraMovement;

    public delegate void StartPotteryEventHandler(float delayTime);
    public static event StartPotteryEventHandler StartPottery;

    private void OnCollisionEnter(Collision collision)
    {
        switch(gameObject.name)
        {
            case "StartButton":
                if (collision.collider.CompareTag("Hands"))
                {
                    StartPottery?.Invoke(cameraMovement.length);
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
}
