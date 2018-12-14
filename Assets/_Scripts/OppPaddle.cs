using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OppPaddle : MonoBehaviour {
	public static float y1 = 0.0f;			// Value y1 when calculating slope.
	public static float y2 = 0.0f;			// Value y2 when calculating slope.
	public static float slope;				// Calculation of slope.
	public static float target;				// The desired y-position of the center of the paddle.
	public static float curr;				// The current y-position of the center of the paddle.
	float pSpeed = 30;						// The speed of the paddle.
	float rP;								// The x-position of the paddle
	public static float range;				// 
	public static float ratio;				// Determines which action should be taken if the ball strikes a boundary.
	public static bool setTarget;			// If true, orders the program to calculate a target for the AI.
	public static float orgTarget;			// The original target, if adjustments for bounces need to be made.
	public static float revTarget;			// The revised target, after adjustments for bounces are made.
	public static int direct;				// The direction that the paddle should move (1: Up, -1: Down, 0: Stay)
	float tL;
	float bL;

	// What loads on the first frame of a new level. This gets the x-position of the paddle
	void Start () {
		rP = GetComponent<Rigidbody> ().position.x;
	}

	// Determines what happens on each individual frame
	void FixedUpdate () {
		tL = Level.tOver - 0.5f;
		bL = Level.bUnder + 0.5f;
		curr = GetComponent<Rigidbody> ().position.y;
		if (y1 == y2) {
			slope = 0.0f;
		} else {
			slope = (y2 - y1) / (Level.lB - Level.eB);		
		}

		if (setTarget == true) {
			SetT ();
			if (curr - target < range) {
				direct = 1;
			} else if (curr - target > range) {
				direct = -1;
			} else {
				direct = 0;
			}
		}

		if (!Level.pause) {
			if (curr - target < range && direct == 1) {
				GetComponent<Rigidbody>().velocity = Vector3.up * pSpeed * 0.1f;	
			} else if (curr - target > range && direct == -1) {
				GetComponent<Rigidbody> ().velocity = Vector3.down * pSpeed * 0.1f;
			} else {
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				direct = 0;
			}
		} else {
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	// Determines where the AI should position their paddle.
	public void SetT () {
		target = (slope * (rP - Level.lB) + y2) + RandA ();
		if (Level.diff == 0) {
			pSpeed = Random.Range (150.0f, 250.0f);
			range = ((pSpeed - 200) / 100.0f);
		} else if (Level.diff == 2) {
			pSpeed = Random.Range (350.0f, 450.0f);
			range = ((pSpeed - 400) / 200.0f);
		} else {
			pSpeed = Random.Range (250.0f, 350.0f);
			range = ((pSpeed - 300) / 150.0f);
		}

		if (target > tL || target < bL) {
			Over ();
		} else {
			orgTarget = target;
			revTarget = target;
		}

		setTarget = false;
	}

	// Calculates where the AI should position their paddle if the ball is going to bounce off of a boundary.
	void Over () {
		orgTarget = target;
		ratio = orgTarget / tL;

		if (orgTarget > tL) {
			revTarget = orgTarget % tL;
			ratio = Mathf.Abs (Mathf.Floor (ratio)) % 4;
		} else {
			revTarget = orgTarget % bL;
			ratio = Mathf.Abs (Mathf.Ceil (ratio)) % 4;
		}

		if (ratio == 1) {
			if (orgTarget > 0) {
				target = tL - revTarget;
			} else {
				target = bL - revTarget;
			}
		} else if (ratio == 2) {
			target = revTarget * -1;
		} else if (ratio == 3) {
			if (orgTarget > 0) {
				target = revTarget - tL;
			} else {
				target = revTarget - bL;
			}
		} else {
			target = revTarget;
		}
	}

	// Returns a random value, in order to add variance and unpredictibility.
	float RandA () {
		if (Level.diff == 0) {
			return Random.Range (-200, 200) / 100.0f;
		} else if (Level.diff == 2) {
			return Random.Range (-50, 50) / 100.0f;
		} else {
			return Random.Range (-100, 100) / 100.0f;			
		}
	}
}
