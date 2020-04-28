using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// event args sent when a pot is being saved
/// </summary>
public class SavePotEventArgs : EventArgs {
    private GameObject pot;
    public GameObject Pot {
        get { return pot; }
    }

    public SavePotEventArgs(GameObject potToSave) {
        pot = potToSave;
    }
}

/// <summary>
/// Manages saving and loading of the pots
/// </summary>
public class PotSaveManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Pots;

    private int numberSaved = 0;
    private string nameOfNumberSavedPref = "SavedPots";
    private string nameOfPotSaveFiles = "PotSave";

    public delegate void SaveFinishedEventHandler();
    public static event SaveFinishedEventHandler Saved;

    private void Start() {
        SaveButton.SavePot += SavePot;
        if (!PlayerPrefs.HasKey(nameOfNumberSavedPref)) {
            PlayerPrefs.SetInt(nameOfNumberSavedPref, 0);
        }
        numberSaved = PlayerPrefs.GetInt(nameOfNumberSavedPref);
        if(numberSaved == 0 || Pots.Length == 0) {
            return;
        }
        LoadAll();
    }

    /// <summary>
    /// Loads all pots
    /// </summary>
    private void LoadAll() {
        int numberToLoad = (numberSaved > Pots.Length) ? Pots.Length : numberSaved;
        for (int i = 0; i < numberToLoad; i++) {
            MeshSaver.LoadMesh(Pots[i], nameOfPotSaveFiles + i.ToString());
        }
    }

    /// <summary>
    /// saves the current pot
    /// </summary>
    /// <param name="savePot"></param>
    private void SavePot(SavePotEventArgs savePot) {
        int fileNumber = numberSaved % Pots.Length;
        numberSaved++;
        PlayerPrefs.SetInt(nameOfNumberSavedPref, numberSaved);
        MeshSaver.SaveMesh(savePot.Pot, nameOfPotSaveFiles + fileNumber.ToString());
        LoadPot(fileNumber);
    }

    /// <summary>
    /// loads a specific pot
    /// </summary>
    /// <param name="filenumber"></param>
    private void LoadPot(int filenumber) {
        StartCoroutine(WaitForFileAccessThenLoad(filenumber));
    }

    /// <summary>
    /// Waits for pot to finish saving, then loads it
    /// </summary>
    /// <param name="filenumber"></param>
    /// <returns></returns>
    private IEnumerator WaitForFileAccessThenLoad(int filenumber) {
        string filename = nameOfPotSaveFiles + filenumber.ToString();
        while (MeshSaver.IsFileLocked(filename)) {
            yield return null;
        }
        Saved?.Invoke();
        MeshSaver.LoadMesh(Pots[filenumber], filename);

    }
}
