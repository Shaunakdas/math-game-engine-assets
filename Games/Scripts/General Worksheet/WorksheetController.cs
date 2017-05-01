using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

using SimpleJSON;

public class WorksheetController : MonoBehaviour {
	public GameObject preWorksheetGO;
	public GameObject worksheetScoreboardGO;
	public GameObject worksheetQAGO;

	public GameObject PrWPBeginBtnGO, PoWPBeginBtnGO, ResetBtnGo, EndBtnGO; 

	public static readonly string domain = "http://69c7e723.ngrok.io";
	PreWorksheetController preWorksheetObject;
	WorksheetScoreBoardController worksheetScoreboardObject;
	WorksheetQAViewController qAObject;

	string getQAListJSONText,postQAAttemptText, getWorksheetJSONText;
	Animator anim;
	int worksheetPanelHash, postWorksheetHash, scoreboardHash,restartWorksheetHash;
	// Use this for initialization
	void Start () {
		Debug.Log ("WorksheetController start() called");
		//Initiate animation components
		anim = GetComponent<Animator>();
		worksheetPanelHash = Animator.StringToHash("beginTrigger");
		postWorksheetHash = Animator.StringToHash("postWorksheetTrigger");
		scoreboardHash = Animator.StringToHash("scoreboardTrigger");
		restartWorksheetHash = Animator.StringToHash("restartWorksheetTrigger");

		//Initiate script components of child gameobjects
		preWorksheetObject = (PreWorksheetController) preWorksheetGO.GetComponent(typeof(PreWorksheetController));
		worksheetScoreboardObject = (WorksheetScoreBoardController) worksheetScoreboardGO.GetComponent(typeof(WorksheetScoreBoardController));
		qAObject = (WorksheetQAViewController) worksheetQAGO.GetComponent(typeof(WorksheetQAViewController));
		openPreWorksheetPanel ();
	}
	public string getDomainAddress(){
		return domain;
	}
	//Calling PreWorksheetPanel
	public void openPreWorksheetPanel (){
		//Calling API call for qAObject
		StartCoroutine(getQAListNetworkCall());
		StartCoroutine(GetWorksheet());
		//preWorksheetObject.getWorksheetDetailsNetworkCall();
		//qAObject.getQAListAPI();
	}
	//Calling WorksheetPanel
	public void openWorksheetPanel (){
		//Called when button is pressed in PreWorksheetPanel
		anim.SetTrigger (worksheetPanelHash);


	}
	//Calling PostWorksheetPanel
	public void openPostWorksheetPanel (){
		//Called when button is pressed in WorksheetPanel
		anim.SetTrigger (postWorksheetHash);
		StartCoroutine (postQAListNetworkCall (qAObject.getQAList ()));
	}
	//Calling ScoreboardPanel
	public void openScoreboardPanel (){
		//Called when button is pressed in PostWorksheetPanel
		anim.SetTrigger (scoreboardHash);
	}


	//Calling New StreamwiseScene
	public void openStreamwiseScene (){
		//Called when button is pressed in UserResultPanel
		SceneManager.LoadScene("StreamwiseScene");
	}
	//Calling PreWorksheetPanel
	public void restartPostWorksheetPanel (){
		//Called when button is pressed in WorksheetPanel
		anim.SetTrigger (restartWorksheetHash);
		openPreWorksheetPanel ();
	}



