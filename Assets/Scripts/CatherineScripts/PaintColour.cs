using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintColour : MonoBehaviour
{
	private void Start()
	{
		switch (gameObject.name)
		{
			case "Black":
				gameObject.GetComponent<Renderer>().material.color = Color.black;
				break;
			case "Blue":
				gameObject.GetComponent<Renderer>().material.color = Color.blue;
				break;
			case "Cyan":
				gameObject.GetComponent<Renderer>().material.color = Color.cyan;
				break;
			case "Green":
				gameObject.GetComponent<Renderer>().material.color = Color.green;
				break;
			case "Magenta":
				gameObject.GetComponent<Renderer>().material.color = Color.magenta;
				break;
			case "Red":
				gameObject.GetComponent<Renderer>().material.color = Color.red;
				break;
			case "Yellow":
				gameObject.GetComponent<Renderer>().material.color = Color.yellow;
				break;
			case "White":
				gameObject.GetComponent<Renderer>().material.color = Color.white;
				break;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		GameObject paintBrush = GameObject.Find("collision pot");
		Painter painterScript = paintBrush.GetComponent<Painter>();

		switch(gameObject.name)
		{
			case "Black":
				painterScript.paintColour = Color.black;
				break;
			case "Blue":
				painterScript.paintColour = Color.blue;
				break;
			case "Cyan":
				painterScript.paintColour = Color.cyan;
				break;
			case "Green":
				painterScript.paintColour = Color.green;
				break;
			case "Magenta":
				painterScript.paintColour = Color.magenta;
				break;
			case "Red":
				painterScript.paintColour = Color.red;
				break;
			case "Yellow":
				painterScript.paintColour = Color.yellow;
				break;
			case "White":
				painterScript.paintColour = Color.white;
				break;
		}
	}
}
