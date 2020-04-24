using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintColour : MonoBehaviour
{
    [SerializeField] private GameObject paintBrush; // Stores the object with the paint brush

    // paint colours
    private Color32 black = new Color32(35, 28, 21, 255);
    private Color32 blue = new Color32(19, 97, 98, 255);
    private Color32 green = new Color32(106, 107, 16, 255);
    private Color32 red = new Color32(73, 12, 11, 255);
    private Color32 pink = new Color32(147, 53, 88, 255);
    private Color32 orange = new Color32(166, 75, 9, 255);

    Painter painterScript;

    private void Start()
    {
        // finds the painter script in the paint brush object
        painterScript = paintBrush.GetComponentInChildren<Painter>();

        //	switch (gameObject.name)
        //	{
        //		case "Black":
        //			gameObject.GetComponent<Renderer>().material.color = Color.black;
        //			break;
        //		case "Blue":
        //			gameObject.GetComponent<Renderer>().material.color = Color.blue;
        //			break;
        //		case "Cyan":
        //			gameObject.GetComponent<Renderer>().material.color = Color.cyan;
        //			break;
        //		case "Green":
        //			gameObject.GetComponent<Renderer>().material.color = Color.green;
        //			break;
        //		case "Magenta":
        //			gameObject.GetComponent<Renderer>().material.color = Color.magenta;
        //			break;
        //		case "Red":
        //			gameObject.GetComponent<Renderer>().material.color = Color.red;
        //			break;
        //		case "Yellow":
        //			gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        //			break;
        //		case "White":
        //			gameObject.GetComponent<Renderer>().material.color = Color.white;
        //			break;
        //	}
    }

    private void OnCollisionEnter(Collision collision)
    {
        // checks collision is with the leap hands
        if (collision.collider.CompareTag("Hands"))
        {
            // checks which colour and assigns the paintbrush that colour
            switch (gameObject.name)
            {
                case "Black":
                    Debug.Log("Black");
                    painterScript.paintColour = black;
                    break;
                case "Blue":
                    Debug.Log("Blue");
                    painterScript.paintColour = blue;
                    break;
                case "Green":
                    Debug.Log("Green");
                    painterScript.paintColour = green;
                    break;
                case "Red":
                    Debug.Log("Red");
                    painterScript.paintColour = red;
                    break;
                case "Pink":
                    Debug.Log("Pink");
                    painterScript.paintColour = pink;
                    break;
                case "Orange":
                    Debug.Log("Orange");
                    painterScript.paintColour = orange;
                    break;
                    //case "Cyan":
                    //	painterScript.paintColour = Color.cyan;
                    //	break;
                    //case "Magenta":
                    //	painterScript.paintColour = Color.magenta;
                    //	break;
                    //case "Yellow":
                    //	painterScript.paintColour = Color.yellow;
                    //	break;
                    //case "White":
                    //	painterScript.paintColour = Color.white;
                    //	break;
            }
        }
    }
}