	// Update is called once per frame
	void Update () {
	
	}
	public void updateAPIStatus (string APIMethodName,bool success){
		switch (APIMethodName)
		{
		case "GetWorksheet":
			//updateButtonContent (PrWPBeginBtnGO, "Get Started", success);
			break;
		case "GetQAList":
			updateButtonContent (PrWPBeginBtnGO, "Lets Play!", success);
			break;
		case "PostQuestionAttempt":
			updateButtonContent (PoWPBeginBtnGO, "Lets see results", success);
			break;

		}
	}
	public void updateButtonContent(GameObject btnGameObject,string text, bool interact){
		//Set Button Text
		btnGameObject.GetComponentInChildren<Text>().text  = text;
		btnGameObject.GetComponent<Button> ().interactable = interact;

	}
	IEnumerator getQAListNetworkCall() {
		var standard_id = 4;
		string getQuesListUrl = getDomainAddress() + "/api/diagnostic_tests/get_test.json?standard_id="+standard_id+"&subject_id="+standard_id;
		UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get (getQuesListUrl);
		yield return www.Send ();

		if (www.isError) {
			Debug.Log (www.error);
			//getQAListNetworkCallResponse = false;
		} else {
			//getQAListNetworkCallResponse = true;
			getQAListJSONText = www.downloadHandler.text; 
			qAObject.setQAList (getQAListJSONText);
			updateAPIStatus ("GetQAList", true);
		}
	}
	IEnumerator postQAListNetworkCall(QuesAnsList quesAnsList){
		//worksheetObject.openWaitingPanel ();
		string postQuesAttemptUrl = getDomainAddress() + "/api/diagnostic_tests/test_attempt";
		WWW www;

		//Creating header
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");

		//CreatingJSONNODE
		JSONNode PostJson = new JSONClass(); // Start with JSONArray or JSONClass

		PostJson["user"]["first_name"] = "shaunak";
		PostJson["user"]["last_name"] = "das";
		PostJson["user"]["email"] = "shaunakdas2020@gmail.com";
		PostJson["user"]["number"] = "9740644522";
		PostJson["diagnostic_test"]["id"] = quesAnsList.DisplayId.ToString();

		for (var i = 0; i < quesAnsList.QAList.Count; i++) {
			//Console.WriteLine("Amount is {0} and type is {1}", quesAnsList[i].amount, quesAnsList[i].type);

			PostJson["diagnostic_test"]["short_choice_questions"][quesAnsList.QAList[i].BackendId.ToString()]["time_taken"] = quesAnsList.QAList[i].getUserTimeTaken().ToString();
			PostJson["diagnostic_test"]["short_choice_questions"][quesAnsList.QAList[i].BackendId.ToString()]["score"] = quesAnsList.QAList[i].getUserScore().ToString();
		}

		string json_string="";
		json_string = PostJson.ToString();
		Debug.Log ("PostQuesAttempt Json" + json_string);
		// convert json string to byte
		var formData = System.Text.Encoding.UTF8.GetBytes(json_string);

		www = new WWW(postQuesAttemptUrl, formData, postHeader);
		yield return www; // Wait until the download is done
		if (www.error != null){
			Debug.Log("There was an error sending request: " + www.error);
			//postQAAttemptNetworkCallResponse = false;
		}else{
			Debug.Log("WWW Response: " + www.text);
			postQAAttemptText = www.text;
			updateAPIStatus ("PostQuestionAttempt", true);
			//postQAAttemptNetworkCallResponse = true;
		}
		//StartCoroutine(PostQuestionAttempt(www));
	}
	IEnumerator GetWorksheet() {
		string postQuesAttemptUrl = getDomainAddress() + "/api/worksheet/get_intro";
		WWW www;

		//Creating header
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");

		//CreatingJSONNODE
		JSONNode PostJson = new JSONClass(); // Start with JSONArray or JSONClass

		PostJson["user"]["first_name"] = "shaunak";
		PostJson["user"]["last_name"] = "das";
		PostJson["user"]["email"] = "shaunakdas2020@gmail.com";
		PostJson["user"]["number"] = "9740644522";
		PostJson["worksheet_id"] = "1";

		string json_string="";
		json_string = PostJson.ToString();
		Debug.Log ("PostQuesAttempt Json" + json_string);
		// convert json string to byte
		var formData = System.Text.Encoding.UTF8.GetBytes(json_string);

		www = new WWW(postQuesAttemptUrl, formData, postHeader);
		yield return www; // Wait until the download is done
		if (www.error != null){
			Debug.Log("There was an error sending request of GetWorksheet: " + www.error);
			//getWorksheetNetworkCallResponse = false;
		}else{
			Debug.Log("Response of GetWorksheet: " + www.text);
			getWorksheetJSONText =(www.text);
			preWorksheetObject.setWorksheet (getWorksheetJSONText);
		}
	}
}
