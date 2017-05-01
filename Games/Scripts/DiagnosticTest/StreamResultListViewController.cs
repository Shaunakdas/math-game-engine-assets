using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StreamResultListViewController : MonoBehaviour {

	//Prefabs
	public GameObject ResultBarPF,QuestionItemPF,QuestionItemTextPF;

	//GameObject References
	public GameObject ResultBarPanelGO,URPAgainBtnGO;
	List<GameObject> userResultGOList,questionItemGOList;

	//List Variables
	List<UserEntityResult> userStreamResultList;
	List<UserEntityResult> userWeakEntityList;
	QuesAnsList qaList;

	//Script References
	DiagnosticTestController diagnosticTestObject;
	DiagQANetworkController diagQANetworkObject;

	//Function which will be called by DiagnosticTestController
	public void setUserStreamResultList (string resultJSONText){
		Debug.Log ("Inside setUserStreamResultList of StreamResultListViewController");
		URPAgainBtnGO.SetActive (false);
		diagQANetworkObject = (DiagQANetworkController) gameObject.GetComponent(typeof(DiagQANetworkController));
		userStreamResultList = new List<UserEntityResult> ();
		userStreamResultList = diagQANetworkObject.getUserResultList (resultJSONText);
		userWeakEntityList = new List<UserEntityResult> ();
		userWeakEntityList = diagQANetworkObject.getUserWeakEntityList (resultJSONText);
		userResultGOList = new List<GameObject> ();
		questionItemGOList = new List<GameObject> ();
		//setResultBarLayout ();
	}

	public void setQAList(QuesAnsList quesAnsList){
		qaList = new QuesAnsList ();
		qaList = quesAnsList;
	}
	//Function for adding Result Bars
	public void setResultBarLayout(){
		destroyChildGOList (ResultBarPanelGO);
		setStreamScoreList ();
		setIncorrectQuesAnsList(QuestionItemPF,QuestionItemTextPF,ResultBarPanelGO,questionItemGOList,qaList);
		setWeakEntityQuesAnsList ();
		displayScaledSizes ();
	}
	public void setStreamScoreList(){
		Debug.Log ("Size of userStreamResultList"+userStreamResultList.Count);
		for (int j = 0; j < userStreamResultList.Count; j = j + 1) {
			GameObject userStreamResultGO = (GameObject)Instantiate (ResultBarPF, ResultBarPanelGO.transform);
			userStreamResultGO.name = "ResultBar";
			getChildGameObject (userStreamResultGO, "ResultTitle").GetComponent<Text> ().text = "Your Score";
			getChildGameObject (userStreamResultGO, "StreamTitle").GetComponent<Text> ().text = userStreamResultList [j].EntityTitle;
			getChildGameObject (userStreamResultGO, "ResultValue").GetComponent<Text> ().text = userStreamResultList [j].UserResultValue+" out of "+userStreamResultList [j].UserResultMaxValue;
			userResultGOList.Add (userStreamResultGO);
			setAnimation (getChildGameObject (getChildGameObject (userStreamResultGO, "ResultBarSmall"), "ResultBarFG"), (float)userStreamResultList [j].UserResultValue, (float)userStreamResultList [j].UserResultMaxValue);
		}
	}
	public void setIncorrectQuesAnsList(GameObject QuestionItemPF, GameObject QuestionItemTextPF,GameObject ResultBarPanelGO, List<GameObject> questionItemGOList,QuesAnsList qaList){
		
		Debug.Log ("Size of qaList"+qaList.QAList.Count);
		Debug.Log ("Incorrect and Skipped Answers");

		string questionListHeader;
		if (qaList.QAList.FindIndex (option => option.userAttempt != 3)>-1 || ((qaList.QAList.FindIndex (option => option.userAttempt == 3)>-1) && (qaList.QAList.FindIndex (option => option.getUserTimeTaken() > 30.0f)>-1))) {
			questionListHeader = "Your score was not perfect. Here are the details of your mistakes:-";
		} else {
			questionListHeader = "Your score was perfect.";
		}
		Debug.Log ("questionListHeader "+questionListHeader);
		GameObject questionListHeaderGO = (GameObject)Instantiate (QuestionItemPF, ResultBarPanelGO.transform);
		getChildGameObject (questionListHeaderGO, "QuestionItemText").GetComponent<TEXDraw>().text = "\\opens[b]{"+questionListHeader+"}";
		questionItemGOList.Add (questionListHeaderGO);
		for (int j = 0; j < qaList.QAList.Count; j = j + 1) {
			int attempt = qaList.QAList [j].userAttempt;
			if ((attempt == 1)||(attempt == 2)) {
				QuesAnsPair qaPair = new QuesAnsPair ();
				qaPair = qaList.QAList [j];

				Debug.Log ("Question "+ (j+1)+": " + qaPair.getQuesText ());
				GameObject incorrectQuestionItemGO = (GameObject)Instantiate (QuestionItemPF, ResultBarPanelGO.transform);
				GameObject questionItemTextGO = getChildGameObject (incorrectQuestionItemGO, "QuestionItemText");
				questionItemTextGO.GetComponent<TEXDraw>().text = "\\opens[b]{Question "+ (j+1)+":}" + StringWrapper.changeString(qaPair.getQuesText ());

				GameObject selectedAnswerGO = (GameObject)Instantiate (QuestionItemTextPF, incorrectQuestionItemGO.transform);
				if (attempt == 1) {
					Debug.Log ("Answer Skipped ");
					selectedAnswerGO.GetComponent<TEXDraw> ().text = "\\opens[b]{Answer Skipped} ";
				} else {
					Debug.Log ("Answer Selected " + qaPair.ansOptionList.Find (option => option.selectedFlag == true).optionText);
					selectedAnswerGO.GetComponent<TEXDraw> ().text = "\\opens[b]{You answered: }" + StringWrapper.changeString(qaPair.ansOptionList.Find (option => option.selectedFlag == true).optionText);
				}
				Debug.Log(selectedAnswerGO.GetComponent<RectTransform>().rect.height);

				Debug.Log ("Correct Answer " + qaPair.ansOptionList.Find(option => option.correctFlag == true).optionText);
				GameObject correctAnswerGO = (GameObject)Instantiate (QuestionItemTextPF, incorrectQuestionItemGO.transform);
				correctAnswerGO.GetComponent<TEXDraw> ().text = "\\opens[b]{Correct Answer: }" + StringWrapper.changeString(qaPair.ansOptionList.Find(option => option.correctFlag == true).optionText);
				Debug.Log(correctAnswerGO.GetComponent<RectTransform>().rect.height);

				questionItemGOList.Add (incorrectQuestionItemGO);
			}
		}
		Debug.Log ("Correct Answers where you took a lot of time");
		for (int j = 0; j < qaList.QAList.Count; j = j + 1) {
			if ((qaList.QAList [j].getUserTimeTaken() > 30.0f)&&(qaList.QAList [j].userAttempt == 3))  {
				//			GameObject userStreamResultGO = (GameObject)Instantiate (ResultBarPF, ResultBarPanelGO.transform);
				QuesAnsPair qaPair = new QuesAnsPair ();
				qaPair = qaList.QAList [j];
				Debug.Log ("Question # " + (j+1));
				Debug.Log ("Question " + qaPair.getQuesText ());
				GameObject incorrectQuestionItemGO = (GameObject)Instantiate (QuestionItemPF, ResultBarPanelGO.transform);
				GameObject questionItemTextGO = getChildGameObject (incorrectQuestionItemGO, "QuestionItemText");
				questionItemTextGO.GetComponent<TEXDraw>().text = "\\opens[b]{Question "+ (j+1)+":}" + StringWrapper.changeString(qaPair.getQuesText ());

				Debug.Log ("Answer Selected " + StringWrapper.changeString(qaPair.ansOptionList.Find(option => option.selectedFlag == true).optionText));
				GameObject selectedAnswerGO = (GameObject)Instantiate (QuestionItemTextPF, incorrectQuestionItemGO.transform);
				selectedAnswerGO.GetComponent<TEXDraw> ().text = "\\opens[b]{You answered: }" + StringWrapper.changeString(qaPair.ansOptionList.Find (option => option.selectedFlag == true).optionText);

				Debug.Log ("Time Taken" + qaPair.getUserTimeTaken());
				Debug.Log ("Ideal Time: 30 sec");
				GameObject correctAnswerGO = (GameObject)Instantiate (QuestionItemTextPF, incorrectQuestionItemGO.transform);
				correctAnswerGO.GetComponent<TEXDraw> ().text = "\\opens[b]{Time Taken}" + qaPair.getUserTimeTaken() + "\\opens[b]{Ideal Time: 30 sec}";

				questionItemGOList.Add (incorrectQuestionItemGO);
			}
		}
	}
	public void setWeakEntityQuesAnsList(){
		GameObject weakEntityListHeaderGO = (GameObject)Instantiate (QuestionItemPF, ResultBarPanelGO.transform);
		getChildGameObject (weakEntityListHeaderGO, "QuestionItemText").GetComponent<TEXDraw>().text = "\\opens[b]{According to your results, these are your weak chapters}";
		questionItemGOList.Add (weakEntityListHeaderGO);
		Debug.Log ("Size of userStreamResultList"+userWeakEntityList.Count);
		for (int j = 0; j < userWeakEntityList.Count; j = j + 1) {
			float ratio = ((float)userWeakEntityList [j].UserResultValue) / ((float)userWeakEntityList [j].UserResultMaxValue);
			Debug.Log("Ratio is "+ratio);
			if(ratio>0.5f){
//				URPAgainBtnGO.SetActive (true);
				GameObject userStreamResultGO = (GameObject)Instantiate (ResultBarPF, ResultBarPanelGO.transform);
				userStreamResultGO.name = "ResultBar";
				getChildGameObject (userStreamResultGO, "ResultTitle").GetComponent<Text> ().text = "Incorrect Answers";
				getChildGameObject (userStreamResultGO, "StreamTitle").GetComponent<Text> ().text = userWeakEntityList [j].EntityTitle;
				getChildGameObject (userStreamResultGO, "ResultValue").GetComponent<Text> ().text = userWeakEntityList [j].UserResultValue+" out of "+userWeakEntityList [j].UserResultMaxValue;
				userResultGOList.Add (userStreamResultGO);
				setAnimation (getChildGameObject (getChildGameObject (userStreamResultGO, "ResultBarSmall"), "ResultBarFG"), (float)userWeakEntityList [j].UserResultValue, (float)userWeakEntityList [j].UserResultMaxValue);
			}
		}
	}
	static public GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		//Author: Isaac Dart, June-13.
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
		return null;
	}
	//Function for setting animation of filling ResultBar
	//public void startAnimation(List<GameObject> resultBarPanelGoList, 
	public void setAnimation(GameObject ResultBarFG, float userValue, float maxValue){
		ResultBarFG.GetComponent<Image>().fillAmount =  userValue/maxValue;
	}
	public void destroyResultBarLayout(){
		foreach (GameObject imageObject in userResultGOList) {
			Destroy (imageObject);
		}
		userResultGOList.Clear ();

		foreach (GameObject imageObject in questionItemGOList) {
			Destroy (imageObject);
		}
		questionItemGOList.Clear ();

	}
	// Use this for initialization
	void Start () {
		//userStreamResultList = new List<UserEntityResult> ();
	}
	
	// Update is called once per frame
	void Update () {

	}
	void displayScaledSizes(){
		foreach (GameObject ansOpGO in userResultGOList) {
			ansOpGO.GetComponent<LayoutElement> ().preferredHeight = ScreenManager.scaledYSize (ansOpGO.GetComponent<LayoutElement> ().preferredHeight);
			ansOpGO.GetComponent<LayoutElement> ().preferredWidth = ScreenManager.scaledXSize (ansOpGO.GetComponent<LayoutElement> ().preferredWidth);
			getChildGameObject (ansOpGO, "StreamTitle").GetComponent<Text> ().fontSize = (int)ScreenManager.scaledXSize (getChildGameObject (ansOpGO, "StreamTitle").GetComponent<Text> ().fontSize);
			getChildGameObject (ansOpGO, "ResultValue").GetComponent<Text> ().fontSize = (int)ScreenManager.scaledXSize (getChildGameObject (ansOpGO, "ResultValue").GetComponent<Text> ().fontSize);
		}

		foreach (GameObject questionItemText in GameObject.FindGameObjectsWithTag("ResultQuestionItemText")) {
			//Debug.Log("Text"+questionItemText.GetComponent<RectTransform>().rect.height);
			questionItemText.GetComponent<TEXDraw> ().size = (int)ScreenManager.scaledXSize (questionItemText.GetComponent<TEXDraw> ().size);
			questionItemText.GetComponent<LayoutElement> ().minWidth = ScreenManager.scaledXSize (questionItemText.GetComponent<LayoutElement> ().minWidth);
			questionItemText.GetComponent<LayoutElement> ().minHeight = ScreenManager.scaledYSize (questionItemText.GetComponent<LayoutElement> ().minHeight);
		}
		foreach (GameObject quesGO in questionItemGOList) {
			quesGO.GetComponent<LayoutElement> ().preferredWidth = ScreenManager.scaledXSize (quesGO.GetComponent<LayoutElement> ().preferredWidth);
		}

	}
	void destroyChildGOList(GameObject GO){
		foreach (Transform child in GO.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}
}
