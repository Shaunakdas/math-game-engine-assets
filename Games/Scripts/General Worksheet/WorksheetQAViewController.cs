using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using SimpleJSON;

public class WorksheetQAViewController : QuesAnsViewController {
	//Display Variables
	int totalOptionCount = 4;
	float currentTime=0,totalTime=0,maxCurrentTime = 90;
	QuesAnsList quesAnsList ;
	public GameObject WorksheetGO;
	WorksheetController WorksheetObject;
	WorksheetQANetworkController WorksheetQANetworkObject;

	//Behind the scene
	UserProfile user;

	//GameObject Reference
	public GameObject testProgressGO, questionProgressGO,questionTextGO,qaPanelGO;

	//Prefabs
	public GameObject ansOption, imagePrefab, tickImage;

	GameObject quesImageGO;
	Texture2D quesTexture;

	// Use this for initialization
	public override void Start () {
		Debug.Log ("WorksheetQAViewController start() called");
		quesAnsList = new QuesAnsList();
		user = new UserProfile();
		WorksheetObject = (WorksheetController) WorksheetGO.GetComponent(typeof(WorksheetController));
		WorksheetQANetworkObject = (WorksheetQANetworkController) gameObject.GetComponent(typeof(WorksheetQANetworkController));
		SwipeManager.OnSwipeDetected += OnSwipeDetected;
	}


	public void setQAList(string qaJSONText){
		Debug.Log ("getQAListAPI started");
		WorksheetQANetworkObject = (WorksheetQANetworkController) gameObject.GetComponent(typeof(WorksheetQANetworkController));
		quesAnsList = new QuesAnsList();
		WorksheetQANetworkObject.setQAListJSON (qaJSONText, quesAnsList);
		setQuesAnsBasedOnIndex (0);
	}
	public QuesAnsList getQAList(){
		return quesAnsList;
	}

//	public void getQAListAPI(){
//		Debug.Log ("getQAListAPI started");
//		WorksheetQANetworkObject = (WorksheetQANetworkController) gameObject.GetComponent(typeof(WorksheetQANetworkController));
//		quesAnsList = new QuesAnsList();
//		StartCoroutine(GetQAList(quesAnsList));
//	}
//	//API Calls
//	IEnumerator GetQAList(QuesAnsList quesAnsList) {
//		yield return StartCoroutine(WorksheetQANetworkObject.getQAListNetworkCall(quesAnsList));
//		Debug.Log ("quesAnsList view opened");
//		setQuesAnsBasedOnIndex (0);
//		WorksheetObject.updateAPIStatus ("GetQAList",WorksheetQANetworkObject.getQAListNetworkCallResponse);
//	}
//	IEnumerator postQuestionAttempt(QuesAnsList quesAnsList){
//		WorksheetObject.openPostWorksheetPanel ();
//		yield return StartCoroutine(WorksheetQANetworkObject.postQAListNetworkCall(quesAnsList));
//		WorksheetObject.updateAPIStatus ("PostQuestionAttempt",WorksheetQANetworkObject.postQAAttemptNetworkCallResponse);
//	}

	//Setting up views
	public void setQuesAnsBasedOnIndex(int index){
		quesAnsList.setUserIndex(index);
		QuesAnsPair currQuesAnsPair = quesAnsList.getCurrentQuesAnsPair ();
		setQuesView (currQuesAnsPair);
		setAnsOpView (currQuesAnsPair);
		setTestProgressView();
		setQuestionProgressView (currQuesAnsPair);
		entryAnim();
	}

	//Setting Question Views
	public  void setQuesView(QuesAnsPair currQuesAnsPair){
		//For setting getCurrentQuesAnsPair.getQuesText ()on view 
		//D var quesText = GameObject.Find("QuesText");
		string question_text = currQuesAnsPair.getQuesText ();
		question_text = StringWrapper.changeString (question_text);
		questionTextGO.GetComponent<TEXDraw>().text  =  "";
		questionTextGO.GetComponent<TEXDraw>().text  =  question_text;
		Debug.Log("Question text"+currQuesAnsPair.getQuesText ());
		Debug.Log("Url of question image"+currQuesAnsPair.getQuesImage ());
		if (currQuesAnsPair.getQuesImage ().Length > 0) {

			StartCoroutine (LoadImage (@currQuesAnsPair.getQuesImage (), qaPanelGO));
		} else{


		}
	}
	//Setting Answer Views
	public  void setAnsOpView(QuesAnsPair currQuesAnsPair){
		Answer answer = currQuesAnsPair.getAnswer ();
		List<string> answerOption = answer.getAnsOptions ();
		List<string> answerOptionImg = answer.getAnsOptionImgs ();
		if (answerOption.Count == 0) {
			changeQuestionIndex (1);

		} else {
			for (int j = 0; j < answer.getAnsOptions ().Count; j = j + 1) {
				GameObject ansOpObject = (GameObject) Instantiate (ansOption,qaPanelGO.transform);
				ansOpObject.name = "AnsOp";
				answerOption [j] = StringWrapper.changeString (answerOption [j]);
				ansOpObject.GetComponentInChildren<TEXDraw> ().text = answerOption [j];

				Button answerButton = ansOpObject.GetComponent<Button> ();
				int tempInt = j;
				answerButton.onClick.AddListener (() => AnswerSelected (tempInt));

				if (j == quesAnsList.getCurrentQuesAnsPair ().getAnswer ().userSelectedOp) {
					Debug.Log ("Correct answer selected + initiating tick"+quesAnsList.getCurrentQuesAnsPair ().getAnswer ().userSelectedOp);
					GameObject tickGO = (GameObject)Instantiate (tickImage,ansOpObject.transform);
					tickGO.name = "TickImg";
					tickGO.transform.position = new Vector3(370f,0f , 0f);
				} else {
					GameObject tick = getChildGameObject (ansOpObject,"TickImg");
					Destroy (tick);
				}
			}
		}
	}

