using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BallS : MonoBehaviour
{
	public static float xPframe; // The number of units that the ball moves in the X direction every frame
	public static float yPframe; // The number of units that the ball moves in the Y direction every frame
	public static float bSpeed;  // The speed of the ball
	static float bPos; 			 // The horizontal position of the ball
	float sPos;					 // The vertical position of the ball
	float pPos;					 // The position of the center of the opponent's paddle
	public static float cDown;	 // The amount of time remaining before the ball is put in play
	public GameObject Ball;

	bool start;	 // If false, the game is in countdown mode, and has not yet started

	// What loads on the first frame of a new level. This sets the timer for the initial countdown
	void Start () {
		cDown = 5;
		start = false;
	}

	// Determines what happens on each individual frame
	void FixedUpdate ()	{
		if (!Level.pause) {
			Vector3 ballGo = new Vector3 (xPframe, yPframe, 0.0f);	
			GetComponent<Rigidbody> ().velocity = ballGo * bSpeed;
			if (!start) {
				yPframe = 0.0f;
				xPframe = -1.0f;
				bSpeed = 0.0f;
				cDown -= Time.deltaTime;
				Level.topT.text = Mathf.Ceil (cDown).ToString ();
				if (cDown <= 0.0f) {
					cDown = 0.0f;
					bSpeed = 20.0f + Boost (2.50f);
					start = true;
					Level.topT.text = "";
				}
			}
			Constrain ();
		} else {
			Vector3 ballGo = new Vector3 (0.0f, 0.0f, 0.0f);	
			GetComponent<Rigidbody> ().velocity = ballGo * 0.0f;
		}
	}

	// Determines what happens when the ball collides with something
	void OnTriggerEnter (Collider other) {
		if (other.tag == "PaddleL") {
			RefAndSpeed ();
			pPos = GameObject.Find ("PaddleL").GetComponent<Rigidbody> ().position.y;
			YPos (bPos, pPos);
		}
		if (other.tag == "PaddleR") {
			RefAndSpeed ();
			pPos = GameObject.Find ("PaddleR").GetComponent<Rigidbody> ().position.y;
			YPos (bPos, pPos);
			OppPaddle.y1 = 0.0f;
			OppPaddle.y2 = 0.0f;
			OppPaddle.slope = 0.0f;
			OppPaddle.orgTarget = 0.0f;
			OppPaddle.revTarget = OppPaddle.target;
			OppPaddle.target = 0.0f + (Random.Range(-500, 500) / 100.0f);
			if (OppPaddle.curr - OppPaddle.target < OppPaddle.range) {
				OppPaddle.direct = 1;
			} else if (OppPaddle.curr - OppPaddle.target > OppPaddle.range) {
				OppPaddle.direct = -1;
			} else {
				OppPaddle.direct = 0;
			}
			OppPaddle.setTarget = false;
		}
		if (other.tag == "EBarr") {
			if (xPframe >= 0.0f) {
				OppPaddle.y1 = bPos;
			}
		}
		if (other.tag == "LBarr") {
			if (xPframe >= 0.0f) {
				OppPaddle.y2 = bPos;
				OppPaddle.setTarget = true;
			}
		}
		if (other.tag == "Spire") {
			RefAndSpeed ();
			yPframe += Random.Range (-0.1f, 0.1f);
		}
		if (other.tag == "Acel") {
			bSpeed *= Random.Range(1.05f, 1.30f) + Boost(0.05f) + RandA ();
			Limit ();
		}
		if (other.tag == "Dcel") {
			bSpeed *= Random.Range(0.80f, 0.95f) - (0.15f + Boost(0.05f)) + RandA ();
			Level.topT.text = "";
			Limit ();
		}
		if (other.tag == "Deflect") {
			yPframe *= -1;
			yPframe += Random.Range (-0.1f, 0.1f);
		}
		if (other.tag == "Warp") {
			if (Random.Range(1,6) % 2 == 0) {
				xPframe *= -1;
			}
			yPframe += Random.Range (-1.8f, 1.8f);
			bSpeed *= Random.Range(0.80f, 1.20f);
			Limit ();
		}
	}

	// Controls the balls behavior when it strikes a paddle
	void RefAndSpeed ()	{
		xPframe *= -1;
		bSpeed *= 1.06f + Boost(0.02f) + RandA ();
		Limit ();
	}

	// Determines the direction the ball will go after striking a paddle
	void YPos (float b, float p) {
		float div = 2.75f;
		yPframe = (b - p) / div;
		if (yPframe > 4 / div) {
			yPframe = 4 / div + RandA ();
		} else if (yPframe < -4 / div) {
			yPframe = -4 / div + RandA ();
		}
	}

	// Places a new ball in play
	public static void SpawnBall () {
		Vector3 rY = new Vector3 (0, Random.Range(-2500, 2500) / 100.0f);
		GameObject.Find ("Ball").GetComponent<Rigidbody> ().position = rY;
		if (Level.scoreLeft >= Level.goal || Level.scoreRight >= Level.goal) {
			bSpeed = 0.0f;
		} else {
			yPframe = 0.0f;
			bSpeed = 20.0f + Boost(2.50f) ;
			if (Level.diff == 0) {
				OppPaddle.target = 0.0f + (Random.Range(-1500, 1500) / 100.0f);	
			} else if (Level.diff == 2) {
				OppPaddle.target = 0.0f + (Random.Range(-500, 500) / 100.0f);
			} else {
				OppPaddle.target = 0.0f + (Random.Range(-1000, 1000) / 100.0f);
			}

		}
	}

	// Limits the speed of the ball
	void Limit() {
		if (bSpeed < 20.0f) {
			bSpeed = 20.0f;
		}

		if (bSpeed > 75.0f) {
			bSpeed = 75.0f;
			Level.topT.text = "MAX SPEED";
		}
	}

	// Keeps the ball inside the boundries of the game
	void Constrain () {
		bPos = Mathf.Clamp(GetComponent<Rigidbody> ().position.y, Level.bUnder + 1.5f, Level.tOver - 1.5f);
		sPos = GetComponent<Rigidbody> ().position.x;
		Vector3 postClamp = new Vector3 (GetComponent<Rigidbody> ().position.x, bPos);
		GetComponent<Rigidbody> ().position = postClamp;
		if (bPos >= Level.tOver - 1.5f) {
			yPframe = -Mathf.Abs (yPframe);
		}
		if (bPos <= Level.bUnder + 1.5f) {
			yPframe = Mathf.Abs (yPframe);
		}
		if (sPos > 80) {
			Level.topT.text = "";
			cDown = 0;
			xPframe = 1;
			Level.scoreLeft += 1;
			Level.UpdateScore ();
			SpawnBall ();	
		}
		if (sPos < -80) {
			Level.topT.text = "";
			cDown = 0;
			xPframe = -1;
			Level.scoreRight += 1;
			Level.UpdateScore ();
			SpawnBall ();
		}
		if (sPos > 50 && sPos <= 80) {
			bSpeed = 10.0f;
			cDown = Mathf.Ceil ((80.0f - sPos) / 10);
			Level.topT.text = cDown.ToString();
		}
		if (sPos < -50 && sPos >= -80) {
			bSpeed = 10.0f;
			cDown = Mathf.Ceil ((80.0f - Mathf.Abs (sPos)) / 10);
			Level.topT.text = cDown.ToString();
		}
	}

	// Returns a random value, in order to add variance and unpredictibility.
	float RandA () {
		return Random.Range (-5.0f, 5.0f) / 100.0f;
	}

	// Returns a value that causes the ball to accelerate more when the difficulty is higher.
	static float Boost(float x) {
		return x * Level.diff;
	}
}
