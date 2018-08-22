using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

enum ViewMode 	{ fp, tp, td, }
enum States 	{ FirstMenu, GameOverMenu, Game, Win, MovingCamera, Lose, Options }

public class box : MonoBehaviour {
	/*======================== VARIABLES ===========================*/
	// UI Elements
		public	Text 		Records;
		public	Text 		GroundColorText;
		public	Text 		BallColorText;
		public	Text 		Score;
		public	Text 		BestScoreTitle;
		public 	Text 		WinLose;
		public 	Text 		TimerTitle;
		public 	Text		HeartsTitle;
		public 	Text 		HudHeart;
		public 	Text 		Timer;
		public 	Image 		Logo;
		public 	GameObject 	MenuButtons;
		public	GameObject	Ground;
		public 	GameObject 	GameOverMenuButtons;
		public 	Dropdown 	BallColorDD;
		public 	Dropdown 	GroundColorDD;
		public 	Toggle 		LightToggle;
		public 	Button		Back;

	// Code Variables
		private 	bool 		isMovingCamera 	= false;
		private 	int 		count  			= 0;
		private 	bool 		isMenu 			= true;
		private 	int 		hearts 			= 3;
		private 	string 		heart  			= "❤";
		private 	float 		StartTime;
		private 	ViewMode 	viewMode;
		private 	Vector3 	Cameraoffset;
		private 	Vector3 	LightOffset;
		private 	int 		number;
		private 	States 		state;
		private 	States 		preState;
		private		Material 	m;
		private 	Material 	n;
		private Dictionary<int, float> scores = new Dictionary<int, float>();
	// Game Objects
		public 	Light 		TheLight;
		Rigidbody 			r;

	// Default Values
		Vector3 	DefaultPosition;
		Vector3 	ThirdPCamera 			= new Vector3(4f, 9.45f, -.9f);
		Vector3 	MenuCamera 				= new Vector3 (7.99f, -1, -4.60f);
		Vector3 	FirstPCamera 			= new Vector3 ( 7.9f, 3.42f, -3.83f);
		Quaternion 	UpdownCamera 			= Quaternion.Euler (90, 0, 0);
		Vector3 	ThreeDCamera 			= new Vector3 (-2.61f, 11.2f, -4.87f);
		Quaternion 	ThreeDCameraRotation 	= Quaternion.Euler (120, -145, -180);


	/*============================ START =========================*/
	// Use this for initialization
	void Start ()
	{
		LightToggle.gameObject.GetComponent<Toggle>();
		number = 0;
		m = gameObject.GetComponent<Renderer>().material;
		n = Ground.gameObject.GetComponent<Renderer>().material;
		n.color = Color.black;
		m.color = Color.red;
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
				showOptions(false);
				Records.gameObject.SetActive(false);
				WinLose.gameObject.SetActive(false);
				ShowHud(false);
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
				ShowHud(false);
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
				ShowHud(false);
				WinLose.gameObject.SetActive(true);
				WinLose.color = Color.green;
				WinLose.text = "YOU WON BLYAT";
				state = States.GameOverMenu;
				break;
			case States.Options:
				MenuButtons.gameObject.SetActive(false);
				GameOverMenuButtons.gameObject.SetActive(false);
				BallColorDD.gameObject.SetActive(true);
				GroundColorDD.gameObject.SetActive(true);
				DropDownIO();
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

	private void DropDownIO()
	{
		if (BallColorDD.value == 0){ m.color = Color.red; }
		else if (BallColorDD.value == 1){ m.color = Color.blue; }
		else if (BallColorDD.value == 2){ m.color = Color.yellow; }
		
		if (GroundColorDD.value == 0){ n.color = Color.black; }
		else if (GroundColorDD.value == 1){ n.color = Color.white; }
		else if (GroundColorDD.value == 2){ n.color = Color.green; }

		LightToggle.onValueChanged.AddListener(delegate { dothelight();});

	}

	private void dothelight()
	{
		if (LightToggle.isOn)
		{
			TheLight.gameObject.SetActive(true);
		}
		else
		{
			TheLight.gameObject.SetActive(false);
		}
	}

	private void showOptions(bool show)
	{
		BallColorDD.gameObject.SetActive(show);
		GroundColorDD.gameObject.SetActive(show);
		LightToggle.gameObject.SetActive(show);
		Back.gameObject.SetActive(show);
		BallColorText.gameObject.SetActive(show);
		GroundColorText.gameObject.SetActive(show);

	}
	
	private void countBest()
	{
		float best = 10000;
		for (int i = 0; i < scores.Count; i++)
		{
			var item = scores.ElementAt(i);
			if (item.Value != 0)
			{
				if (item.Value < best)
				{
					best = item.Value;
				}
			}
		}

		if (best == 10000) { Score.text = "-"; }
		else { Score.text = best.ToString("0.0") + " Seconds"; }
	}
	
	private void ShowHud(bool show)
	{
		Score.gameObject.SetActive(show);
		BestScoreTitle.gameObject.SetActive(show);
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
			scores.Add(number++, Time.time - StartTime);
			countBest();
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
			scores.Add(number++, 0);
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
	
	private void MoveCameratoPlay() {
		if (count++ < 60) {
			Camera.main.transform.position += (ThirdPCamera - MenuCamera) / 60;
		} else {
			count = 0;
			StartTime = Time.time;
			state = States.Game;
		}
	}


	public void ScoresMenu()
	{
		ShowHud(false);
		string WL;
		Records.gameObject.SetActive(true);
		Records.text = "Recent Games \n";
		for(int i = scores.Count-1; i >= 0; i--)
		{
			var item = scores.ElementAt(i);
			if (item.Value == 0)
			{
				WL = "LOSE";
			}
			else
			{
				WL = "WIN ";
			}
			
			Records.text += "#" + (scores.Count - i) + " 	|	" + WL + "|		" + item.Value.ToString("0.0") + "\n";
		}
	}


	
	public void PlayMenu() {
		MenuButtons.gameObject.SetActive (false);
		Logo.gameObject.SetActive (false);
		state = States.MovingCamera;
	}

	public void RetryMenu() {
		Records.gameObject.SetActive(false);
		showOptions(false);
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

	public void OptionsMenu()
	{
		preState = state;
		WinLose.gameObject.SetActive(false);
		Logo.gameObject.SetActive(preState == States.FirstMenu);
		GameOverMenuButtons.gameObject.SetActive(false);
		MenuButtons.gameObject.SetActive(false);
		ShowHud(false);
		showOptions(true);
		state = States.Options;
	}

	public void onBack()
	{
		showOptions(false);
		state = preState;
	}

}