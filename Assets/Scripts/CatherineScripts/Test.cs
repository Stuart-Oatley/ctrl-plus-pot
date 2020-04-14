using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	private float speed = 50.0f;
	private Vector3 scaleChange = new Vector3(-0.01f, 0f, -0.01f);

	void Update() 
	{
		transform.Rotate(Vector3.up * speed * Time.deltaTime);
	}

	public void changeShape() 
	{
		if (gameObject.transform.localScale.x <= 0.02f ||
			gameObject.transform.localScale.y <= 0.02f) 
		{
			gameObject.transform.localScale = new Vector3(0.02f, 0.06f, 0.02f);
		}
		else 
		{
			gameObject.transform.localScale += scaleChange;
		}
	}
}
