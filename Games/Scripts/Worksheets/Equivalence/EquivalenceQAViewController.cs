using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using SimpleJSON;

public class EquivalenceQAViewController : QuesAnsViewController {
	//Display Variables
	int totalOptionCount = 4;
	float currentTime=0,totalTime=0,maxCurrentTime = 90;
	QuesAnsList quesAnsList ;
	EquivalenceQANetworkController commonQANetworkObject;
	List<GameObject> ImageGOList,AnsOpGOList;

	//Behind the scene

	//GameObject Reference
	public GameObject questionTextGO;

	//Prefabs
	public GameObject ansOption, imagePrefab;

	//Game specific GameObject
	public GameObject questionStartPanelGO,questionStartTitleGO,questionStartHintGO;

	//Game specific Prefabs
	public GameObject questionTextPF;

	//GameObject textureList to hold reference to all textures downloaded
	List<Texture2D> textureList;

	// Use this for initialization
	public override void Start () {
		quesAnsList = new QuesAnsList();
		textureList = new List<Texture2D> ();
		setQAList ();

		AnsOpGOList = new List<GameObject> ();
	}
	public override void setQAList(){
		Debug.Log ("getQAListAPI started");
		commonQANetworkObject = (EquivalenceQANetworkController) gameObject.GetComponent(typeof(EquivalenceQANetworkController));
		quesAnsList = new QuesAnsList();
		commonQANetworkObject.setQAListJSON (quesAnsList);

	}
	public override void getQAListCallFinished(){
		//Get QA List API finished. Now Display work can start.
		setQuesAnsBasedOnIndex (0);
	}
	public override string postQAAttempt(){
		Debug.Log ("postQAAttempt started");
		commonQANetworkObject = (EquivalenceQANetworkController) gameObject.GetComponent(typeof(EquivalenceQANetworkController));

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

		entryAnim();
	}

