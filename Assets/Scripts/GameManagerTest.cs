using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    #region Variables
    //Game Panel variables to allow screens to be enabled and disabled
    [Header("User Inteface Panels")]
    [Tooltip("Add separate disabled Game Panel Object with HangmanHandlerTest script attached")]
    public GameObject gamePanel;
    //public GameObject menuPanel;
    //public GameObject pausePanel;
    //public GameObject postGamePanel;
    //Dictionary to store words
    [SerializeField] public static Dictionary<int, string> wordList = new Dictionary<int, string>();
    //Location of WordFile
    private string path = Path.Combine(Application.streamingAssetsPath, "WordFile.txt");
    #endregion
    void Start()
    {
        //Call function to load values into the dictionary at start
        BuildDictionary();
        //Set gamePanel object to active to allow the hangman game to run
        //We activate it here to stop the word from being selected before the dictionary is built
        gamePanel.SetActive(true);
    }
    #region Dictionary
    void BuildDictionary()
    {
        //Read words from the WordFile path
        StreamReader reader = new StreamReader(path);
        //Create variable to store the current line from the WordFile
        string line;
        //Create a loop to add values to dictionary while current line of WordFile is not empty
        while ((line = reader.ReadLine()) != null)
        {
            //Create string array to store word from file using the number before the : as an index
            string[] parts = line.Split(':');
            //Add the key and value from the array to the dictionary
            wordList.Add(int.Parse(parts[0]), parts[1]);
        }
        //Close the reader
        reader.Close();
        //TestDictionary();//A function to print values to console to verify dictionary is filled correctly
    }
    /*void TestDictionary()
    {
        //Start count at 0
        int indexCount = 0;
        //Create loop to print values until we reach the end of the dictionary
        while (wordList.Count > indexCount)
        {
            //Print the value
            Debug.Log(wordList[indexCount]);
            //Increment the count to allow us to stop at end of dictionary
            indexCount++;
        }
    }*/
    #endregion
    #region End Game
    public static void EndGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion
}