	IEnumerator LoadImage(string @Url,GameObject QAGameObject)
	{
		Debug.Log ("Initiated");

		quesImageGO = Instantiate (imagePrefab,QAGameObject.transform ) as GameObject;
		quesImageGO.transform.SetSiblingIndex (0);
		WWW www = new WWW(Url);
		yield return www;

		Debug.Log ("Loaded");
		quesTexture = www.texture;


		Image img = quesImageGO.GetComponent<Image>();
		img.sprite = Sprite.Create (quesTexture, new Rect (0, 0, quesTexture.width, quesTexture.height),Vector2.zero);
		LayoutElement layout = quesImageGO.GetComponent<LayoutElement>();
		layout.minWidth = 1.5f*quesTexture.width;
		layout.minHeight = 1.5f*quesTexture.height;
	}
	//Setting question progress views
	public void setQuestionProgressView(QuesAnsPair currQuesAnsPair){
		currentTime = (float)currQuesAnsPair.getUserTimeTaken();
		Debug.Log ("Setting current time " + currentTime);
	}
	//Setting test progress views
	public void setTestProgressView(){
		float currentIndex = (float)quesAnsList.getUserIndex ();
		float totalIndex = (float)quesAnsList.getMaxIndex ();
		setProgressBar (currentIndex, totalIndex, testProgressGO);
	}

	//Animation methods
	public override void entryAnim(){
		//For entry animation
	}
	public override void exitAnim(){
		//For exit animation
	}
	public override void correctAnsAnim(){
		//For correct answer animation
	}
	public override void incorrectAnsAnim(){
		//For incorrect animation
	}


	//On Selection of answer
	void AnswerSelected(int buttonNo)
	{
		int solved = 0;
		Debug.Log ("Button clicked = " + buttonNo+ currentTime);
		quesAnsList.getCurrentQuesAnsPair ().getAnswer ().userSelectedOp = buttonNo;
		if (quesAnsList.getCurrentQuesAnsPair ().getAnswer ().correctAnswer (buttonNo)) {
			solved = 3;
		} else {
			solved = 2;
		}
		//GO: Set color of user selected option to light color
		quesAnsList.postQuestionCalculations (solved, (int)(10*currentTime));

		//Starting next question
		changeQuestionIndex(1);
	}

	public void changeQuestionIndex(int increment){
		Destroy (quesImageGO);
		Destroy (quesTexture);

		GameObject[] ansOpTagged = GameObject.FindGameObjectsWithTag ("AnswerOption");
		foreach (GameObject go in ansOpTagged) { 
			Destroy (go);
		}
		quesAnsList.setUserTimeTaken ( (int)(currentTime));
		if (increment > 0) {
			//Going to next question
			if (quesAnsList.getUserIndex () < quesAnsList.getMaxIndex () - 1) {
				int increment_index = quesAnsList.getUserIndex () + increment;
				setQuesAnsBasedOnIndex (increment_index);

			} else {
				//GO: Show end of quiz page
				Debug.Log ("End of quiz reached");
				//StartCoroutine(postQuestionAttempt (quesAnsList));
				WorksheetObject.openPostWorksheetPanel();
			}
		} else {
			//Going to previous question
			if (quesAnsList.getUserIndex() > 0) {
				int increment_index = quesAnsList.getUserIndex () + increment;
				setQuesAnsBasedOnIndex (increment_index);
			}
		}
	}
	//Helper Methods
	public void setProgressBar(float current, float total, GameObject progressGO){
		//D GameObject progressGO = GameObject.Find (gameObjectName);
		if (progressGO != null) {

			Image img = progressGO.GetComponent<Image> ();
			img.fillAmount = (float)(current / total);
		}
	}
	static public GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		//Author: Isaac Dart, June-13.
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
		return null;
	}



	void OnSwipeDetected (Swipe direction)
	{
		switch (direction) {
		case global::Swipe.Left:
			{
				changeQuestionIndex (1);
				break; 
			}
		case global::Swipe.Right:
			{
				changeQuestionIndex (-1);
				break; 
			}
		}
	}


	//During question methods



	//On Answer Submission
	public override bool answerValidated(){
		//Check for answer
		return true;
	}

	public void totalTimeFinished(){
		//GO: Total time finished
	}
	public override void Update(){

		//Debug.Log (currentTime);
		currentTime += Time.deltaTime;
		totalTime += Time.deltaTime;
		if ((quesAnsList.getMaxTotalTime() > 0)&&(quesAnsList.getMaxTotalTime() > ((int)totalTime)-1)) {
			totalTimeFinished ();
		}
		//Debug.Log ("currentTime"+currentTime+"maxCurrentTime"+maxCurrentTime);
		setProgressBar (currentTime, maxCurrentTime, questionProgressGO);
		//GO: Set totalTime remaining
	}

}
