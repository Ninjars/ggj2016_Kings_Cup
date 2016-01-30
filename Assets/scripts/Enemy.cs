using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {
	private int startX;
	private int startY;
	public int endX;
	public int endY;
	private Vector2 startPosition;
	private Vector2 endPosition;

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
			Debug.Log ("reached end");
			endPosition = startPosition;
			startPosition = new Vector2(transform.position.x, transform.position.y);
		}

		bool moved = false;
		List<Directions> failedMoves = new List<Directions>();
		while (!moved && failedMoves.Count < 4) {
			Debug.Log ("hello from moved loop");
			Directions next_step = this.getDoStep (failedMoves);
			moved = this.MoveInDirection (next_step);
			if (!moved) {
				failedMoves.Add (next_step);
			}
		}
	}

	protected Directions getDoStep(List<Directions> failedMoves){
		Debug.Log ("startPosition: " + this.startPosition);
		Debug.Log ("endPosition: " + this.endPosition);
		Debug.Log ("transform.position: " + transform.position);

		Debug.Log ("in getDoStep");
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
		
}
