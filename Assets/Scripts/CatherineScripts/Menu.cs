using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public delegate void StartPotteryEventHandler(float delayTime);
    public static event StartPotteryEventHandler StartPottery;

    public delegate void ShowingPotsEventHandler(float delayTime);
    public static event ShowingPotsEventHandler ShowPots;

    private void OnCollisionEnter(Collision collision)
    {
        if (!transform.parent.gameObject.activeSelf) {
            return;
        }
        switch(gameObject.name)
        {
            case "StartButton":
                if (collision.collider.CompareTag("Index"))
                {
                    AnimationStateManager.MoveCamera(Position.pottery);
                }
                break;
            case "PotsButton":
                if (collision.collider.CompareTag("Index"))
                {
                    AnimationStateManager.MoveCamera(Position.pots);
                }
                break;
            case "QuitButton":
                if (collision.collider.CompareTag("Index"))
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