	//Setting Question Views
	public override  void setQuesView(QuesAnsPair currQuesAnsPair){
		//Setting Question Text
		questionTextGO.GetComponent<TEXDrawNGUI>().text  =  base.getQuestionText(currQuesAnsPair);
		ImageGOList = new List<GameObject> ();
		//Setting Question Image
		if (currQuesAnsPair.getQuesImage ().Length > 0) {
			StartCoroutine (LoadImage (@currQuesAnsPair.getQuesImage (), gameObject));
		} 
//		setQuestionPanel (currQuesAnsPair);
		setAnswerPanel();
	}
	public void setQuestionPanel(QuesAnsPair currQuesAnsPair){
		//First screen when user comes in
		questionStartTitleGO.GetComponent<TEXDrawNGUI> ().text = base.getQuestionText (currQuesAnsPair);
		questionStartHintGO.GetComponent<TEXDrawNGUI> ().text = currQuesAnsPair.hint_text;
	}
	public void popQuestionPanel(){
		//Pending popping animation
		//Pending questionStartTitleGO enlarge animation
		//Pending questionStart animating to transparent

	}
	public void setAnswerPanel(){
		setAnsOpView (getQAList().getCurrentQuesAnsPair());
	}
	//Setting Answer Views
	public override  void setAnsOpView(QuesAnsPair currQuesAnsPair){
		//Changing to answerOption
		List<AnswerOption> ansOptionList = currQuesAnsPair.ansOptionList;
		if (ansOptionList.Count == 0) {
			changeQuestionIndex (1,-1);
		} else {
			initAnswerOption(ansOptionList,currQuesAnsPair,0);
		}
	}
	public void initAnswerOption(List<AnswerOption> ansOptionList,QuesAnsPair currQuesAnsPair,int index){
		GameObject ansOpObject = (GameObject) InstantiateNGUIGO (ansOption,gameObject.transform,"AnsOp");
		//Setting Answer Option Text
		ansOpObject.GetComponentInChildren<TEXDrawNGUI> ().text = base.getAnswerOptionText(currQuesAnsPair,index);

		//Setting Answer option Image
		if (ansOptionList[index].optionImg.Length > 0) {
			ansOpObject.GetComponent<Image>().color = new Vector4(0.5F, 0.5F, 0.5F, 1);
			StartCoroutine (LoadImage (base.getAnswerOptionImageUrl(currQuesAnsPair,index), ansOpObject));
		}
//		Debug.Log(ansOpObject.)
		//Setting Button onClickListener
		UIButton answerButton = ansOpObject.GetComponent<UIButton> ();
		int tempInt = index;
		EventDelegate.Set(answerButton.onClick, delegate() { AnswerSelected(tempInt); });

		//Keeping reference to current ansOpObject
		AnsOpGOList.Add (ansOpObject);
		Debug.Log("AnsOpGOList.Count"+AnsOpGOList.Count);
		//Answer Animation
		answerOptionEntryAnim(ansOpObject,ansOptionList,currQuesAnsPair,index);
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
	public override void questionEntryAnim(){
		//For question entry animation
	}
	public override void questionExitAnim(){
		//For exit animation
	}
	public  void answerOptionEntryAnim(GameObject answerOpObject,List<AnswerOption> ansOptionList,QuesAnsPair currQuesAnsPair,int index){
		//For entry animation
		//Pending coming up animation answerOpObject
		float startPosition = UnityEngine.Random.Range(0,640) - 320;

		answerOpObject.GetComponent<TweenPosition>().to.x = startPosition;
		answerOpObject.GetComponent<TweenPosition>().from.x = startPosition;
		answerOpObject.GetComponent<TweenPosition> ().Play ();
		StartCoroutine (WaitMethod(ansOptionList,currQuesAnsPair,index));
	}
	IEnumerator WaitMethod(List<AnswerOption> ansOptionList,QuesAnsPair currQuesAnsPair,int index) {
		Debug.Log("Before Waiting some seconds");
		int wait = UnityEngine.Random.Range(0,5) + 5;
		yield return new WaitForSeconds(wait);
		Debug.Log("After Waiting some Seconds");
		initAnswerOption (ansOptionList, currQuesAnsPair, index + 1);
	}
	public override void answerOptionExitAnim(){
		//For exit animation
	}
	public  void correctAnsAnim(GameObject ansOpObject){
		//For correct answer animation
		//Pending correct animation on ansOpObject
		Debug.Log("Correct Animation"+ansOpObject.gameObject.name);
		TweenHeight height = ansOpObject.GetComponent<TweenHeight>();
		TweenWidth width = ansOpObject.GetComponent<TweenWidth>();
		height.to = 10*height.to; height.duration = 3; height.style = UITweener.Style.Once; height.PlayForward ();
		width.to = 10*width.to; width.duration = 3; width.style = UITweener.Style.Once; width.PlayForward ();
		EventDelegate.Set(height.onFinished, delegate() { deactivateAnswerOption(ansOpObject); });
	}
	public  void incorrectAnsAnim(GameObject ansOpObject){
		//For incorrect animation
		//Pending incorrect animation on ansOpObject
		Debug.Log("Incorrect Animation"+ansOpObject.gameObject.name);
		TweenHeight height = ansOpObject.GetComponent<TweenHeight>();
		TweenWidth width = ansOpObject.GetComponent<TweenWidth>();
		TweenPosition position = ansOpObject.GetComponent<TweenPosition> ();
		height.to = height.to/10; height.duration = 5; height.style = UITweener.Style.Once; height.PlayForward ();
		width.to = width.to/10; width.duration = 5; width.style = UITweener.Style.Once; width.PlayForward ();
		position.to.y = -560f;position.from.y =ansOpObject.transform.localPosition.y; position.duration = 5; position.PlayForward (); 
		EventDelegate.Set(height.onFinished, delegate() { deactivateAnswerOption(ansOpObject); });
	}
	public void deactivateAnswerOption(GameObject ansOp){
		ansOp.GetComponent<UI2DSprite> ().enabled = false;
		destroyChildGOList(ansOp);

	}

	//On Selection of answer
	public override void AnswerSelected(int buttonNo)
	{
		Debug.Log ("Button clicked = " + buttonNo+ base.getSolutionFlag (quesAnsList, buttonNo));
		//Selected Flag of current option set to true
		if (base.getSolutionFlag (quesAnsList, buttonNo) == 3) {
			Debug.Log ("AnsOpGOList[buttonNo]" + AnsOpGOList [buttonNo].GetComponentInChildren<TEXDrawNGUI>().text);
			correctAnsAnim (AnsOpGOList [buttonNo]);
		}else 
			incorrectAnsAnim (AnsOpGOList[buttonNo]);
		base.setSelectionflag(quesAnsList,buttonNo);
		//GO: Set color of user selected option to light color
//		quesAnsList.postQuestionCalculations (base.getSolutionFlag(quesAnsList,buttonNo), (float)(currentTime));
//		QuesEndAnimation(1,-1);
		//Starting next question
		//changeIndex(1);
//		changeQuestionIndex(1,-1);
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
