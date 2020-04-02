using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{

    public delegate void FinishPotteryEventHandler();
    public static event FinishPotteryEventHandler FinishPottery;

    private void OnCollisionEnter(Collision collision) {
        if(transform.parent.gameObject.activeSelf == true) {
            FinishPottery?.Invoke();
        }
    }
}
