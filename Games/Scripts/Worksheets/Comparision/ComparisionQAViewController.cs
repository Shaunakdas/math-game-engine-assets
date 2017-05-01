using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using SimpleJSON;

public class ComparisionQAViewController : QuesAnsViewController {
	//Display Variables
	int totalOptionCount = 4;
	float currentTime=0,totalTime=0,maxCurrentTime = 90;
	QuesAnsList quesAnsList ;
	ComparisionQANetworkController commonQANetworkObject;
	List<GameObject> ImageGOList,AnsOpGOList;
	List<int> correctOrderList;
	//Behind the scene

	//GameObject Reference
	public GameObject questionTextGO,qaPanelGO;

	//Prefabs
	public GameObject ansOption, imagePrefab;

	//Game specific GameObject
	public GameObject submitBtnGO, continueBtnGO;

	//GameObject textureList to hold reference to all textures downloaded
	List<Texture2D> textureList;

	// Use this for initialization
	public override void Start () {
		quesAnsList = new QuesAnsList();
		textureList = new List<Texture2D> ();
		correctOrderList = new List<int> ();
		setQAList ();
	}
	public override void setQAList(){
		Debug.Log ("getQAListAPI started");
		commonQANetworkObject = (ComparisionQANetworkController) gameObject.GetComponent(typeof(ComparisionQANetworkController));
		quesAnsList = new QuesAnsList();
		commonQANetworkObject.setQAListJSON (quesAnsList);

	}
	public override void getQAListCallFinished(){
		//Get QA List API finished. Now Display work can start.
//		setQuesAnsBasedOnIndex (0);
	}
	public override string postQAAttempt(){
		Debug.Log ("postQAAttempt started");
		commonQANetworkObject = (ComparisionQANetworkController) gameObject.GetComponent(typeof(ComparisionQANetworkController));

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
		entryAnim();
	}

	//Setting Question Views
	public override  void setQuesView(QuesAnsPair currQuesAnsPair){
		//Setting Question Text
		questionTextGO.GetComponent<TEXDrawNGUI>().text  =  base.getQuestionText(currQuesAnsPair);
		ImageGOList = new List<GameObject> ();
		//Setting Question Image
		if (currQuesAnsPair.getQuesImage ().Length > 0) {
			StartCoroutine (LoadImage (@currQuesAnsPair.getQuesImage (), qaPanelGO));
		} 
		questionEntryAnim (questionTextGO);
		submitEntryAnim (submitBtnGO);
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
				UIButton answerButton = submitBtnGO.GetComponent<UIButton> ();
				EventDelegate.Set(answerButton.onClick, delegate() { AnswerSelected(); });

				//Keeping reference to current ansOpObject
				AnsOpGOList.Add (ansOpObject);
				correctOrderList.Add (ansOptionList [j].correctOrder);
				answerOptionEntryAnim (ansOpObject);
			}
		}
	}
	public int getAnsOpOrder(GameObject AnsOp){
		return AnsOp.transform.GetSiblingIndex ();
	}
	public void setInputOrder(GameObject AnsOp, int inputOrder){
		getQAList ().getCurrentQuesAnsPair ().ansOptionList [AnsOpGOList.IndexOf (AnsOp)].inputOrder = inputOrder;
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
		//Pending Question animation to come down
		submitEntryAnim(submitBtnGO);
	}
	public void submitEntryAnim(GameObject submitBtnGO){
		//Pending Submit Button animation to come up
	}
	public void submitExitAnim(){
		//Pending Submit Button animation to go down
	}
	public void questionExitAnim(GameObject questionGO){
		//Pending Question animation to go up
	}
	public void answerOptionEntryAnim(GameObject ansOpGO){
		//Pending Answer option animation to come to start index
	}
	public void answerOptionExitAnim(GameObject ansOpGO){
		//Pending Answer option animation to come to correct index
	}
	public override void correctAnsAnim(){
		//For correct answer animation
	}
	public override void incorrectAnsAnim(){
		//For incorrect animation
		//Pending incorrect answer animation 
		submitExitAnim();
		continueEntryAnim(continueBtnGO);
	}
	public void continueEntryAnim(GameObject continueBtnGO){
		//Pending Continue Button animation to come up
	}
	public void continueExitAnim(){
		//Pending Continue Button animation to go down
	}


	//On Selection of answer
	public void AnswerSelected()
	{
		submitExitAnim ();
		foreach (GameObject ansOp in AnsOpGOList) {
			setInputOrder (ansOp,getAnsOpOrder(ansOp));
		}
		quesAnsList.postQuestionCalculations (getSolutionFlag(), (float)(currentTime));
		if (getSolutionFlag () == 3)
			correctAnsAnim ();
		else 
			incorrectAnsAnim ();
		//changeIndex(1);
		changeQuestionIndex(1,0);
	}
	public override bool answerValidated(){
		List<AnswerOption> ansOptionList = getQAList ().getCurrentQuesAnsPair ().ansOptionList;
		bool order = false;
		for (int j = 0; j < ansOptionList.Count; j = j + 1) {
			if (ansOptionList[j].correctOrder != getAnsOpOrder(AnsOpGOList[j]))
				return false;
		}
		return order;
	}
	public int getSolutionFlag(){
		if (answerValidated()) {
			return 3;
		} else {
			return 2;
		}
	}
	public void setListCorrectOrder(){
		List<int> sortCorrectOrderList = correctOrderList;
		List<AnswerOption> ansOptionList = getQAList ().getCurrentQuesAnsPair ().ansOptionList;
		sortCorrectOrderList.Sort ();
		foreach (int correctOrder in sortCorrectOrderList) {
			GameObject ansOpGO = AnsOpGOList[correctOrderList.IndexOf(correctOrder)];
			setCorrectOrder (ansOpGO, correctOrder);
		}
	}
	public void setCorrectOrder(GameObject AnsOp, int correcOrder){
		//Pending animation to animate ansOpGO to position correctOrder
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
