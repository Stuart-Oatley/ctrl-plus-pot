using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotRotator : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 20;

    private bool active = false;

    private void Start() {
        AnimationStateManager.MovingCam += SetActive;

    }

    private void SetActive(CamMoveEventArgs e) {
        if(e.MovingTo == Position.pots) {
            active = true;
            return;
        }
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(active) {
            Vector3 rotation = new Vector3(0, rotationSpeed, 0);
            transform.Rotate(Time.deltaTime * rotation);
        }
    }
}
