using System.Collections; //To use an Ienumerator to handle the failure post game animation
using UnityEngine.UI; //Add ability to control Canvas elements
using UnityEngine;

public class HangmanHandler : MonoBehaviour
{
    #region Variables
    //Variables to allow the display to be adjusted
    [Header("Game Objects")]
    [Tooltip("Add TextField from WordDisplay panel on GamePanel so we can modify it with each guess")]
    [SerializeField] private Text _wordDisplay;
    [Tooltip("Add each element of the hangman display that needs to be enabled for incorrect guesses in correct order")]
    [SerializeField] private GameObject[] _hangmanDisplay;
    [Tooltip("Add the GuessButtons container so we can manipulate the child buttons")]
    [SerializeField] private GameObject _buttonContainer;
    [Tooltip("Add ResultMessage text field from PostGamePanel so we can modify victory/failure message")]
    [SerializeField] private Text _resultText;
    //Game Panel variables to allow screens to be enabled and disabled. Add the panels that match the variable name.
    [Header("User Interface Canvases")]
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject postGamePanel;
    //Hangman Word Variables to control game setup
    private int _index = 0;
    private string _selectedWord;
    private int _wordLength;
    private string[] _displayWord;
    //Guess variables to determine when game is over
    private readonly int _maximumWrongGuesses = 8;
    private int _correctGuesses, _incorrectGuesses;
    //Animator variable to control post game animation
    private Animator _animator;
    #endregion
    private void Start()
    {
        //Retrieve the animator from GameObject this script is attached to
        _animator = GetComponent<Animator>();
    }
    #region Start Game
    //Function to set up new game each time we start from any screen
    public void SelectWord()
    {
        //Enable the GuessButton panel in case we are restarting a game from the post game screen
        _buttonContainer.SetActive(true);
        //Loop to set all child objects in the GuessButton panel to active se we can guess any letter at start of game
        for (int a = 0; a < _buttonContainer.transform.childCount; a++)
        {
            _buttonContainer.transform.GetChild(a).gameObject.SetActive(true);
        }
        //Loop to reset HangmanDisplay objects to disabled so only the base and stool are visible at start of game
        for (int i = 0; i < _hangmanDisplay.Length; i++)
        {
            _hangmanDisplay[i].SetActive(false);
        }
        //Enable the stool object as it was disabled in previous loop
        _hangmanDisplay[9].SetActive(true);
        //Reset guess counters to zero for new game
        _correctGuesses = 0;
        _incorrectGuesses = 0;
        //Create random number to pull a random word out of the dictionary
        _index = Random.Range(0, GameManager.wordList.Count);
        //Pull word out of dictionary based off random index created
        _selectedWord = GameManager.wordList[_index];
        //Set _wordLength to length of selected word to use as victory condition
        _wordLength = _selectedWord.Length;
        //Create new array for _displayWord with a size of our selected word so we can display enough characters on screen
        _displayWord = new string[_wordLength];
        //Change the value of each entry in the array so an underscore will be shown on screen for each letter in the selected word
        for (int i = 0; i < _wordLength; i++)
        {
            _displayWord[i] = " _ ";
        }
        //Run InitializeDisplay function to modify the screen according to our new inputs
        InitializeDisplay();
        //Debug.Log(_selectedWord);
        //Debug.Log(_wordLength);
    }
    //Function to adjust the onscreen elements at start of game and every time a guess is made
    void InitializeDisplay()
    {
        //Set the GameObject in the array that corresponds to the incorrect guess counter to active. Will enable new objects as counter increments. 
        _hangmanDisplay[_incorrectGuesses].SetActive(true);
        //Create string variable to store the entries of the _displayWord array as a single string that can be assigned to the text field 
        string str = string.Join(" ", _displayWord);
        //Adjust the on screen text field to show the str string
        _wordDisplay.text = str;
    }
    #endregion
    #region Handle Guesses
    //OnGUI function to enable us to single out alphabetical keyboard inputs
    private void OnGUI()
    {
        //If we press the escape key enable the PausePanel
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
        }
        //Store the current event in e
        Event e = Event.current;
        //If our e event is a keyboard press and is a single character button and is a letter
        if (e.type == EventType.KeyDown && e.keyCode.ToString().Length == 1 && char.IsLetter(e.keyCode.ToString()[0]))
        {
            //Loop through every child object in the GuessButton panel
            for (int a = 0; a < _buttonContainer.transform.childCount; a++)
            {
                //If value of keyboard button we pressed matches the value of the current button being checked
                if (_buttonContainer.transform.GetChild(a).GetComponentInChildren<Text>().text == e.keyCode.ToString())
                {
                    //If that button is still active
                    if (_buttonContainer.transform.GetChild(a).gameObject.activeInHierarchy)
                    {
                        //Register players guess in PlayerGuess function
                        PlayerGuess(e.keyCode.ToString());
                    }
                    //Disable the button so it can't be guessed again
                    _buttonContainer.transform.GetChild(a).gameObject.SetActive(false);
                }
            }            
        }        
    }
    //Function to check the players guess based off letter variable passed from the UI button or OnGUI function
    public void PlayerGuess(string letter)
    {
        //Create integer to use as index so we can select the right entry in the array if we have a correct guess
        int index = 0;
        //Boolean to be set to true only if we have found a match
        bool correctFound = false;
        //Check each character in the selected word for a match
        foreach (char c in _selectedWord)
        {
            //If our click matches the current character in the word
            if (char.Parse(letter) == c)
            {
                //Change the entry in the array at the current index to display correct match on screen
                _displayWord[index] = " " + letter + " ";
                //increment correct guesses
                _correctGuesses++;
                //Set boolean for correct guess to true
                correctFound = true;
                //Debug.Log("Correct Guesses: " + _correctGuesses);                
            }
            //Increment index so we access the next value in the array
            index++;
        }
        //If correct guess boolean is still false we haven't found a match
        if (correctFound == false)
        {
            //Increment incorrect guesses
            _incorrectGuesses++;
            //Debug.Log("Incorrect Guesses " + _incorrectGuesses);
        }
        //Run the InitializeDisplay function to update the new values onto the screen
        InitializeDisplay();
        //If number of incorrect guesses has reached the maximum allowed we have failed
        if (_incorrectGuesses >= _maximumWrongGuesses)
        {
            //Disable GuessButtons panel so we can't make any more guesses
            _buttonContainer.SetActive(false);
            //Start PostGameProcess coroutine to run the animation
            StartCoroutine(PostGameProcess());
        }
        //If number of correct guesses has reached the number of letters in the word we have won
        else if (_correctGuesses >= _wordLength)
        {
            //Change victory/failure message to reflect our success
            _resultText.text = "Congratulations on avoiding the noose";
            //Enable PostGamePanel and disable GamePanel
            postGamePanel.SetActive(true);
            gamePanel.SetActive(false);
        }
    }
    //PostGameProcess coroutine to handle animation for end of game
    private IEnumerator PostGameProcess()
    {
        //Set the animation trigger to start the animation
        _animator.SetTrigger("HangTrigger");
        //Pause the coroutine for length required to view the desired section of animation
        yield return new WaitForSecondsRealtime(2.2f);
        //Change victory/failure message to reflect our failure
        _resultText.text = "You failed    Please try again";
        //Enable PostGamePanel
        postGamePanel.SetActive(true);
        //Pause the coroutine for enough to let the rest of animation we didnt want shown to play through.
        //This allows our man to return to his correct position for start of new game
        yield return new WaitForSecondsRealtime(0.2f);
        //Disable GamePanel
        gamePanel.SetActive(false);
    }
    #endregion
}
