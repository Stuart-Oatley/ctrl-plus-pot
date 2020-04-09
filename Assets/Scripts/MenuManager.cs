using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startMenuHolder;

    [SerializeField]
    private GameObject potteryMenuHolder;

    [SerializeField]
    private GameObject paintMenuHolder;

    [SerializeField]
    private GameObject potsMenuHolder;

    private GameObject currentMenu;

    void Start()
    {
        AnimationStateManager.MovingCam += ChangeActiveMenu;
        currentMenu = startMenuHolder;
    }

    private void ChangeActiveMenu(CamMoveEventArgs e) {
        currentMenu.SetActive(false);
        switch (e.MovingTo) {
            case Position.menu:
                currentMenu = startMenuHolder;
                break;
            case Position.pottery:
                currentMenu = potteryMenuHolder;
                break;
            case Position.painting:
                currentMenu = paintMenuHolder;
                break;
            case Position.pots:
                currentMenu = potsMenuHolder;
                break;
            default:
                currentMenu = startMenuHolder;
                break;
        }
        StartCoroutine(EnableMenu(e.AnimationLength));
    }

    private IEnumerator EnableMenu(float animationLength) {
        yield return new WaitForSeconds(animationLength);
        currentMenu.SetActive(true);
    }
}
