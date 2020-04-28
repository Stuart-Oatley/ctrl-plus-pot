using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to enable aand disable different menus when the camera is moveed
/// </summary>
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

    /// <summary>
    /// disables the current menu then switches current menu to the appropriate menu
    /// </summary>
    /// <param name="e"></param>
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

    /// <summary>
    /// enables the new menu after the animation has finished
    /// </summary>
    /// <param name="animationLength"></param>
    /// <returns></returns>
    private IEnumerator EnableMenu(float animationLength) {
        yield return new WaitForSeconds(animationLength);
        currentMenu.SetActive(true);
    }
}
