/* Font used is SquareFont by Bou Fonts
 * www.dafont.com/squarefont.font
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {
	public static int scoreLeft = 0;			// Determines the player score
	public static int scoreRight = 0;			// Determines the opponent score

	public static float tOver = 28.0f;			// The upper bound of the game
	public static float bUnder = -28.0f;		// The lower bound of the game
	public static float eB;						// The first barrier that helps the AI determine ball position
	public static float lB;						// The second barrier that helps the AI determine ball position

	public static Text ScoreL;					// The text object that holds the player's score
	public static Text ScoreR;					// The text object that holds the opponent's score
	public static Text topT;					// The text object that holds the value of the countdown timer
	public static Text botT;					// The text object that holds the prompts to the player

	public static Toggle EasyT;					// The checkbox for the Easy difficulty
	public static Toggle NormalT;				// The checkbox for the Normal difficulty
	public static Toggle HardT;					// The checkbox for the Hard difficulty

	public static int goal;						// The target score which decides when the game ends

	public static int level = 0;				// The game level (0 is the splash page)
	public static int diff = 1;					// The difficulty level (0 = Easy, 1 = Normal, 2 = Hard)

	public static bool classic = false;			// If true: Classic mode is active
	public static bool advance = false;			// If true: Player advances to next level by pressing the spacebar
	public static bool toMenu = false;			// If true: Player returns to the main menu by pressing the spacebar
	public static bool pause = false;			// If true: Game is paused, screen is covered, and controls are locked.
	Vector3 pauseYes = new Vector3 (0.0f, 69.420f, -4.6f);
	Vector3 pauseNo = new Vector3 (0.0f, 0.0f, -4.6f);

	// Determines what happens on each individual frame
	void FixedUpdate(){
		if (advance || toMenu ) {
			BallS.bSpeed = 0.0f;
			if (advance && Input.GetButtonDown("Jump")) {
				level++;
				NewLevel ();
			}
			if (toMenu && Input.GetButtonDown("Jump")) {
				level = 0;
				NewLevel ();
			}
		} else {
			if (BallS.cDown <= 0.0f) {
				if (Input.GetButtonDown ("Jump")) {
					if (pause) {
						pause = false;
						//topT.text = "";
						GameObject.Find ("pauseScreen").GetComponent<Rigidbody> ().MovePosition(pauseYes);
					} else {
						pause = true;
						//topT.text = "PAUSE";
						GameObject.Find ("pauseScreen").GetComponent<Rigidbody> ().MovePosition(pauseNo);
					}
				}
				if (pause && Input.GetKeyDown("return")) {
					Application.Quit();
					topT.text = "EXIT";
				}
			}
		}
	}

	// This resets the game for the next level.
	void NewLevel() {
		scoreLeft = 0;
		scoreRight = 0;
		UpdateScore ();
		SceneManager.LoadScene (level);
		advance = false;
		toMenu = false;
	}

	// What loads on the first frame of a new level. This initializes the components used in the game.
	void Start () { 
		if (level > 0) {
			tOver = GameObject.Find ("Top").GetComponent<Rigidbody> ().position.y;
			bUnder = GameObject.Find ("Bottom").GetComponent<Rigidbody> ().position.y;
			eB = GameObject.Find ("Early").GetComponent<Rigidbody> ().position.x;
			lB = GameObject.Find ("Late").GetComponent<Rigidbody> ().position.x;

			ScoreL = GameObject.Find ("ScoreL").GetComponent<Text> ();
			ScoreR = GameObject.Find ("ScoreR").GetComponent<Text> ();
			topT = GameObject.Find ("Middle").GetComponent<Text> ();
			botT = GameObject.Find ("Other").GetComponent<Text> ();

			if (classic) {
				botT.text = "Classic Pong      First to " + goal + " wins";			
			} else {
				botT.text = "Level " + level + "      First to " + goal + " wins";
			}

			//b = Ball.GetComponent<BallS> ();
			BallS.SpawnBall ();
		} else {
			EasyT = GameObject.Find ("ToggleE").GetComponent<Toggle> ();
			NormalT = GameObject.Find ("ToggleN").GetComponent<Toggle> ();
			HardT = GameObject.Find ("ToggleH").GetComponent<Toggle> ();			
		}
	}

	// Sets the difficulty. 0 = Easy, 1 = Normal, 2 = Hard
	public static int DiffCheck() {
		if (EasyT.isOn) {
			return 0;
		} else if (HardT.isOn) {
			return 2;
		} else {
			return 1;
		}
	}

	// Activates Classic Pong
	public void ClassicPong() {
		diff = DiffCheck ();
		goal = 11;
		level = 1;
		classic = true;
		SceneManager.LoadScene (level);
	}

	// Activates New Pong
	public void NewPong() {
		diff = DiffCheck ();
		goal = 7;
		level = 1;
		classic = false;
		SceneManager.LoadScene (level);
	}

	// Updates the score, and prompts the user if the game is over
	public static void UpdateScore() {
		ScoreL.text = scoreLeft.ToString();
		ScoreR.text = scoreRight.ToString();

		if (classic || level == 5) {
			if (scoreLeft >= goal) {
				botT.text = "You win!      Press 'Space' to return to menu.";
				toMenu = true;
			}
			if (scoreRight >= goal) {
				botT.text = "You lose      Press 'Space' to return to menu. ";
				toMenu = true;
			}
		} else {
			if (scoreLeft >= goal) {
				botT.text = "Level " + level + " cleared!      Press 'Space' to continue.";
				advance = true;
			}
			if (scoreRight >= goal) {
				botT.text = "You lose      Press 'Space' to return to menu.";
				toMenu = true;
			}
		}

	}
}
