using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ViewMode {
	fp, 
	tp, 
	td,
}

enum States {
	FirstMenu, 
	GameOverMenu, 
	Game, 
	Win, 
	MovingCamera, 
	Lose,
}

public class box : MonoBehaviour {
	/*======================== VARIABLES ===========================*/
	// UI Elements
		public 	Text 		WinLose;
		public 	Text 		TimerTitle;
		public 	Text		HeartsTitle;
		public 	Text 		HudHeart;
		public 	Text 		Timer;
		public 	Image 		Logo;
		public 	GameObject 	MenuButtons;
		public 	GameObject 	GameOverMenuButtons;
		
	// Code Variables
		bool 		isMovingCamera 	= false;
		int 		count  			= 0;
		bool 		isMenu 			= true;
		int 		hearts 			= 3;
		string 		heart  			= "❤";
		float 		StartTime;
		ViewMode 	viewMode;
		Vector3 	Cameraoffset;
		Vector3 	LightOffset;
		private 	States state;

	// Game Objects
		public 	Light 		TheLight;
		Rigidbody 			r;

	// Default Values
	private Scrollbar s;
		Vector3 	DefaultPosition;
		Vector3 	ThirdPCamera 			= new Vector3(4f, 9.45f, -.9f);
		Vector3 	MenuCamera 				= new Vector3 (7.99f, -1, -4.60f);
		Vector3 	FirstPCamera 			= new Vector3 ( 7.9f, 3.42f, -3.83f);
		Quaternion 	UpdownCamera 			= Quaternion.Euler (90, 0, 0);
		Vector3 	ThreeDCamera 			= new Vector3 (-2.61f, 11.2f, -4.87f);
		Quaternion 	ThreeDCameraRotation 	= Quaternion.Euler (120, -145, -180);


	/*============================ START =========================*/
	// Use this for initialization
	void Start () {
		// First Things First
		GameOverMenuButtons.gameObject.SetActive (false);
		HudHeart.text = heart + " " + heart + " " + heart;
		WinLose.text = "";
		r = gameObject.GetComponent<Rigidbody> ();
		Timer.text = "0.0";
		// Camera
		Camera.main.transform.position = MenuCamera;
		DefaultPosition = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,gameObject.transform.position.z);
		viewMode = ViewMode.tp;
		Cameraoffset = FirstPCamera - DefaultPosition;
		LightOffset = TheLight.transform.position - DefaultPosition;

		// Set State
		state = States.FirstMenu;
	}

	/*======================== UPDATE ============================*/
	// Update is called once per frame
	void Update () {
		switch (state) {
			case States.FirstMenu:
				WinLose.gameObject.SetActive(false);
				HudHeart.gameObject.SetActive (false);
				Timer.gameObject.SetActive(false);
				HeartsTitle.gameObject.SetActive(false);
				TimerTitle.gameObject.SetActive(false);
				MenuButtons.gameObject.SetActive (true);
				Logo.gameObject.SetActive (true);
				break;
			case States.Game:
				playGame ();
				break;
			case States.MovingCamera:
				MoveCameratoPlay ();
				break;
			case States.Lose:
				WinLose.gameObject.SetActive(true);
				WinLose.color = Color.red;
				WinLose.text = "YOU LOST BLYAT";
				state = States.GameOverMenu;
				break;
			case States.GameOverMenu:
				HudHeart.gameObject.SetActive (false);
				HeartsTitle.gameObject.SetActive(false);
				TimerTitle.gameObject.SetActive(false);
				Timer.gameObject.SetActive(false);
				GameOverMenuButtons.gameObject.SetActive (true);
				Logo.gameObject.SetActive (true);
				break;
			case States.Win:
				WinLose.gameObject.SetActive(true);
				WinLose.color = Color.green;
				WinLose.text = "YOU WON BLYAT";
				state = States.GameOverMenu;
				break;
		}
	}


	/*========================== METHODS =============================*/
	private void playGame()
	{
		ShowHud(true);
		CountTime();
		ControlKeys ();
		controlCamera ();
		failCheck ();
		winCheck();
	}

	private void ShowHud(bool show)
	{
		HudHeart.gameObject.SetActive (show);
		HeartsTitle.gameObject.SetActive(show);
		TimerTitle.gameObject.SetActive(show);
		Timer.gameObject.SetActive(show);	
	}
	
	private void CountTime()
	{
		Timer.text = (Time.time - StartTime).ToString("0.0");
	}
	
	private void winCheck()
	{
		if (gameObject.transform.position.z > 1.5) {
			gameObject.transform.position = DefaultPosition;
			r.velocity = Vector3.zero;
			state = States.Win;
		}
	} 

	private void failCheck() {
		if (gameObject.transform.position.y < -20) {
			gameObject.transform.position = DefaultPosition;
			r.velocity = Vector3.zero;
			hearts--;
			HudHeart.text = "";
			printHearts (hearts);
			Debug.Log ("Failed");
		}
		if (hearts == 0) {
			state = States.Lose;
			Debug.Log("LOSE");
		}
	}

	private void printHearts(int number)
	{
		HudHeart.text = "";
		for(int i = 0; i < number; i++){
			HudHeart.text += heart + " ";
		}
		if (hearts == 1) {
			HudHeart.color = Color.red;
		}
	}

	private void ControlKeys() {
		if (Input.GetKey (KeyCode.W)) 	{ r.velocity = Vector3.forward * 5; }
		if (Input.GetKey (KeyCode.S)) 	{ r.velocity = Vector3.forward * -5; }
		if (Input.GetKey (KeyCode.D)) 	{ r.velocity = Vector3.right   * 5; }
		if (Input.GetKey (KeyCode.A)) 	{ r.velocity = Vector3.right   * -5; }
	}

	private void controlCamera() {
		TheLight.transform.position = gameObject.transform.position + LightOffset;
		if (viewMode == ViewMode.fp) {
			Camera.main.transform.position = gameObject.transform.position + Cameraoffset;
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
	}


	public void PlayMenu() {
		MenuButtons.gameObject.SetActive (false);
		Logo.gameObject.SetActive (false);
		state = States.MovingCamera;
	}

	public void RetryMenu() {
		WinLose.gameObject.SetActive(false);
		GameOverMenuButtons.gameObject.SetActive (false);
		ShowHud(true);
		Logo.gameObject.SetActive (false);
		gameObject.transform.position = DefaultPosition;
		HudHeart.color = Color.green;
		hearts = 3;
		printHearts (hearts);
		StartTime = Time.time;
		state = States.Game;
	}

	private void MoveCameratoPlay() {
		if (count++ < 60) {
			Camera.main.transform.position += (ThirdPCamera - MenuCamera) / 60;
		} else {
			count = 0;
			StartTime = Time.time;
			state = States.Game;
		}
	}
}