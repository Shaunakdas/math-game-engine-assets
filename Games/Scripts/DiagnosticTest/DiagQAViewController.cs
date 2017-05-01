using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using SimpleJSON;

public class DiagQAViewController : QuesAnsViewController {
	//Display Variables
	int totalOptionCount = 4;
	float currentTime=0,totalTime=0,maxCurrentTime = 90;
	QuesAnsList quesAnsList ;
	public GameObject diagnosticTestGO;
	DiagnosticTestController diagnosticTestObject;
	DiagQANetworkController diagQANetworkObject;
	List<GameObject> ImageGOList,QuesAttemptGOList,AnsOpGOList,questionItemGOList;
	public GameObject ScrollPanelGO,QAPanelGO;

	bool answerImage;


	//Behind the scene
	UserProfile user;

	//GameObject Reference
	public GameObject testProgressGO, questionProgressGO,questionTextGO,qaPanelGO,quesAttemptPanelGO,quesAttemptGridLayoutGO,currentCounterGO,ResultBarPanelGO;

	//Prefabs
	public GameObject ansOption, imagePrefab, tickImage,questionAttemptImage,waitingPanelPf,ResultBarPF,QuestionItemPF,QuestionItemTextPF;

	//GameObject quesImageGO;
	Texture2D quesTexture;

	//Animation
	Animator anim;
	int quesStartHash, quesEndHash;
	public Animation quesStart;

