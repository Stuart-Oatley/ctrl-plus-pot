using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the paint pots to and from their painting positions
/// </summary>
public class PaintMovement : MonoBehaviour
{
    private Vector3 originalPos;
    private Quaternion originalRot;

    [SerializeField]
    Transform paintPosAndRot;

    private void Start() {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
        AnimationStateManager.MovingCam += Move;
    }

    private void Move(CamMoveEventArgs e) {
        if(e.MovingTo == Position.painting) {
            transform.localPosition = paintPosAndRot.localPosition;
            transform.localRotation = paintPosAndRot.localRotation;
            return;
        }
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
    }
}
