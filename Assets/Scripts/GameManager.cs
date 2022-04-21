using System.Collections.Generic; //To use Dictionary to store word values
using System.IO; //To use reader to read word values from txt file
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    //Dictionary to store words
    [SerializeField] public static Dictionary<int, string> wordList = new Dictionary<int, string>();
    //Location of WordFile
    private readonly string path = Path.Combine(Application.streamingAssetsPath, "WordFile.txt");
    #endregion
    void Start()
    {
        //Call function to load values into the dictionary at start
        BuildDictionary();
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
    #region EndGame
    public static void EndGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion
}