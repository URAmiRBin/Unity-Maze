using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ViewMode {
	fp, tp, td,
}

enum State {
	FirstMenu, GameOverMenu, Game, Win,
}

public class box : MonoBehaviour {
	// UI Elements
		public Text HudHeart;
		public Image Logo;
	// Code Variables
		int count = 0;
		bool isMenu = true;
		int hearts = 3;
		string heart = "❤";
		bool isMovingCamera = false;
		ViewMode viewMode;
		Vector3 Cameraoffset;
		Vector3 LightOffset;
	// Game Objects
		public GameObject MenuButtons;
		public GameObject GameOverMenuButtons;
		public Light TheLight;
		Rigidbody r;
	// Default Values
		Vector3 DefaultPosition;
		Vector3 ThirdPCamera = new Vector3(2.88f, 9.45f, -.9f);
		Vector3 MenuCamera = new Vector3 (7.99f, -1, -4.60f);
		Vector3 FirstPCamera = new Vector3 ( 7.9f, 3.42f, -3.83f);
		Quaternion UpdownCamera = Quaternion.Euler (90, 0, 0);
		Vector3 ThreeDCamera = new Vector3 (-2.61f, 11.2f, -4.87f);
		Quaternion ThreeDCameraRotation = Quaternion.Euler (120, -145, -180);
	// Use this for initialization
	void Start () {
		// First Things First
		GameOverMenuButtons.gameObject.SetActive (false);
		HudHeart.text = heart + " " + heart + " " + heart;
		r = gameObject.GetComponent<Rigidbody> ();

		// Camera
		Camera.main.transform.position = MenuCamera;
		DefaultPosition = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,gameObject.transform.position.z);
		viewMode = ViewMode.tp;
		Cameraoffset = FirstPCamera - DefaultPosition;
		LightOffset = TheLight.transform.position - DefaultPosition;

		// Set State

	}
		
	// Update is called once per frame
	void Update () {
		if (isMenu) {
			if (hearts == 0) {
				GameOverMenuButtons.gameObject.SetActive (true);
				MenuButtons.gameObject.SetActive (false);
			}
			else if (isMovingCamera) {
				MoveCameratoPlay ();
				MenuButtons.gameObject.SetActive (!true);
			} else if ((!isMovingCamera)) {
				HudHeart.gameObject.SetActive (false);
				MenuButtons.gameObject.SetActive (true);
			}
		} else {
			if (gameObject.transform.position.y < -20) {
				gameObject.transform.position = DefaultPosition;
				r.velocity = Vector3.zero;
				hearts--;
				HudHeart.text = "";
				for(int i = 0; i < hearts; i++){
					HudHeart.text += heart + " ";
				}
				Debug.Log ("Failed");
			}
			if (gameObject.transform.position.z > 1.5) {
				gameObject.transform.position = DefaultPosition;
				r.velocity = Vector3.zero;
				Debug.Log ("Win");
			}

			if (hearts == 0) {
				isMenu = true;
				Camera.main.transform.position = MenuCamera;
			}

			if (Input.GetKey (KeyCode.W)) {
				r.velocity = Vector3.forward * 5;
			}
			if (Input.GetKey (KeyCode.S)) {
				r.velocity = Vector3.forward * -5;
			}
			if (Input.GetKey (KeyCode.D)) {
				r.velocity = Vector3.right * 5;
			}
			if (Input.GetKey (KeyCode.A)) {
				r.velocity = Vector3.right * -5;
			}

			if (Input.GetKeyDown (KeyCode.V)) {
				if (viewMode == ViewMode.tp) {
					Camera.main.transform.position = FirstPCamera;
					Camera.main.transform.rotation = UpdownCamera;
					viewMode = ViewMode.fp;
				} else if (viewMode == ViewMode.fp) {
					Camera.main.transform.position = ThirdPCamera;
					Camera.main.transform.rotation = UpdownCamera;
					viewMode = ViewMode.td;
				} else if (viewMode == ViewMode.td) {
					Camera.main.transform.position = ThreeDCamera;
					Camera.main.transform.rotation = ThreeDCameraRotation;
					viewMode = ViewMode.tp;
				}
			}

			if (viewMode == ViewMode.fp) {
				Camera.main.transform.position = gameObject.transform.position + Cameraoffset;
			}

			TheLight.transform.position = gameObject.transform.position + LightOffset;

		}
	}


	public void PlayMenu() {
		Logo.gameObject.SetActive (false);
		MoveCameratoPlay ();
	}

	void MoveCameratoPlay() {
		if (count++ < 60) {
			Camera.main.transform.position += (ThirdPCamera - MenuCamera) / 60;
			isMovingCamera = true;
		} else {
			count = 0;
			HudHeart.gameObject.SetActive (!false);
			isMovingCamera = false;
			isMenu = false;
		}
	}
}