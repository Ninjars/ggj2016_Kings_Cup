using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.

public class GameManager : MonoBehaviour {
	public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
	public float turnDelay = 1f;							//Delay between each Player turn.
	public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
	public List<Enemy> enemies;								//List of all Enemy units, used to issue them move commands.
	[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
	public int MaxTurns = 10;

	//private Text levelText;									//Text to display current level number.
	public GameObject overlay;							//Image to block out level as levels are being set up, background for levelText.
	public GameObject mainText;
	public GameObject canvas;
	private GameObject overlayInstance;
	private GameObject mainTextInstance;
	private bool enemiesMoving;								//Boolean to check if enemies are 

	public enum GameOverReason {WINE, TIME, SHADOWS, ATTEMPTED_REGICIDE};

	//Awake is always called before any Start functions
	void Awake() {
		//Check if instance already exists
		if (instance == null) {
			Debug.Log ("instantiating GameManager");	
			//if not, set instance to this
			instance = this;
		} else if (instance != this) {
			//If instance already exists and it's not this:
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy (gameObject);
		}

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
		
		//Assign enemies to a new List of Enemy objects.
		enemies = new List<Enemy>();
	}
	
	//Hides black image used between levels
	void HideLevelImage() {
		//Disable the levelImage gameObject.
		if (overlayInstance != null) Transform.Destroy(overlayInstance);
		if (mainText != null) Transform.Destroy(mainText);
	}

	void ShowLevelImage(string message) {
		Debug.Log ("ShowLevelImage()");
		overlayInstance = Instantiate (overlay) as GameObject;
		mainTextInstance = Instantiate (mainText) as GameObject;
		mainTextInstance.GetComponent<Text> ().text = message;
		overlayInstance.transform.parent = canvas.transform;
		mainTextInstance.transform.parent = canvas.transform;
	}
	
	//Update is called every frame.
	void Update() {
//		Debug.Log ("players turn? " + this.playersTurn + " enemy turn? " + this.enemiesMoving);
		//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if (playersTurn || enemiesMoving) {
			//If any of these are true, return and do not start MoveEnemies.
			return;
		}
		
		//Start moving enemies.
//		Debug.Log ("start moving enemies");
		StartCoroutine (MoveEnemies ());
	}
	
	//GameOver is called when the player reaches 0 food points
		//Set levelText to display number of levels passed and game over message
		//Enable black background image gameObject.
		ShowLevelImage("You were removed from court");
	public void GameOver(GameOverReason reason) {

		//Disable this GameManager.
		enabled = false;
	}

	public void setPlayersTurn(bool playersTurn){
		this.playersTurn = playersTurn;
//		Debug.Log ("players turn set to " + this.playersTurn);
	}

	public void addEnemyToList(Enemy enemy){
		enemies.Add (enemy);
	}
	
	//Coroutine to move enemies in sequence.
	IEnumerator MoveEnemies() {
//		Debug.Log ("MoveEnemies");
		//While enemiesMoving is true player is unable to move.
		enemiesMoving = true;
		
		//Wait for turnDelay seconds, defaults to .1 (100 ms).
		yield return new WaitForSeconds(turnDelay);
		
		//If there are no enemies spawned (IE in first level):
		if (enemies.Count == 0) 
		{
//			Debug.Log ("no enemies");
			//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
			yield return new WaitForSeconds(turnDelay);
		}

		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
		{
//			Debug.Log ("Moving an enemy");
			//Call the MoveEnemy function of Enemy at index i in the enemies List.
			enemies[i].MoveEnemy ();
			
			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

//		Debug.Log ("done moving enemies");
		//Once Enemies are done moving, set playersTurn to true so player can move.
		setPlayersTurn(true);
		
		//Enemies are done moving, set enemiesMoving to false.
		enemiesMoving = false;
	}
}
