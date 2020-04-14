using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour
{
    private Image image;
	public void startGame() 
	{
		Debug.Log("play");
		SceneManager.LoadScene("Main");
	}

	public void quitGame() 
	{
		Debug.Log("Quitting");
		Application.Quit();
	}

	public void mainMenu() 
	{
		Debug.Log("MainMenu");
		SceneManager.LoadScene("Menu");
	}

    public void highlight()
    {
        Debug.Log("Hover");
        image = gameObject.GetComponent<Image>();
        image.enabled = false;
    }
}
