using System.Collections.Generic;
using UnityEngine;

public class HangmanHandlerTest : MonoBehaviour
{
    #region Variables
    //Variables to allow the display to be adjusted
    [Header("Display Variables")]
    [Tooltip("Add HangmanVisual object to allow sprite to be changed")]
    [SerializeField] private SpriteRenderer _hangmanDisplay;
    [Tooltip("Add all hangman image variations in order")]
    [SerializeField] private List<Sprite> _hangmanImages;
    [Tooltip("Add GUI Skin asset to adjust the style of the IMGUI elements on screen")]
    [SerializeField] private GUISkin _hangmanSkin;
    //Hangman Word Variables
    private int _index = 0;
    private string _selectedWord;
    private int _wordLength;
    //Array to contain the values to be displayed on screen before and after correct guesses
    private string[] _displayWord;
    //Guess variables
    private readonly int _maximumWrongGuesses = 8;
    private int _correctGuesses, _incorrectGuesses;
    #endregion
    void Start()
    {
        //Run select word function at starup to set up first game
        SelectWord();        
    }
    #region Initialize New Game
    void SelectWord()
    {
        //Reset guess counters in case we are starting new game
        _correctGuesses = 0;
        _incorrectGuesses = 0;
        //Reset hangman display for new game
        _hangmanDisplay.sprite = _hangmanImages[_incorrectGuesses];
        //Create random number to access a random word from the dictionary
        _index = Random.Range(0, GameManagerTest.wordList.Count);
        //Select and store a word from dictionary based off of random _index integer
        _selectedWord = GameManagerTest.wordList[_index];
        //Store the length of the selected word to be used as a victory condition
        _wordLength = _selectedWord.Length;
        //Create new array for _displayWord based off the length of the word so we can change the on screen display for the current word 
        _displayWord = new string[_wordLength];
        //Store underscores in the array to be displayed on screen by default as the word is hidden
        for (int i = 0; i < _wordLength; i++)
        {
            _displayWord[i] = " _ ";
        }
        //Debug.Log(_selectedWord);
        //Debug.Log(_wordLength);
    }
    #endregion
    #region User Interface
    //Create function to handle user guess from passed click variable which is taken from the letter for the button clicked
    private void OnGUIClick(char click)
    {
        //Debug.Log(click);
        //Create integer to use as index so we can select the right entry in the array if we have a correct guess
        int index = 0;
        //Boolean to be set to true only if we have found a match
        bool correctFound = false;
        //Check each character in the selected word for a match
        foreach (char c in _selectedWord)
        {
            //If our click matches the current character in the word
            if (click == c)
            {
                //Change the entry in the array at the current index to display correct match on screen
                _displayWord[index] = click.ToString();
                //increment correct guesses
                _correctGuesses++;
                //Set boolean for correct guess to true
                correctFound = true;
                //Debug.Log(_correctGuesses);                
            }
            //Increment index so we access the next value in the array
            index++;
        }
        //If correct guess boolean is still false we haven't found a match
        if (correctFound == false)
        {
            //Increment incorrect guesses
            _incorrectGuesses++;
            //Change sprite on hangman visual display to show user they have not found a match
            _hangmanDisplay.sprite = _hangmanImages[_incorrectGuesses];
            //Debug.Log(_incorrectGuesses);
        }
    }
    private void OnGUI()
    {
        //Set GUI Skin to the asset we declared in variables so GUI will be styled the way we want
        GUI.skin = _hangmanSkin;
        //Define a character variable starting at a to be incremented through for each guess button we create on screen
        char letter = 'A';
        //Create a Vector2 variable so we can adjust position and size of elements
        Vector2 scr;
        //Divide screen height by 9 so height of text boxes and letter buttons is 1/9 of the screen height
        scr.y = Screen.height / 9;
        //Create a text field on the screen for each character in the selected word based off of _wordLength
        for (int i = 0; i < _wordLength; i++)
        {
            //Divide the width of screen by the length of the word so the text elements will fill the screen width
            scr.x = Screen.width / _wordLength;            
            //Create Text field at the set size and position horizontally based off of the i index and vertically about halfway 
            GUI.TextField(new Rect(scr.x * i, scr.y * 4.5f, scr.x, scr.y), _displayWord[i]);
        }
        //Divide the screen length by the amount of letter boxes we'll store horizonatally so they'll cover the width of screen
        scr.x = Screen.width / 13;
        //If we still have guesses left and we haven't completed the word
        if (_incorrectGuesses < _maximumWrongGuesses && _correctGuesses < _wordLength)
        {
            //Create a button for each letter of the alphabet in 2 rows of 13
            for (int y = 7; y < 9; y++)
            {
                for (int x = 0; x < 13; x++)
                {
                    //Position the button based off of the x and y indexes and size by scr values and label it with the letter from the variable
                    if (GUI.Button(new Rect(scr.x * x, scr.y * y, scr.x, scr.y), letter.ToString()))
                    {
                        //Bool to check if we have already correctly guessed the letter
                        bool alreadyGuessed = false;
                        //Check each entry in the displayed word to see if we find a match for the button we clicked and set bool to true if we do
                        for (int i = 0; i < _displayWord.Length; i++)
                        {
                            foreach (char c in _displayWord[i])
                            {
                                if (c == letter)
                                {
                                    alreadyGuessed = true;
                                }
                            }
                        }
                        //If button is clicked and bool check is false, activate the OnGUIClick function and pass it the letter of the button pushed
                        if (alreadyGuessed == false)
                        {
                            OnGUIClick(letter);
                        }                                                
                    }
                    //Increment the letter so the next button has a different letter
                    letter++;
                }
            }
        }
        //If we have reached the limit of incorrect guesses
        else if (_incorrectGuesses >= _maximumWrongGuesses)
        {
            //Display a failure message instead of the buttons
            GUI.TextField(new Rect(scr.x * 0, scr.y * 7, scr.x * 13, scr.y * 2), "You Failed");
        }
        //Else we must have completed the word, display victory message instead of buttons
        else
        {
            GUI.TextField(new Rect(scr.x * 0, scr.y * 7, scr.x * 13, scr.y * 2), "Congratulations");
        }
        //Create a New button to start a new game by rerunning the SelectWord function
        if (GUI.Button(new Rect(scr.x, scr.y, scr.x, scr.y), "New"))
        {
            SelectWord();
        }
        //Create an exit button to quit the game using the EndGame function from the GameManager script
        if (GUI.Button(new Rect(scr.x * 11, scr.y, scr.x, scr.y), "Exit"))
        {
            GameManagerTest.EndGame();
        }
    }
    #endregion
}
