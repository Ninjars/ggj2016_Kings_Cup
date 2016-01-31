using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {
	private int startX;
	private int startY;
	public int endX;
	public int endY;
	public LayerMask enemyLayer;			//Layer on which collision will be checked.
	private Vector2 startPosition;
	private Vector2 endPosition;
	private bool isHeadedToEnd = true;

	protected enum Directions {LEFT, RIGHT, UP, DOWN, STAY};

	// Use this for initialization
	protected override void Start () {
		startX = (int)transform.position.x;
		startY = (int)transform.position.y;
		this.startPosition = new Vector2 (startX, startY);
		this.endPosition = new Vector2 (endX, endY);
		GameManager.instance.addEnemyToList (this);
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MoveEnemy() {
		// If we've reached the end position, we want to head back to the start position
		if (transform.position.x == endPosition.x && transform.position.y == endPosition.y) {
//			Debug.Log ("reached end");
			switchStartEndPositions();
		}

		beginMoveCycle (false);
	}

	private void beginMoveCycle(bool alreadyRedirected) {
		List<Directions> failedMoves = new List<Directions>();
		for (int i = 0; i < 4; i++) {
			Directions next_step = this.getNextStepDirection (failedMoves);
			if (!alreadyRedirected && checkEnemyCollision (next_step)) {
				Debug.Log("collision with npc!");
				switchStartEndPositions ();
				beginMoveCycle (true);
				return;
			}
			if (!this.MoveInDirection (next_step)) {
				failedMoves.Add (next_step);
			} else {
				return;
			}
		}
	}

	private void switchStartEndPositions() {
		isHeadedToEnd = !isHeadedToEnd;
		endPosition = startPosition;
		if (isHeadedToEnd) {
			endPosition = new Vector2 (endX, endY);
		} else {
			endPosition = new Vector2 (startX, startY);
		}
		startPosition = new Vector2(transform.position.x, transform.position.y);
	}

	protected Directions getNextStepDirection(List<Directions> failedMoves){
//		Debug.Log ("startPosition: " + this.startPosition);
//		Debug.Log ("endPosition: " + this.endPosition);
//		Debug.Log ("transform.position: " + transform.position);
//
		//		Debug.Log ("in getNextStepDirection");
		// attempts to go in a direction towards the end point - if that's not
		// possible, travels in another direction

		if (transform.position.x > endPosition.x || (failedMoves.Count >= 2 && !(failedMoves.Contains(Directions.LEFT)))) {
			return Directions.LEFT;
		} else if (transform.position.x < endPosition.x || (failedMoves.Count >= 2 && !(failedMoves.Contains(Directions.RIGHT)))) {
			return Directions.RIGHT;
		} else if (transform.position.y > endPosition.y || (failedMoves.Count >= 2 && !(failedMoves.Contains(Directions.DOWN)))) {
			return Directions.DOWN;
		} else if (transform.position.y < endPosition.y || (failedMoves.Count >= 2 && !(failedMoves.Contains(Directions.UP)))) {
			return Directions.UP;
		}

		return Directions.STAY;
	}

	private bool checkEnemyCollision(Directions direction) {
		Debug.Log("checkCollision " + direction);
		switch (direction) {
		case Directions.LEFT:
			return this.isCollision(-1, 0, enemyLayer);
		case Directions.RIGHT:
			return this.isCollision(1, 0, enemyLayer);
		case Directions.UP:
			return this.isCollision(0, 1, enemyLayer);
		case Directions.DOWN:
			return this.isCollision(0, -1, enemyLayer);
		default:
			return false;
		}
	}

	protected bool MoveInDirection(Directions direction) {
		//Hit will store whatever our linecast hits when Move is called.
		bool didMove;
		switch (direction) {
		case Directions.LEFT:
			didMove = this.Move (-1, 0);
			break;
		case Directions.RIGHT:
			didMove = this.Move (1, 0);
			break;
		case Directions.UP:
			didMove = this.Move (0, 1);
			break;
		case Directions.DOWN:
			didMove = this.Move (0, -1);
			break;
		default:
			didMove = true;
			break;
		}
		return didMove;

	}
		
}
