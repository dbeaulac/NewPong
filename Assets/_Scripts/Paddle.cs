using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour {
	public int pSpeed;

	// Determines what happens on each individual frame
	void FixedUpdate () {
		if (Level.level > 0 && !Level.pause) {
			float move = Input.GetAxis ("Vertical");
			float move2 = Input.GetAxis ("Horizontal");

			Vector3 moveV = new Vector3 (0.0f, move, 0.0f);
			Vector3 moveV2 = new Vector3 (0.0f, move2, 0.0f);

			if (move != 0.0f) {
				GetComponent<Rigidbody> ().velocity = moveV * pSpeed;
			} else if (move2 != 0.0f) {
				GetComponent<Rigidbody> ().velocity = moveV2 * pSpeed * 2;
			} else {
				GetComponent<Rigidbody> ().velocity = Vector3.zero;
			}
		} else {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}
	}
}
