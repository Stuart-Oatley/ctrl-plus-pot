using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public int maxtime = 20;
    public float time = 0;

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
        WorldValues.maxtime = maxtime;
    }

    private void Update()
    {
        time += Time.deltaTime * WorldValues.faketimescale;
        text.text = Mathf.Round(maxtime - time).ToString();

        if (maxtime - time <= 0)
        {
            text.text = "Amazing!";
        }
    }
}
