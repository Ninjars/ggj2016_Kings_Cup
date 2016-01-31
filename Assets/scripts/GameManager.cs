using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
	public float turnDelay = 0.1f;							//Delay between each Player turn.
	public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
	public List<Enemy> enemies;								//List of all Enemy units, used to issue them move commands.
	[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
	public int MaxTurns = 10;

	public string startLevelMessage;
	public string levelTitle;
	public GameObject overlay;							//Image to block out level as levels are being set up, background for levelText.
	public GameObject mainText;
	public GameObject canvas;
	public GameObject button;
	private GameObject overlayInstance;
	private GameObject mainTextInstance;
	private GameObject buttonInstance;
	private GameObject canvasInstance;
	private bool enemiesMoving;								//Boolean to check if enemies are
	private Text stepsText;
	private Text shadowStepText;

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
		
		//Assign enemies to a new List of Enemy objects.
		enemies = new List<Enemy>();
		canvasInstance = Instantiate (canvas) as GameObject;
		stepsText = (GameObject.Find ("StepText") as GameObject).GetComponentInChildren<Text> ();
		GameObject shadowTextObj = GameObject.Find ("ShadowStepText") as GameObject;
		if (shadowTextObj != null) {
			shadowStepText = shadowTextObj.GetComponentInChildren<Text> ();
		}
	}

	void Start() {
		ShowStartScreen ();
	}

	public void updateStepsText(int steps) {
		stepsText.text = "Steps: " + steps;
	}

	public void updateShadowStepsText(int stepsInShadow) {
		if (shadowStepText != null) {
			shadowStepText.text = "Steps in shadow: " + stepsInShadow;
		}
	}
	
	//Hides black image used between levels
	void HideLevelImage() {
		//Disable the levelImage gameObject.
		if (overlayInstance != null) Transform.Destroy(overlayInstance);
		if (mainTextInstance != null) Transform.Destroy(mainTextInstance);
		if (buttonInstance != null) Transform.Destroy(buttonInstance);
	}

	private void ShowStartScreen() {
		Debug.Log ("ShowStartScreen()");
		instantiateUI ();
		playersTurn = false;
		mainTextInstance.GetComponent<Text> ().text = levelTitle + "\n" + startLevelMessage;
		buttonInstance.GetComponentInChildren<Text> ().text = "ENTER THE HALL";
		buttonInstance.GetComponentInChildren<Button>().onClick.AddListener (() => onStartLevelButtonPressed());
	}

	private void onStartLevelButtonPressed() {
		HideLevelImage ();
		playersTurn = true;
	}

	void ShowGameOverMessage(string message) {
		Debug.Log ("ShowGameOverMessage()");
		instantiateUI ();
		playersTurn = false;
		mainTextInstance.GetComponent<Text> ().text = message;
		buttonInstance.GetComponentInChildren<Text> ().text = "GET MORE WINE";
		buttonInstance.GetComponentInChildren<Button>().onClick.AddListener (() => RestartLevel());
	}

	public void ShowNextLevelMessage() {
		Debug.Log ("ShowNextLevelMessage()");
		instantiateUI ();
		playersTurn = false;
		mainTextInstance.GetComponent<Text> ().text = getNextLevelString();
		buttonInstance.GetComponentInChildren<Text> ().text = "GET MORE WINE";
		buttonInstance.GetComponentInChildren<Button> ().onClick.AddListener (() => GoToNextLevel());
	}

	private string getNextLevelString() {
		string baseString = "EXCELLENT!\n";
		string addition = "";
		switch (Random.Range (0, 5)) {
		case 0:
			addition = "How about a nice bottle of red?";
			break;
		case 1:
			addition = "Any more house white?";
			break;
		case 2:
			addition = "I'm still rather parched - another!";
			break;
		case 3:
			addition = "Mmmm, wine...";
			break;
		default:
			addition = "*Gurgle, slurp*";
			break;
		}
		return baseString + addition;
	}

	private void instantiateUI() {
		Debug.Log ("instantiateUI()");
		overlayInstance = Instantiate (overlay) as GameObject;
		mainTextInstance = Instantiate (mainText) as GameObject;
		buttonInstance = Instantiate (button) as GameObject;
		overlayInstance.transform.SetParent(canvasInstance.transform);
		mainTextInstance.transform.SetParent(canvasInstance.transform);
		buttonInstance.transform.SetParent(canvasInstance.transform, false);
	}

	private void RestartLevel() {
		Debug.Log ("RestartLevel()");
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		HideLevelImage ();
	}

	private void GoToNextLevel() {
		Debug.Log ("GoToNextLevel()");
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
		HideLevelImage ();
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
	public void GameOver(GameOverReason reason) {
		string failMessage = "YOU WERE REMOVED FROM COURT\n";
		switch (reason) {
			case GameOverReason.WINE:
			failMessage += "You ran out of wine before reaching the King.";
			break;
			case GameOverReason.TIME:
			failMessage += "You ran out of time before reaching the King.";
			break;
			case GameOverReason.SHADOWS:
			failMessage += "You spent too long skulking in the shadows.";
			break;
			case GameOverReason.ATTEMPTED_REGICIDE:
			failMessage += "TOO CLOSE.";
			break;
		}
		ShowGameOverMessage(failMessage);

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
