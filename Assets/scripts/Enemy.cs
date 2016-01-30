using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject {
	private Vector2 startPosition;
	private Vector2 endPosition;
	private Vector2 currentPosition;

	// Use this for initialization
	protected override void Start () {
		startPosition = new Vector2 (0, 0);
		currentPosition = new Vector2 (0, 0);
		endPosition = new Vector2 (10, 10);
		GameManager.instance.addEnemyToList (this);
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MoveEnemy() {
		// If we've reached the end position, we want to head back to the start position
		if (currentPosition == endPosition) {
			endPosition = startPosition;
			startPosition = currentPosition;
		}

		bool moved = false;
		List<Directions> failedMoves = new List<Directions>();
		while (!moved && failedMoves.Count < 4) {
			Directions next_step = this.getDoStep (failedMoves);
			moved = this.MoveInDirection (next_step);
		}
	}

	protected Directions getDoStep(List<Directions> failedMoves){
		// attempts to go in a direction towards the end point - if that's not
		// possible, travels in another direction

		if (currentPosition.x > endPosition.x || (failedMoves.Count >= 2 && !(failedMoves.Contains(Directions.LEFT)))) {
			return Directions.LEFT;
		} else if (currentPosition.x < endPosition.x || (failedMoves.Count >= 2 && !(failedMoves.Contains(Directions.RIGHT)))) {
			return Directions.RIGHT;
		} else if (currentPosition.y > endPosition.y || (failedMoves.Count >= 2 && !(failedMoves.Contains(Directions.DOWN)))) {
			return Directions.DOWN;
		} else if (currentPosition.y < endPosition.y || (failedMoves.Count >= 2 && !(failedMoves.Contains(Directions.UP)))) {
			return Directions.UP;
		}

		return Directions.STAY;
	}
		
}
