using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using SimpleJSON;

public class CommonQAViewController : QuesAnsViewController {
	//Display Variables
	int totalOptionCount = 4;
	float currentTime=0,totalTime=0,maxCurrentTime = 90;
	QuesAnsList quesAnsList ;
	public GameObject commonTestGO;
	CommonTestController commonTestObject;
	CommonQANetworkController commonQANetworkObject;
	List<GameObject> ImageGOList,QuesAttemptGOList,AnsOpGOList,questionItemGOList;
	public GameObject ScrollPanelGO,QAPanelGO;

	bool answerImage;

	//Behind the scene
	UserProfile user;

	//GameObject Reference
	public GameObject testProgressGO, questionProgressGO,questionTextGO,qaPanelGO,quesAttemptPanelGO,quesAttemptGridLayoutGO,currentCounterGO,ResultBarPanelGO;

	//Prefabs
	public GameObject ansOption, imagePrefab, tickImage,questionAttemptImage;

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
		commonTestObject = (CommonTestController) gameObject.GetComponent(typeof(CommonTestController));
		commonQANetworkObject = (CommonQANetworkController) gameObject.GetComponent(typeof(CommonQANetworkController));


		//Initialising animation components
//		anim = qaPanelGO.GetComponent<Animator>();
//		quesStartHash = Animator.StringToHash("quesStartTrigger");
//		quesEndHash = Animator.StringToHash("quesEndTrigger");
		answerImage = false;
	}
	public override void setQAList(string qaJSONText){
		Debug.Log ("getQAListAPI started");
		commonQANetworkObject = (CommonQANetworkController) gameObject.GetComponent(typeof(CommonQANetworkController));
		quesAnsList = new QuesAnsList();
		Debug.Log ("setQAList JSON "+qaJSONText);
		commonQANetworkObject.setQAListJSON (qaJSONText, quesAnsList);
//		waitingPanelPf.transform.SetSiblingIndex (1);
		//Uncomment this
		setQuesAnsBasedOnIndex (0);
		questionItemGOList = new List<GameObject> ();
	}
	public string postQAAttempt(){
		Debug.Log ("postQAAttempt started");
		commonQANetworkObject = (CommonQANetworkController) gameObject.GetComponent(typeof(CommonQANetworkController));
		return commonQANetworkObject.getQAAttemptJSON ( quesAnsList);
	}
	public QuesAnsList getQAList(){
		return quesAnsList;
	}

	//Setting up views
	public override void setQuesAnsBasedOnIndex(int index){
		quesAnsList.setUserIndex(index);
		QuesAnsPair currQuesAnsPair = quesAnsList.getCurrentQuesAnsPair ();
		setQuesView (currQuesAnsPair);
		setAnsOpView (currQuesAnsPair);
//		setTestProgressView();
//		setQuestionProgressView (currQuesAnsPair);
		entryAnim();
	}

	//Setting Question Views
	public  void setQuesView(QuesAnsPair currQuesAnsPair){
		//For setting getCurrentQuesAnsPair.getQuesText ()on view 
		//D var quesText = GameObject.Find("QuesText");
		string question_text = currQuesAnsPair.getQuesText ();
		Debug.Log ("id "+currQuesAnsPair.BackendId+" question_text "+question_text);
		if (question_text.Length>0)
			question_text = StringWrapper.changeString (question_text);
		questionTextGO.GetComponent<TEXDrawNGUI>().text  =  "";
		questionTextGO.GetComponent<TEXDrawNGUI>().text  =  question_text;
		Debug.Log("Question text"+currQuesAnsPair.getQuesText ());
		Debug.Log("Url of question image"+currQuesAnsPair.getQuesImage ());
		ImageGOList = new List<GameObject> ();
		if (currQuesAnsPair.getQuesImage ().Length > 0) {

//			StartCoroutine (LoadImage (@currQuesAnsPair.getQuesImage (), qaPanelGO));
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
				GameObject ansOpObject = (GameObject) InstantiateUnityGO (ansOption,qaPanelGO.transform);

				ansOpObject.name = "AnsOp";
				//Setting Answer Option Text
				string option_text  = ansOptionList[j].optionText;
				Debug.Log("Answer Option Text is"+option_text);
				option_text = StringWrapper.changeString (option_text);

				ansOpObject.GetComponentInChildren<TEXDrawNGUI> ().text = option_text;

				//Setting Answer option Image
				if (ansOptionList[j].optionImg.Length > 0) {
					answerImage = true;
					ansOpObject.GetComponent<Image>().color = new Vector4(0.5F, 0.5F, 0.5F, 1);
					StartCoroutine (LoadImage (@ansOptionList[j].optionImg, ansOpObject));

				} else{
					answerImage = false;
				}

				//Setting Button onClickListener
				UIButton answerButton = ansOpObject.GetComponent<UIButton> ();
				int tempInt = j;
				EventDelegate.Set(answerButton.onClick, delegate() { AnswerSelected(tempInt); });

				//Setting tick image if already selected

//					GameObject tick = getChildGameObject (ansOpObject,"TickImg");
//					Destroy (tick);
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
//		anim.SetTrigger (quesStartHash);
//		StartCoroutine(disableInteractivity (AnsOpGOList));
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

		GameObject quesImageGO = InstantiateUnityGO (imagePrefab,QAGameObject.transform ) as GameObject;
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
	public override void AnswerSelected(int buttonNo)
	{
		//Animating question end
		//anim.SetTrigger (quesEndHash);
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
		QuesEndAnimation(1,-1);
		//Starting next question
		//changeIndex(1);
	}
	public override void QuesEndAnimation(int increment,int updated)
	{
//		anim.Play ("QuesEnd");
//
//		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);s
		Debug.Log("This happens 2 seconds later. Tada.");
		//Starting next question
		changeQuestionIndex(increment,updated);
	}

	public override void changeQuestionIndex(int increment,int updated){
		//Destroy (quesImageGO);
		base.destroyGOList(ImageGOList);
		base.destroyGOList (AnsOpGOList);
		Destroy (quesTexture);
		questionTextGO.GetComponent<TEXDrawNGUI>().text  =  "";

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
		//commonTestObject.openWaitingPanel ();
		base.destroyChildGOList(ResultBarPanelGO);
//		waitingPanelPf.transform.SetSiblingIndex (3);
	}
	//Helper Methods
	public void setProgressBar(float current, float total, GameObject progressGO){
		//D GameObject progressGO = GameObject.Find (gameObjectName);
		if (progressGO != null) {

			Image img = progressGO.GetComponent<Image> ();
			img.fillAmount = (float)(current / total);
		}
	}

	//During question methods



	//On Answer Submission
	public override bool answerValidated(QuesAnsList currQuesAnsList,int answerIndex){
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

				GameObject quesAttemptObject = (GameObject) InstantiateUnityGO (questionAttemptImage,quesAttemptGridLayoutGO.transform);

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

			}
		}else{
		}


	}

	void closeQuesAttemptBoard(){
	}
}
