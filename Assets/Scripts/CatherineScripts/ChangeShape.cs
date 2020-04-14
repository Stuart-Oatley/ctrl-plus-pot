using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShape : MonoBehaviour
{
    private float speed = 50.0f;

    private int clickNum = 0;

    private bool isClicked1;
    private bool isClicked2;
    private bool isClicked3;
    private bool isClicked4;

    private void Start()
    {
        clickNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }

    void OnMouseDown()
    {
        switch (clickNum)
        {
            case 0:
                Debug.Log("click");
                isClicked1 = true;
                gameObject.GetComponent<Animator>().SetBool("isClicked1", isClicked1);
                clickNum = 1;
                break;

            case 1:
                isClicked2 = true;
                gameObject.GetComponent<Animator>().SetBool("isClicked2", isClicked2);
                clickNum = 2;
                break;

            case 2:
                isClicked3 = true;
                gameObject.GetComponent<Animator>().SetBool("isClicked3", isClicked3);
                clickNum = 3;
                break;

            case 3:
                isClicked4 = true;
                gameObject.GetComponent<Animator>().SetBool("isClicked4", isClicked4);
                clickNum = 4;
                break;

            default:
                break;
        }
        

        
    }
}
