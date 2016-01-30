using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour 
{
	public GameObject gameManager;			//GameManager prefab to instantiate.
	public GameObject soundManager;			//SoundManager prefab to instantiate.

	public int MaxTurns;
	public GameObject overlay;
	public GameObject mainText;
	public GameObject canvas;
	
	
	void Awake ()
	{
		//Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
		if (GameManager.instance == null)

			//Instantiate gameManager prefab
			Instantiate(gameManager);

		GameManager.instance.MaxTurns = MaxTurns;
		GameManager.instance.canvas = canvas;
		GameManager.instance.overlay = overlay;
		GameManager.instance.mainText = mainText;
		
        //Check if a SoundManager has already been assigned to static variable GameManager.instance or if it's still null
        if (SoundManager.instance == null)
		//Instantiate SoundManager prefab
			Instantiate(soundManager);
	}
}
