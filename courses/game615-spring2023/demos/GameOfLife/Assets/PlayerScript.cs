using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	float moveSpeed = 5f;
	float rotateSpeed = 60f;
	float jumpForce = 0.35f;

	//NOTE: Changing this will drastically affect the jumpForce and fall speed.
	float gravityModifier = 0.2f;

	float yVel = 0;
	bool previousIsGroundedValue;

	public CharacterController cc;

	//Camera controls (we can tweak these to make the camera follow differently - see Update())
	float camLookAhead = 8f;
	float camFollowBehind = 5f;
	float camFollowAbove = 3f;

	// Start is called before the first frame update
	void Start()
	{
		//Initialize the value of this value (what it does is described in Update())
		previousIsGroundedValue = cc.isGrounded;
	}

	// Update is called once per frame
	void Update()
	{
		float hAxis = Input.GetAxis("Horizontal");
		float vAxis = Input.GetAxis("Vertical");

		//--- ROTATION ---
		//Rotate on the y axis based on the hAxis value
		//NOTE: If the player isn't pressing left or right, hAxis will be 0 and there will be no rotation
		transform.Rotate(0, hAxis * rotateSpeed * Time.deltaTime, 0);


		//--- DEALING WITH GRAVITY ---
		if (cc.isGrounded) {
			if (!previousIsGroundedValue) {
				//By being in this if statement, we know we JUST landed.
				//NOTE: We know we just landed because cc.isGrounded is true (meaning
				//		on the last cc.Move() call we collided with something. This condition also
				//		required previousIsGroundedValue to be false which means we were not colliding
				//		with the ground on the previous Update.

				//Set the yVel to zero right when we hit the ground so that we don't accumulate 
				//a bunch of yVel. The CharacterController will prevent us from moving through
				//the floor, but we are managing the yVel ourselves, so we need to make sure
				//that it is zero when we start to fall. This is where we do that.
				yVel = 0;
			}

			//JUMP. When the player presses space, set yVel to the jump force. This will immediately
			//make the player start moving upwards, and gravity will start slowing the movement upward
			//and eventually make the player hit the ground (thus landing in the 'if' statment above)
			if (Input.GetKeyDown(KeyCode.Space)) {
				yVel = jumpForce;
			}
		} else {
			//If we're not on the ground, apply "gravity" to yVel
			yVel = yVel + Physics.gravity.y * gravityModifier * Time.deltaTime;

			//If we are moving upward (yVel > 0), and the player has released the jump button
			//start falling downward (by setting the yVel to 0)
			if (Input.GetKeyUp(KeyCode.Space) && yVel > 0) {
				yVel = 0;
			}
		}

		//--- TRANSLATION ---
		//Move the player forward based on the vAxis value
		//NOTE: If the player isn't pressing up or down, vAxis will be 0 and there will be no movement
		//		based on input. However, yVel will still move the player downward if they are
		//		not colliding with the ground anymore
		Vector3 amountToMove = transform.forward * vAxis * moveSpeed * Time.deltaTime;

		//Set the amount we move in the y direction to be whatever we have gotten from simulating physics
		amountToMove.y = yVel;

		//This will move the player according to the forward vector and the yVel using the
		//CharacterController.
		cc.Move(amountToMove);

		//Store our current previousIsGroundedValue (so we can do that check to see if we just
		//landed above as described above)
		//NOTE: After cc.Move() is called, cc.isGrounded is updated to relfect whether that Move()
		//		function call collided with the ground.
		previousIsGroundedValue = cc.isGrounded;


		//-- CAMERA --
		//Like the flight simulator tutorial, we will position the camera behind and above the player
		Vector3 cameraPosition = transform.position + (Vector3.up * camFollowAbove) + (-transform.forward * camFollowBehind);
		Camera.main.transform.position = cameraPosition;
		Camera.main.transform.LookAt(transform.position + transform.forward * camLookAhead);
	}

}
