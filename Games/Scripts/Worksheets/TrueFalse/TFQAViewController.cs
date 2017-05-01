using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using SimpleJSON;

public class TFQAViewController : QuesAnsViewController {
	//Display Variables
	int totalOptionCount = 4;
	float currentTime=0,totalTime=0,maxCurrentTime = 90;
	QuesAnsList quesAnsList ;
	TFQANetworkController commonQANetworkObject;
	List<GameObject> ImageGOList,AnsOpGOList;

	//Behind the scene

	//Generic GameObject Reference
	public GameObject questionTextGO,qaPanelGO;

	//Generic Prefabs
	public GameObject ansOption, imagePrefab, trueOptionPF, falseOptionPF;

	//Game specific GameObject
	public GameObject questionGridGO;

	//Game specific Prefabs
	public GameObject questionTextPF;

	//GameObject textureList to hold reference to all textures downloaded
	List<Texture2D> textureList;

	// Use this for initialization
	public override void Start () {
		quesAnsList = new QuesAnsList();
		textureList = new List<Texture2D> ();
		setQAList ();
	}
	public override void setQAList(){
		Debug.Log ("getQAListAPI started");
		commonQANetworkObject = (TFQANetworkController) gameObject.GetComponent(typeof(TFQANetworkController));
		quesAnsList = new QuesAnsList();
		commonQANetworkObject.setQAListJSON (quesAnsList);

	}
	public override void getQAListCallFinished(){
		//Get QA List API finished. Now Display work can start.
		entryAnim();
		setQuesAnsBasedOnIndex (0);
	}
	public override string postQAAttempt(){
		Debug.Log ("postQAAttempt started");
		commonQANetworkObject = (TFQANetworkController) gameObject.GetComponent(typeof(TFQANetworkController));

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
		if (index==1)
			setAnsOpView (currQuesAnsPair); 

	}

	//Setting Question Views
	public override  void setQuesView(QuesAnsPair currQuesAnsPair){
		//Setting Question Text
		GameObject questionGO = InstantiateNGUIGO(questionTextPF,questionGridGO.transform);
		questionGO.GetComponentInChildren<TEXDrawNGUI>().text  =  base.getQuestionText(currQuesAnsPair);

		ImageGOList = new List<GameObject> ();
		//Setting Question Image
		if (currQuesAnsPair.getQuesImage ().Length > 0) {
			StartCoroutine (LoadImage (@currQuesAnsPair.getQuesImage (), qaPanelGO));
		} 
		questionEntryAnim (questionGO);
	}
	//Setting Answer Views
	public override  void setAnsOpView(QuesAnsPair currQuesAnsPair){
		//Changing to answerOption
		List<AnswerOption> ansOptionList = currQuesAnsPair.ansOptionList;
		if (ansOptionList.Count == 0) {
			changeQuestionIndex (1,-1);
		} else {
			AnsOpGOList = new List<GameObject> ();
			for (int j = 0; j < ansOptionList.Count; j = j + 1) {
				GameObject ansOpObject = (GameObject) InstantiateNGUIGO (ansOption,qaPanelGO.transform,"AnsOp");
				//Setting Answer Option Text
				ansOpObject.GetComponentInChildren<TEXDrawNGUI> ().text = base.getAnswerOptionText(currQuesAnsPair,j);

				//Setting Answer option Image
				if (ansOptionList[j].optionImg.Length > 0) {
					ansOpObject.GetComponent<Image>().color = new Vector4(0.5F, 0.5F, 0.5F, 1);
					StartCoroutine (LoadImage (base.getAnswerOptionImageUrl(currQuesAnsPair,j), ansOpObject));
				}

				//Setting Button onClickListener
				UIButton answerButton = ansOpObject.GetComponent<UIButton> ();
				//Pending UIButton colour of pressed state to change based on its correctFlag
				int tempInt = j;
				EventDelegate.Set(answerButton.onClick, delegate() { AnswerSelected(tempInt); });

				//Keeping reference to current ansOpObject
				AnsOpGOList.Add (ansOpObject);
			}
			answerOptionEntryAnim ();
		}
	}
	IEnumerator LoadImage(string @Url,GameObject QAGameObject)
	{
		Debug.Log ("LoadImage Initiated");
		GameObject itemImageGO = InstantiateNGUIGO (imagePrefab,QAGameObject.transform ) as GameObject;
		itemImageGO.transform.SetSiblingIndex (0);
		//Calling url
		WWW www = new WWW(Url);
		yield return www;
		Debug.Log ("Loaded"+Url);
		textureList.Add (www.texture);
		//Setting image Gameobjct with downloaded texture
		base.setImageTexture (itemImageGO, www.texture);
		//Adding new generated image inside 
		ImageGOList.Add (itemImageGO);
	}
	//Animation methods
	public override void entryAnim(){
		//For entry animation
	}
	public override void exitAnim(){
		//For exit animation
	}
	public void questionEntryAnim(GameObject questionGO){
		//For question entry animation
		questionGridGO.GetComponent<UICenterOnChild> ().CenterOn(questionGO.transform);
	}
	public override void questionExitAnim(){
		//For exit animation
	}
	public override void answerOptionEntryAnim(){
		//For entry animation
	}
	public override void answerOptionExitAnim(){
		//For exit animation
	}
	public override void correctAnsAnim(){
		//For correct answer animation
		//Pending
		GameObject currQuestionGO = questionGridGO.GetComponent<UICenterOnChild> ().centeredObject;
		//Pending currQuestionGO.tween 
	}
	public override void incorrectAnsAnim(){
		//For incorrect animation
		//Pending
		GameObject currQuestionGO = questionGridGO.GetComponent<UICenterOnChild> ().centeredObject;
		//Pending currQuestionGO.tween 
		//Pending Solution Display
	}


	//On Selection of answer
	public override void AnswerSelected(int buttonNo)
	{
		Debug.Log ("Button clicked = " + buttonNo+ currentTime);
		if (base.getSolutionFlag (quesAnsList, buttonNo) == 3)
			correctAnsAnim ();
		else 
			incorrectAnsAnim ();
		//Selected Flag of current option set to true
		base.setSelectionflag(quesAnsList,buttonNo);
		//GO: Set color of user selected option to light color
		quesAnsList.postQuestionCalculations (base.getSolutionFlag(quesAnsList,buttonNo), (float)(currentTime));
		questionExitAnim ();
		//Starting next question
		//changeIndex(1);
		changeQuestionIndex(1,0);
	}

	public override void changeQuestionIndex(int increment,int updated){
		//Destroy (quesImageGO);
		base.destroyGOList(ImageGOList);
		base.destroyGOList (AnsOpGOList);
		textureList.ForEach (itemTexture => Destroy (itemTexture));
		quesAnsList.setUserTimeTaken (currentTime);
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
	}


	//During question methods
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

	}
}