	// Use this for initialization
	public override void Start () {
		quesAnsList = new QuesAnsList();
		user = new UserProfile();
		diagnosticTestObject = (DiagnosticTestController) diagnosticTestGO.GetComponent(typeof(DiagnosticTestController));
		diagQANetworkObject = (DiagQANetworkController) gameObject.GetComponent(typeof(DiagQANetworkController));
		SwipeManager.OnSwipeDetected += OnSwipeDetected;


		//Initialising animation components
		anim = qaPanelGO.GetComponent<Animator>();
		quesStartHash = Animator.StringToHash("quesStartTrigger");
		quesEndHash = Animator.StringToHash("quesEndTrigger");
		answerImage = false;
	}
	public void setQAList(string qaJSONText){
		Debug.Log ("getQAListAPI started");
		diagQANetworkObject = (DiagQANetworkController) gameObject.GetComponent(typeof(DiagQANetworkController));
		quesAnsList = new QuesAnsList();
		Debug.Log ("setQAList JSON "+qaJSONText);
		diagQANetworkObject.setQAListJSON (qaJSONText, quesAnsList);
		waitingPanelPf.transform.SetSiblingIndex (1);
		setQuesAnsBasedOnIndex (0);
		questionItemGOList = new List<GameObject> ();
	}
	public string postQAAttempt(){
		Debug.Log ("postQAAttempt started");
		diagQANetworkObject = (DiagQANetworkController) gameObject.GetComponent(typeof(DiagQANetworkController));
		return diagQANetworkObject.getQAAttemptJSON ( quesAnsList);
	}
	public QuesAnsList getQAList(){
		return quesAnsList;
	}
//	public void getQAListAPI(){
//		quesAnsList = new QuesAnsList();
//		StartCoroutine(GetQAList());
//	}
//
//	//API Calls
//	IEnumerator GetQAList() {
//		yield return StartCoroutine(diagQANetworkObject.getQAListNetworkCall(quesAnsList));
//		Debug.Log ("quesAnsList view opened");
//		setQuesAnsBasedOnIndex (0);
//		diagnosticTestObject.updateAPIStatus ("GetQAList",diagQANetworkObject.getQAListNetworkCallResponse);
//	}
//	IEnumerator postQuestionAttempt(QuesAnsList quesAnsList){
//		diagnosticTestObject.openWaitingPanel ();
//		yield return StartCoroutine(diagQANetworkObject.postQAListNetworkCall(quesAnsList));
//		diagnosticTestObject.updateAPIStatus ("PostQuestionAttempt",diagQANetworkObject.postQAAttemptNetworkCallResponse);
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
		Debug.Log ("id "+currQuesAnsPair.BackendId+" question_text "+question_text);
		question_text = StringWrapper.changeString (question_text);
		questionTextGO.GetComponent<TEXDraw>().text  =  "";
		questionTextGO.GetComponent<TEXDraw>().text  =  question_text;
		Debug.Log("Question text"+currQuesAnsPair.getQuesText ());
		Debug.Log("Url of question image"+currQuesAnsPair.getQuesImage ());
		ImageGOList = new List<GameObject> ();
		if (currQuesAnsPair.getQuesImage ().Length > 0) {
			
			StartCoroutine (LoadImage (@currQuesAnsPair.getQuesImage (), qaPanelGO));
		} else{
			

		}
	}
	//Setting Answer Views
	public  void setAnsOpView(QuesAnsPair currQuesAnsPair){
		//Changing to answerOption
		List<AnswerOption> ansOptionList = new List<AnswerOption>();
		ansOptionList = currQuesAnsPair.ansOptionList;
		//Answer answer = currQuesAnsPair.getAnswer ();
//		List<string> answerOption = ansOptionList.Select(c => c.optionText).ToList();
//		List<string> answerOptionImg = ansOptionList.Select(c => c.optionImg).ToList();
		if (ansOptionList.Count == 0) {
			changeQuestionIndex (1,-1);

		} else {
			
			AnsOpGOList = new List<GameObject> ();
			Debug.Log ("Correct Answer is "+quesAnsList.getCurrentQuesAnsPair ().ansOptionList.FindIndex(option => option.correctFlag == true));
			bool attempted = false;
			for (int j = 0; j < ansOptionList.Count; j = j + 1) {
				GameObject ansOpObject = (GameObject) Instantiate (ansOption,qaPanelGO.transform);
				ansOpObject.name = "AnsOp";
				ansOpObject.GetComponent<Button> ().interactable = false;
				//Setting Answer Option Text
				string option_text  = ansOptionList[j].optionText;
				Debug.Log("Answer Option Text is"+option_text);
				option_text = StringWrapper.changeString (option_text);

				ansOpObject.GetComponentInChildren<TEXDraw> ().text = option_text;

				//Setting Answer option Image
				if (ansOptionList[j].optionImg.Length > 0) {
					answerImage = true;
					ansOpObject.GetComponent<Image>().color = new Vector4(0.5F, 0.5F, 0.5F, 1);
					StartCoroutine (LoadImage (@ansOptionList[j].optionImg, ansOpObject));

				} else{
					answerImage = false;
				}

				//Setting Button onClickListener
				Button answerButton = ansOpObject.GetComponent<Button> ();
				int tempInt = j;
				answerButton.onClick.AddListener (() => AnswerSelected (tempInt));

				//Setting tick image if already selected

				if (j == quesAnsList.getCurrentQuesAnsPair ().ansOptionList.FindIndex(option => option.selectedFlag == true)) {
					ansOpObject.GetComponentInChildren<TEXDraw> ().size = 80;
					ansOpObject.GetComponent<Image>().color = new Vector4(0.5F, 1.0F, 0.5F, 1.0F);
//					GameObject tickGO = (GameObject)Instantiate (tickImage,ansOpObject.transform);
//					tickGO.name = "TickImg";
//					tickGO.transform.position = new Vector3(370f,0f , 0f);
					attempted = true;
					Debug.Log ("Answer selected + initiating tick"+j+attempted);
				} else {
					GameObject tick = getChildGameObject (ansOpObject,"TickImg");
					Destroy (tick);

				}
				if (!answerImage) {
//					maxTextSize = Math.Max (getTextSize (answerOption [j]), maxTextSize);
				}
				AnsOpGOList.Add (ansOpObject);
			}
			if (!attempted) {
				Debug.Log ("Correct answer selected"+attempted+" initiating tick"+currQuesAnsPair.userAttempt);
				currQuesAnsPair.userAttempt = 1;
				Debug.Log ("Correct answer selected"+attempted+" initiating tick"+currQuesAnsPair.userAttempt);
			}
		}
		anim.SetTrigger (quesStartHash);
		displayRectSizes (AnsOpGOList);
		StartCoroutine(disableInteractivity (AnsOpGOList));
	}
	IEnumerator disableInteractivity(List<GameObject> ansOpList) {
		Debug.Log("Before Waiting 2 seconds");
		yield return new WaitForSeconds(2);
		Debug.Log("After Waiting 2 Seconds");
		foreach (GameObject ansOpGO in AnsOpGOList) {
			ansOpGO.GetComponent<Button> ().interactable = true;
		}
	}
	IEnumerator LoadImage(string @Url,GameObject QAGameObject)
	{
		Debug.Log ("LoadImage Initiated");

		GameObject quesImageGO = Instantiate (imagePrefab,QAGameObject.transform ) as GameObject;
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
		RectTransform rt = quesImageGO.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(1.5f*quesTexture.width, 1.5f*quesTexture.height);

		ImageGOList.Add (quesImageGO);
	}
	//Setting question progress views
	public void setQuestionProgressView(QuesAnsPair currQuesAnsPair){
		currentTime = (float)currQuesAnsPair.getUserTimeTaken();
		Debug.Log ("Setting stored current time " + currentTime);
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
		//Animating question end
		//anim.SetTrigger (quesEndHash);
		QuestionSelected(-1);
		int solved = 0;
		Debug.Log ("Button clicked = " + buttonNo+ currentTime);
		//Selected Flag of current option set to true
		quesAnsList.getCurrentQuesAnsPair ().ansOptionList.ForEach(option => option.selectedFlag = false);
		quesAnsList.getCurrentQuesAnsPair ().ansOptionList [buttonNo].selectedFlag = true;
		//Adding selected Option to selectedOpList pf QuestionAnswerPair
		addToSelectedList(currentTime,buttonNo);
		if (buttonNo == quesAnsList.getCurrentQuesAnsPair ().ansOptionList.FindIndex(option => option.correctFlag == true)) {
			solved = 3;
		} else {
			solved = 2;
		}
		//GO: Set color of user selected option to light color
		quesAnsList.postQuestionCalculations (solved, (float)(currentTime));
		StartCoroutine(QuesEndAnimation(1,-1));
		//Starting next question
		//changeIndex(1);
	}
	public IEnumerator QuesEndAnimation(int increment,int updated)
	{
		anim.Play ("QuesEnd");

		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
		Debug.Log("This happens 2 seconds later. Tada.");
		//Starting next question
		changeQuestionIndex(increment,updated);
	}

	public void changeQuestionIndex(int increment,int updated){
		//Destroy (quesImageGO);
		base.destroyGOList(ImageGOList);
		base.destroyGOList (AnsOpGOList);
		Destroy (quesTexture);
		questionTextGO.GetComponent<TEXDraw>().text  =  "";

		GameObject[] ansOpTagged = GameObject.FindGameObjectsWithTag ("AnswerOption");
		foreach (GameObject go in ansOpTagged) { 
			Destroy (go);
		}
		quesAnsList.setUserTimeTaken ((currentTime));
		//If right swipe, left swipe or answer selection
		if (updated == -1) {
			if (increment > 0) {
				//Going to next question
				if (quesAnsList.getUserIndex () < quesAnsList.getMaxIndex () - 1) {
					int increment_index = quesAnsList.getUserIndex () + increment;
					setQuesAnsBasedOnIndex (increment_index);

				} else {
					//GO: Show end of quiz page
					openFinalScreen();
				}
			} else {
				//Going to previous question
				if (quesAnsList.getUserIndex () > 0) {
					int increment_index = quesAnsList.getUserIndex () + increment;
					Debug.Log ("Going to question of index " + increment_index);
					setQuesAnsBasedOnIndex (increment_index);
				}
			}
		}else {
			//If question selection
			setQuesAnsBasedOnIndex (updated);
		}
	}
	public void openFinalScreen(){
		Debug.Log ("End of quiz reached");
		//StartCoroutine(postQuestionAttempt (quesAnsList));
		//diagnosticTestObject.openWaitingPanel ();
		base.destroyChildGOList(ResultBarPanelGO);
		setQuesAnsReviewList(QuestionItemPF,QuestionItemTextPF,ResultBarPanelGO,questionItemGOList,quesAnsList);
		waitingPanelPf.transform.SetSiblingIndex (3);
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


	public void swipeButton(int increment){
		int updated = -1;
		QuestionSelected(quesAnsList.getUserIndex ()+increment);
	}
	void OnSwipeDetected (Swipe direction)
	{	
		int updated = -1;
		switch (direction) {
		case global::Swipe.Left:
			{
				StartCoroutine(QuesEndAnimation(1,updated));
				break; 
			}
		case global::Swipe.Right:
			{
				StartCoroutine(QuesEndAnimation(-1,updated));
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
		//Debug.Log ("Update function : currentTime"+currentTime+"maxCurrentTime"+maxCurrentTime);
		setProgressBar (currentTime, maxCurrentTime, questionProgressGO);
		//GO: Set totalTime remaining

	}
	public void addToSelectedList(float currentTime, int selectedButtonCount){
		QuesAnsPair quesAnsPair = new QuesAnsPair ();
		quesAnsPair = quesAnsList.getCurrentQuesAnsPair ();
		int selectedOpCount = quesAnsPair.selectedOpList.Count;
		if (selectedOpCount == 0) {
			quesAnsPair.ansOptionList [selectedButtonCount].timeFromLastSelection = currentTime; 
			quesAnsPair.selectedOpList.Add (quesAnsPair.ansOptionList [selectedButtonCount]);
		} else {
			quesAnsPair.ansOptionList [selectedButtonCount].timeFromLastSelection = currentTime - quesAnsPair.ansOptionList [selectedOpCount-1].timeFromLastSelection ; 
			quesAnsPair.selectedOpList.Add (quesAnsPair.ansOptionList [selectedButtonCount]);
		}
	}
	public void setQuestionAttemptBoard(){
		if(!quesAttemptPanelGO.activeSelf){
			QuesAttemptGOList = new List<GameObject> ();
			quesAttemptPanelGO.SetActive(true);
			currentCounterGO.GetComponent<Text>().text =  (quesAnsList.getUserIndex ()+1)+"/"+quesAnsList.QAList.Count;
			for (var i = 0; i < quesAnsList.QAList.Count; i++) {
				
				GameObject quesAttemptObject = (GameObject) Instantiate (questionAttemptImage,quesAttemptGridLayoutGO.transform);

				quesAttemptObject.GetComponentInChildren<Text> ().text = ""+(i+1);
				if (quesAnsList.QAList[i].userAttempt == 0) {
					//Not displayed
					quesAttemptObject.GetComponent<Image>().color = new Vector4(0.5F, 0.5F, 1.0F, 1);
				}else if(quesAnsList.QAList[i].userAttempt == 1){
					//Displayed and Skipped
					quesAttemptObject.GetComponent<Image>().color = new Vector4(0.5F, 0.5F, 1.0F, 0.5F);
				}else{
					//Attempted
					quesAttemptObject.GetComponent<Image>().color = new Vector4(0.5F, 1.0F, 0.5F, 1);
				}
				QuesAttemptGOList.Add (quesAttemptObject);
				Button answerButton = quesAttemptObject.GetComponent<Button> ();
				int tempInt = i;
				answerButton.onClick.AddListener (() => QuestionSelected (tempInt));

			}
			displayBoardSizes (QuesAttemptGOList);
		}else{
			QuestionSelected(-1);
		}


	}
	public void QuestionSelected(int quesNo)
	{
		//Animating question end
		//anim.SetTrigger (quesEndHash);
		if (quesNo > -1) {
			Debug.Log ("Question Selected");
			waitingPanelPf.transform.SetSiblingIndex (1);
			int solved = quesAnsList.getCurrentQuesAnsPair().userAttempt;
			Debug.Log ("Solved status of earlier question" + solved);
			//GO: Set color of user selected option to light color
			if (quesAnsList.postQuestionCalculations (solved, (float)(currentTime))) {
				StartCoroutine (QuesEndAnimation (0, quesNo));
			}

		}
		if (quesAttemptPanelGO.activeSelf) {
			quesAttemptPanelGO.SetActive (false);
			foreach (GameObject imageObject in QuesAttemptGOList) {
				Destroy (imageObject);
			}
			QuesAttemptGOList.Clear ();
		}
	}
	void closeQuesAttemptBoard(){
	}
	void displayRectSizes(List<GameObject> AnsOpGOList){
		foreach (GameObject ansOpGO in AnsOpGOList) {
			//If option text is small
			if((getTextSize (ansOpGO.GetComponentInChildren<TEXDraw> ().text) < 200f)&&(ansOpGO.GetComponentInChildren<TEXDraw> ().text.Length <25)){
				ansOpGO.GetComponent<LayoutElement> ().preferredWidth = 4*getTextSize (ansOpGO.GetComponentInChildren<TEXDraw> ().text)+100f;
				getChildGameObject(ansOpGO,"AnsText").GetComponent<LayoutElement>().preferredWidth = 4*getTextSize (ansOpGO.GetComponentInChildren<TEXDraw> ().text)+100f;
			}
			//If option text height is more than 240f
			Debug.Log("Height AnsText is "+getChildGameObject (ansOpGO, "AnsText").GetComponent<RectTransform> ().rect.height);
			if (getChildGameObject (ansOpGO, "AnsText").GetComponent<RectTransform> ().rect.height > 210f) {
				ansOpGO.GetComponent<LayoutElement> ().preferredHeight = getChildGameObject (ansOpGO, "AnsText").GetComponent<RectTransform> ().rect.height + 30f;
			}
			//Setting according to screen size
			ansOpGO.GetComponent<LayoutElement> ().preferredHeight = ScreenManager.scaledYSize (ansOpGO.GetComponent<LayoutElement> ().preferredHeight);
			ansOpGO.GetComponent<LayoutElement> ().preferredWidth = ScreenManager.scaledXSize (ansOpGO.GetComponent<LayoutElement> ().preferredWidth);
			ansOpGO.GetComponentInChildren<TEXDraw> ().size = ScreenManager.scaledXSize (ansOpGO.GetComponentInChildren<TEXDraw> ().size);
		}
	}
	void displayBoardSizes(List<GameObject> GOList){
		foreach (GameObject quesAttemptObject in GOList) {
			quesAttemptObject.GetComponentInChildren<Text> ().fontSize = (int)ScreenManager.scaledXSize ((float)quesAttemptObject.GetComponentInChildren<Text> ().fontSize);
		}
		var cellSize = quesAttemptGridLayoutGO.GetComponent<GridLayoutGroup>().cellSize;
		quesAttemptGridLayoutGO.GetComponent<GridLayoutGroup>().cellSize = new Vector2(ScreenManager.scaledXSize(cellSize.x),ScreenManager.scaledYSize(cellSize.y));

	}

	public void setQuesAnsReviewList(GameObject QuestionItemPF, GameObject QuestionItemTextPF,GameObject ResultBarPanelGO, List<GameObject> questionItemGOList,QuesAnsList qaList){
		base.destroyGOList(questionItemGOList);
		base.destroyChildGOList (ResultBarPanelGO);
		Debug.Log ("Size of qaList" + qaList.QAList.Count);

//		string questionListHeader = "";
//		GameObject questionListHeaderGO = (GameObject)Instantiate (QuestionItemPF, ResultBarPanelGO.transform);
//		getChildGameObject (questionListHeaderGO, "QuestionItemText").GetComponent<TEXDraw> ().text = "\\opens[b]{" + questionListHeader + "}";
//		questionItemGOList.Add (questionListHeaderGO);
		for (int j = 0; j < qaList.QAList.Count; j = j + 1) {
			int attempt = qaList.QAList [j].userAttempt;
			QuesAnsPair qaPair = new QuesAnsPair ();
			qaPair = qaList.QAList [j];

			Debug.Log ("Question " + (j + 1) + ": " + qaPair.getQuesText ());
			GameObject reviewQuestionItemGO = (GameObject)Instantiate (QuestionItemPF, ResultBarPanelGO.transform);
			GameObject questionItemTextGO = getChildGameObject (reviewQuestionItemGO, "QuestionItemText");
			questionItemTextGO.GetComponent<TEXDraw> ().text = "\\opens[b]{Question " + (j + 1) + ":}" + StringWrapper.changeString (qaPair.getQuesText ());

			GameObject selectedAnswerGO = (GameObject)Instantiate (QuestionItemTextPF, reviewQuestionItemGO.transform);
			if (attempt == 1) {
				Debug.Log ("Answer Skipped ");
				selectedAnswerGO.GetComponent<TEXDraw> ().text = "\\opens[b]{Answer Skipped} ";
			} else if (attempt == 0) {
				Debug.Log ("Answer Not viewed ");
				selectedAnswerGO.GetComponent<TEXDraw> ().text = "\\opens[b]{Answer not viewed} ";
			} else {
				Debug.Log ("Answer Selected " + qaPair.ansOptionList.Find (option => option.selectedFlag == true).optionText);
				selectedAnswerGO.GetComponent<TEXDraw> ().text = "\\opens[b]{You answered: }" + StringWrapper.changeString (qaPair.ansOptionList.Find (option => option.selectedFlag == true).optionText);
			}
			Debug.Log (selectedAnswerGO.GetComponent<RectTransform> ().rect.height);
			int index = j;
			reviewQuestionItemGO.AddComponent<Button> ();
			reviewQuestionItemGO.GetComponent<Button> ().onClick.AddListener (() => QuestionSelected (index));
			questionItemGOList.Add (reviewQuestionItemGO);
		}
	}
}
