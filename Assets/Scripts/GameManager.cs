using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	//make references to SpriteRenderers- no need to make references to the gameobjects itself, just their SpriteRenderer components will do
	public SpriteRenderer[] colours;
	public AudioSource[] buttonSounds;

	//used to select random buttons
	private int colourSelect;

	//The amount of time that we want the buttons to light up b4 fading away
	public float stayLit;
	//Kinda like using delay(time) in arduino, only difference is that this is a manual counter for the number of loops (# of frames in this case)
	private float stayLitCounter;

	//wait between each sequence of light
	public float waitBetweenLights;
	private float waitBetweenLightsCounter;

	public float timeBetweenSequence;
	//Delay between the button sound and the correct sound
	public float correctSoundDelay;
	//so that even if u tap the button for 1 ms, the sound will still play for at least __ seconds
	public float soundDelay;

	//Used to combat the problem where once the light has been turned off, we cannot turn it back on
	private bool shouldBeLit;
	private bool shouldBeDark;

	public List<int> activeSequence;
	//Position in the sequence
	private int positionInSequence;
	//Is it time for the player to interact with game?
	private bool gameActive;
	private int inputInSequence;

	public AudioSource correct;
	public AudioSource incorrect;

	public Text scoreText;

	// Use this for initialization
	void Start () {

		if (!PlayerPrefs.HasKey ("HiScore")) {
			PlayerPrefs.SetInt ("HiScore",0);
		}	
		//Playerprefs is used to store game data permanently - learn how to read and write files for Unity!
		scoreText.text = "Score: 0 - High Score: " + PlayerPrefs.GetInt ("HiScore");
	
		
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldBeLit) {
			//stayLitCounter var decreases (by minusing the amount of time taken to complete last frame)
			stayLitCounter -= Time.deltaTime;
			if (stayLitCounter < 0) {
				colours[activeSequence[positionInSequence]].color = new Color(colours[activeSequence[positionInSequence]].color.r,colours[activeSequence[positionInSequence]].color.g,colours[activeSequence[positionInSequence]].color.b,0.5f);
				buttonSounds [activeSequence [positionInSequence]].Stop ();
				//so that we cannot dim the button again and again every frame
				shouldBeLit = false;
				shouldBeDark = true;
				if (activeSequence.Count % 5 == 0 && waitBetweenLights>0.1)
					waitBetweenLights = waitBetweenLights * (float) 0.9;
				waitBetweenLightsCounter = waitBetweenLights;
				positionInSequence++;
			}
		}

		if (shouldBeDark) {
			waitBetweenLightsCounter -= Time.deltaTime;
			//if positionInSequence is more than the number of elements in activeSequence
			if (positionInSequence >= activeSequence.Count) {
				shouldBeDark = false;
				gameActive = true;
			} else {
				if (waitBetweenLightsCounter < 0) {
					//we are only changing the A values, while using back the same rgb values, that is y we use back the same colours (notice how the last parameter is 1f, which means max value for A)
					colours[activeSequence[positionInSequence]].color = new Color(colours[activeSequence[positionInSequence]].color.r,colours[activeSequence[positionInSequence]].color.g,colours[activeSequence[positionInSequence]].color.b,1f);
					buttonSounds [activeSequence [positionInSequence]].Play ();
					if (activeSequence.Count % 5 == 0 && stayLit > 0.7)
						stayLit = stayLit * (float) 0.9;
					stayLitCounter = stayLit;
					shouldBeLit = true;
					shouldBeDark = false;
				}
			}	
		}	
	}

	//Must exclusively state public in order for method to be accessed externally
	public void StartGame(){
		stayLit = 1;
		waitBetweenLights = (float)0.25;

		activeSequence.Clear ();

		positionInSequence = 0;
		inputInSequence = 0;
		colourSelect = Random.Range (0, colours.Length); //Random.Range will actually range from the parameters (min,max-1) therefore we do not nid to -1 ourselves
		activeSequence.Add(colourSelect);

		//we are only changing the A values, while using back the same rgb values, that is y we use back the same colours (notice how the last parameter is 1f, which means max value for A)
		colours[activeSequence[positionInSequence]].color = new Color(colours[activeSequence[positionInSequence]].color.r,colours[activeSequence[positionInSequence]].color.g,colours[activeSequence[positionInSequence]].color.b,1f);
		buttonSounds [activeSequence [positionInSequence]].Play ();

		stayLitCounter = stayLit;

		shouldBeLit = true;
		gameActive = false;

		scoreText.text = "Score: 0 - High Score: " + PlayerPrefs.GetInt ("HiScore");
	}

	public void ColourPressed(int whichButton){
		if (gameActive) {

			//if the randomly chosen button is the same as the one the user selects
			if (activeSequence[inputInSequence] == whichButton) {
				Debug.Log ("Correct");
				inputInSequence++;
				waitBetweenLightsCounter = waitBetweenLights;

					
				if (inputInSequence >= activeSequence.Count) {
					if (activeSequence.Count > PlayerPrefs.GetInt ("HiScore"))
						PlayerPrefs.SetInt ("HiScore", activeSequence.Count);
					scoreText.text = "Score: " + activeSequence.Count + " - High Score: " + PlayerPrefs.GetInt ("HiScore");
					StartCoroutine (PlayCorrectSoundAfterDelay (correctSoundDelay));
					StartCoroutine (StartNewSequenceAfterTime (timeBetweenSequence));
				}
				
			} else {
				Debug.Log ("Wrong");
				incorrect.Play ();
				gameActive = false;
			}
		}
			
	}

	public bool GetGameState(){
		return gameActive;
	}	

	IEnumerator StartNewSequenceAfterTime(float time){
		yield return new WaitForSeconds(time);


		positionInSequence = 0;
		inputInSequence = 0;
		colourSelect = Random.Range (0, colours.Length); //Random.Range will actually range from the parameters (min,max-1) therefore we do not nid to -1 ourselves
		activeSequence.Add (colourSelect);

		//we are only changing the A values, while using back the same rgb values, that is y we use back the same colours (notice how the last parameter is 1f, which means max value for A)
		colours [activeSequence [positionInSequence]].color = new Color (colours [activeSequence [positionInSequence]].color.r, colours [activeSequence [positionInSequence]].color.g, colours [activeSequence [positionInSequence]].color.b, 1f);
		buttonSounds [activeSequence [positionInSequence]].Play ();
		stayLitCounter = stayLit;

		shouldBeLit = true;
		gameActive = false;
	}

	IEnumerator PlayCorrectSoundAfterDelay(float time){
		yield return new WaitForSeconds (time);
		correct.Play ();
	}	
}
