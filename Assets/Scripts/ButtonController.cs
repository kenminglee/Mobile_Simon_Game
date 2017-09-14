using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

	private SpriteRenderer theSprite;
	public int thisButtonNumber;

	private GameManager theGM;  

	private AudioSource theSound;


	// Use this for initialization
	void Start () {
		//get SpriteRenderer component of the object that this script is attached to.
		theSprite = GetComponent<SpriteRenderer>();
		theGM = FindObjectOfType<GameManager>();
		theSound = GetComponent<AudioSource> ();
	}
	
	//Called on mouse click
	void OnMouseDown(){
		if (theGM.GetGameState()) {
			theSprite.color = new Color (theSprite.color.r, theSprite.color.g, theSprite.color.b, 1f);
			theSound.Play ();
		}
	}

	//Called on mouse release
	void OnMouseUp(){
		if (theGM.GetGameState()) {
			theSprite.color = new Color (theSprite.color.r, theSprite.color.g, theSprite.color.b, 0.5f);
			theGM.ColourPressed (thisButtonNumber); 
			StartCoroutine (PlayAfterDelay (theGM.soundDelay));

		}
	}

	IEnumerator PlayAfterDelay(float time){
		yield return new WaitForSeconds (time);
		theSound.Stop ();
	}	
}
