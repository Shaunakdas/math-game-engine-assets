using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using SimpleJSON;

public class NameRecallQAViewController : QuesAnsViewController {
	//Display Variables
	int totalOptionCount = 4;
	float currentTime=0,totalTime=0,maxCurrentTime = 90;
	QuesAnsList quesAnsList ;
	NameRecallQANetworkController commonQANetworkObject;
	List<GameObject> ImageGOList,AnsOpGOList;

	//Behind the scene

	//GameObject Reference
	public GameObject questionTextGO,answerGridGO;

	//Prefabs
	public GameObject ansOption, imagePrefab;

	//GameObject textureList to hold reference to all textures downloaded
	List<Texture2D> textureList;


	//Animation Sprite
	public GameObject correctSprite,incorrectSprite;

	// Use this for initialization
	public override void Start () {
		quesAnsList = new QuesAnsList();
		textureList = new List<Texture2D> ();
		setQAList ();

	}
	public override void setQAList(){
		Debug.Log ("getQAListAPI started");
		commonQANetworkObject = (NameRecallQANetworkController) gameObject.GetComponent(typeof(NameRecallQANetworkController));
		quesAnsList = new QuesAnsList();
		commonQANetworkObject.setQAListJSON (quesAnsList);

	}
	public override void getQAListCallFinished(){
		//Get QA List API finished. Now Display work can start.
		setQuesAnsBasedOnIndex (0);
	}
	public override string postQAAttempt(){
		Debug.Log ("postQAAttempt started");
		commonQANetworkObject = (NameRecallQANetworkController) gameObject.GetComponent(typeof(NameRecallQANetworkController));

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
		questionTextGO.GetComponentInChildren<TEXDrawNGUI>().text  =  base.getQuestionText(currQuesAnsPair);
		ImageGOList = new List<GameObject> ();
		//Setting Question Image
		if (currQuesAnsPair.getQuesImage ().Length > 0) {
			StartCoroutine (LoadImage (@currQuesAnsPair.getQuesImage (), answerGridGO));
		} 
		EventDelegate.Set(questionTextGO.GetComponent<TweenColor>().onFinished, delegate() { changeQuestionIndex(1,-1);; });
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
				GameObject ansOpObject = (GameObject) InstantiateNGUIGO (ansOption,answerGridGO.transform,"AnsOp");
				//Setting Answer Option Text
				ansOpObject.GetComponentInChildren<TEXDrawNGUI> ().text = base.getAnswerOptionText(currQuesAnsPair,j);

				//Setting Answer option Image
				if (ansOptionList[j].optionImg.Length > 0) {
					ansOpObject.GetComponent<Image>().color = new Vector4(0.5F, 0.5F, 0.5F, 1);
					StartCoroutine (LoadImage (base.getAnswerOptionImageUrl(currQuesAnsPair,j), ansOpObject));
				}

				NameRecallDragDropItem dropItem = ansOpObject.GetComponent<NameRecallDragDropItem> ();
				dropItem.answerOpIndex = j;
				dropItem.questionContainer = questionTextGO;
				dropItem.rootGO = gameObject;
				//Keeping reference to current ansOpObject
				AnsOpGOList.Add (ansOpObject);
			}
		}
		answerGridGO.GetComponent<UIGrid> ().Reposition ();
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
	public override void correctAnsAnim(){
		//For correct answer animation
		correctSprite.SetActive(true);
		questionTextGO.GetComponent<TweenColor>().Play();

	}
	public override void incorrectAnsAnim(){
		//For incorrect animation
		incorrectSprite.SetActive(true);
		questionTextGO.GetComponent<TweenColor>().to = new Color(1f,0f,0f);
		questionTextGO.GetComponent<TweenColor>().Play();
	}


	//On Selection of answer
	public override void AnswerSelected(int buttonNo)
	{
		if (base.getSolutionFlag (quesAnsList, buttonNo) == 3)
			correctAnsAnim ();
		else 
			incorrectAnsAnim ();
		//Selected Flag of current option set to true
		base.setSelectionflag(quesAnsList,buttonNo);
		//GO: Set color of user selected option to light color
		quesAnsList.postQuestionCalculations (base.getSolutionFlag(quesAnsList,buttonNo), (float)(currentTime));
		//Starting next question
		//changeIndex(1);

	}

	public override void changeQuestionIndex(int increment,int updated){
		//Destroy (quesImageGO);
		EventDelegate.Remove(questionTextGO.GetComponent<TweenColor>().onFinished, delegate() { changeQuestionIndex(1,-1);; });
		base.destroyGOList(ImageGOList);
		base.destroyGOList (AnsOpGOList);
		correctSprite.SetActive(false);incorrectSprite.SetActive(false);
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
