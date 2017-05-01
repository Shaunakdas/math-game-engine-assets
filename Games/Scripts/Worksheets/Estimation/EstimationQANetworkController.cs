using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using SimpleJSON;
public class EstimationQANetworkController : MonoBehaviour {

	public static readonly string domain = "localhost:3000";
	EstimationQAViewController commonQAViewCtrl;
	public bool getQAListNetworkCallResponse { get; set; } 
	public bool postQAAttemptNetworkCallResponse { get; set; } 


	// Use this for initialization
	void Start () {

	}
	public string getDomainAddress(){
		return domain;
	}
	string authenticate(string username, string password)
	{
		string auth = username + ":" + password;
		auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
		auth = "Basic " + auth;
		return auth;
	}
	public void setQAListJSON(QuesAnsList quesAnsList){
		StartCoroutine (getQAListNetworkCall (quesAnsList));
	}
	IEnumerator getQAListNetworkCall(QuesAnsList quesAnsList) {
		commonQAViewCtrl = (EstimationQAViewController) gameObject.GetComponent(typeof(EstimationQAViewController));;
		string getQuesListUrl;
		getQuesListUrl = getDomainAddress () + "/api/worksheet/get_worksheet?worksheet_id=6";

		Debug.Log("getQuesListUrl"+getQuesListUrl);
		UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get (getQuesListUrl);
		string authorization = authenticate("education", "education");
		www.SetRequestHeader("AUTHORIZATION", authorization);
		yield return www.Send ();

		if (www.isError) {
			Debug.Log (www.error);
			//getQAListNetworkCallResponse = false;
		} else {
			//getQAListNetworkCallResponse = true;
//			getQAListJSONText = www.downloadHandler.text; 
			setQAListJSON (www.downloadHandler.text,quesAnsList);
			commonQAViewCtrl.getQAListCallFinished ();
		}
	}

	public void setQAListJSON(string qaJSONText,QuesAnsList quesAnsList){
		var N = JSON.Parse(qaJSONText);
		int questionCount =( N ["questions"].Count);
		Debug.Log ("Values of diagnostic test id"+N["worksheet_id"].Value + qaJSONText);
		quesAnsList.DisplayId = int.Parse(N ["worksheet_id"].Value);
		int maxLives = 4;
		int maxTotalTime = 900;
		quesAnsList.setMaxParams (maxLives, maxTotalTime);
		//for loop on standardCount
		//add standard from API
		for (int i = 0; i <   N ["questions"].Count; i = i + 1) {
			string question=N ["questions"][i]["question_text"].Value;
			string question_image=N ["questions"][i]["question_image"].Value; 
			if (question_image == "null")
				question_image = "";
			string hint=""; 
			string answer_description=N ["questions"][i]["answer"].Value;
			int listIndex=i;
			int backendId = int.Parse(N ["questions"][i]["short_choice_question_id"].Value);
			int opCount = (N ["questions"][i]["answers"].Count);
			//			string[] ansOps = new string[opCount];
			//			string[] ansOpImgs = new string[opCount];
			//			int correctOp = 0;
			//Changing to AnswerOption
			List<AnswerOption> ansOptionListNetwork = new List<AnswerOption>();

			for (int j = 0; j < opCount; j = j + 1) {
				
				string answerImg = N ["questions"] [i] ["answers"] [j] ["image"].Value;
				if (answerImg == "null")
					answerImg = "";
				Debug.Log ("Values of i"+N ["questions"] [i] ["answers"] [j] ["image"].Value);
				AnswerOption ansOp = new AnswerOption(N ["questions"] [i] ["answers"] [j] ["answer_text"].Value,answerImg, false,int.Parse( N ["questions"] [i] ["answers"] [j] ["short_choice_answer_id"].Value)) ;
				if ((N ["questions"] [i] ["answers"] [j] ["correct"].Value)=="true") {
					ansOp.correctFlag = true;
				}
				ansOptionListNetwork.Add (ansOp);
			}

			quesAnsList.add (question, ansOptionListNetwork, hint, answer_description,question_image, listIndex, backendId);

		}
	}
	public List<UserEntityResult> getUserResultList(string resultJSONText){
		List<UserEntityResult> userStreamResultList = new List<UserEntityResult> ();
		Debug.Log (resultJSONText);
		var N = JSON.Parse(resultJSONText);
		N = N ["result"] ["streams"];
		for (int i = 0; i < N.Count; i = i + 1) {
			//			Debug.Log (i);
			//			Debug.Log (N[i].Count);
			//			Debug.Log (N[i]["other_details"].Count);
			//			Debug.Log (N[i]["other_details"]["stream_name"].Count);
			//			Debug.Log (N[i]["other_details"]["stream_name"].Value);
			if (N["result"].Count != null) {
				Debug.Log ("Adding userResult "+N[i] ["other_details"] ["average_score"].Value);
				UserEntityResult userStreamResult = new UserEntityResult ();
				userStreamResult.EntityTitle = N [i] ["other_details"] ["stream_name"].Value;
				userStreamResult.UserResultValue = (int)float.Parse(N[i] ["other_details"] ["average_score"].Value);
				userStreamResultList.Add (userStreamResult);
			}
		}
		Debug.Log ("Length of userStreamResultList"+userStreamResultList.Count);
		return userStreamResultList;
	}
	public List<UserEntityResult> getUserWeakEntityList(string resultJSONText){
		List<UserEntityResult> userWeakEntityList = new List<UserEntityResult> ();

		var N = JSON.Parse(resultJSONText);
		int test = int.Parse (N ["personalized_test_remaining"].Value);
		if (test > 0) {
//			commonTestObject.getPersonalizedTestCount (test);
		}
		N = N ["weak_entity"] ["entity_list"];
		for (int j = 0; j < N.Count; j = j + 1) {
			if (N.Count != null) {
				Debug.Log (" Iteration "+ j);
				Debug.Log ("Adding userResult "+N[j]["total"].Value);
				UserEntityResult userStreamResult = new UserEntityResult ();
				userStreamResult.EntityTitle = N[j]["name"].Value;
				userStreamResult.UserResultValue = int.Parse(N [j] ["incorrect"].Value);
				userStreamResult.UserResultMaxValue = int.Parse(N [j] ["total"].Value);
				userWeakEntityList.Add (userStreamResult);
			}
		}
		Debug.Log ("Length of userStreamResultList"+userWeakEntityList.Count);
		return userWeakEntityList;
	}
	public string getQAAttemptJSON(QuesAnsList quesAnsList){
		//CreatingJSONNODE
		JSONNode PostJson = new JSONClass(); // Start with JSONArray or JSONClass

		PostJson["user"]["first_name"] = UserProfile.firstName;
		PostJson["user"]["last_name"] = UserProfile.lastName;
		PostJson["user"]["email"] = UserProfile.email;
		PostJson["user"]["number"] = UserProfile.number;
		PostJson["diagnostic_test"]["id"] = quesAnsList.DisplayId.ToString();
		PostJson["diagnostic_test"]["personalization_type"] = quesAnsList.PersonalizationType.ToString();

		for (var i = 0; i < quesAnsList.QAList.Count; i++) {
			//Console.WriteLine("Amount is {0} and type is {1}", quesAnsList[i].amount, quesAnsList[i].type);
			string question_id = quesAnsList.QAList[i].BackendId.ToString();
			PostJson["diagnostic_test"]["short_choice_questions"][question_id]["question_text"] = quesAnsList.QAList[i].getQuesText().ToString();
			PostJson["diagnostic_test"]["short_choice_questions"][question_id]["index"] = i.ToString();
			PostJson["diagnostic_test"]["short_choice_questions"][question_id]["time_taken"] = quesAnsList.QAList[i].getUserTimeTaken().ToString();
			PostJson["diagnostic_test"]["short_choice_questions"][question_id]["score"] = quesAnsList.QAList[i].getUserScore().ToString();
			PostJson["diagnostic_test"]["short_choice_questions"][question_id]["attempt"] = quesAnsList.QAList[i].userAttempt.ToString();
			if(quesAnsList.QAList[i].ansOptionList.FirstOrDefault(option => option.selectedFlag == true)!=null){
				PostJson["diagnostic_test"]["short_choice_questions"][question_id]["answer_selected"] = quesAnsList.QAList[i].ansOptionList.FirstOrDefault(option => option.selectedFlag == true).optionId.ToString();
			}
			for (var j =0; j< quesAnsList.QAList[i].selectedOpList.Count ; j++){
				string answer_id = quesAnsList.QAList [i].selectedOpList [j].optionId.ToString();
				PostJson["diagnostic_test"]["short_choice_questions"][question_id]["selected_answers"][answer_id]["index"] = j.ToString();
				PostJson["diagnostic_test"]["short_choice_questions"][question_id]["selected_answers"][answer_id]["time_taken"] = quesAnsList.QAList [i].selectedOpList [j].timeFromLastSelection.ToString();
				PostJson["diagnostic_test"]["short_choice_questions"][question_id]["selected_answers"][answer_id]["text"] = quesAnsList.QAList [i].selectedOpList [j].optionText.ToString();
			}

		}

		string json_string="";
		json_string = PostJson.ToString();
		Debug.Log ("PostQuesAttempt Json" + json_string);
		return json_string;
	}
	IEnumerator postQAListNetworkCall(string post_json){
		//worksheetObject.openWaitingPanel ();
		string postQuesAttemptUrl = getDomainAddress() + "/api/diagnostic_tests/test_attempt";
		WWW www;

		//Creating header
		Dictionary<string,string> postHeader = new Dictionary<string,string>();
		postHeader.Add("Content-Type", "application/json");
		string authorization = authenticate("education", "education");
		postHeader.Add("AUTHORIZATION", authorization);
		// convert json string to byte
		var formData = System.Text.Encoding.UTF8.GetBytes(post_json);

		www = new WWW(postQuesAttemptUrl, formData, postHeader);
		yield return www; // Wait until the download is done
		if (www.error != null){
			Debug.Log("There was an error sending request: " + www.error);
			//postQAAttemptNetworkCallResponse = false;
		}else{
			Debug.Log("WWW Response: " + www.text);
			string postQAAttemptText = www.text;
		}
		//StartCoroutine(PostQuestionAttempt(www));
	}
	//	public IEnumerator getQAListNetworkCall(QuesAnsList quesAnsList) {
	//		var standard_id = 4;
	//		string getQuesListUrl = diagnosticTestObject.getDomainAddress() + "/api/diagnostic_tests/get_test.json?standard_id="+standard_id+"&subject_id="+standard_id;
	//		UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get (getQuesListUrl);
	//		yield return www.Send ();
	//
	//		if (www.isError) {
	//			Debug.Log (www.error);
	//			getQAListNetworkCallResponse = false;
	//		} else {
	//			getQAListNetworkCallResponse = true;
	//			var the_JSON_string = www.downloadHandler.text; 
	//			var N = JSON.Parse(the_JSON_string);
	//			int questionCount =( N ["questions"].Count);
	//			Debug.Log ("Values of diagnostic test id"+N["diagnostic_test_id"].Value);
	//			quesAnsList.DisplayId = int.Parse(N ["diagnostic_test_id"].Value);
	//			int maxLives = 4;
	//			int maxTotalTime = 900;
	//			quesAnsList.setMaxParams (maxLives, maxTotalTime);
	//			//for loop on standardCount
	//			//add standard from API
	//			for (int i = 0; i < questionCount; i = i + 1) {
	//				string question=N ["questions"][i]["question_text"].Value;
	//				string question_image=N ["questions"][i]["question_image"].Value;
	//				string hint=""; 
	//				string answer_description=N ["questions"][i]["answer"].Value;
	//				int listIndex=i;
	//				int backendId = int.Parse(N ["questions"][i]["short_choice_question_id"].Value);
	//				int opCount = (N ["questions"][i]["answers"].Count);
	//				string[] ansOps = new string[opCount];
	//				string[] ansOpImgs = new string[opCount];
	//				int correctOp = 0;
	//				for (int j = 0; j < opCount; j = j + 1) {
	//					ansOps [j] = N ["questions"] [i] ["answers"] [j] ["answer_text"].Value;
	//					ansOpImgs [j] = N ["questions"] [i] ["answers"] [j] ["image"].Value;
	//
	//					if ((N ["questions"] [i] ["answers"] [j] ["correct"].Value)=="true") {
	//						correctOp = j;
	//					}
	//
	//				}
	//				Answer givenAnswer= new Answer(ansOps,opCount,correctOp,ansOpImgs);
	//
	//				quesAnsList.add (question, givenAnswer, hint, answer_description,question_image, listIndex, backendId);
	//			}
	//			//setQuesAnsBasedOnIndex (0, true);
	//			//diagnosticTestObject.updateAPIStatus ("GetQAList",true);
	//		}
	//	}
	//
	//	public IEnumerator postQAListNetworkCall(QuesAnsList quesAnsList){
	//		//diagnosticTestObject.openWaitingPanel ();
	//		string postQuesAttemptUrl = diagnosticTestObject.getDomainAddress() + "api/diagnostic_tests/test_attempt";
	//		WWW www;
	//
	//		//Creating header
	//		Dictionary<string,string> postHeader = new Dictionary<string,string>();
	//		postHeader.Add("Content-Type", "application/json");
	//
	//		//CreatingJSONNODE
	//		JSONNode PostJson = new JSONClass(); // Start with JSONArray or JSONClass
	//
	//		PostJson["user"]["first_name"] = "shaunak";
	//		PostJson["user"]["last_name"] = "das";
	//		PostJson["user"]["email"] = "shaunakdas2020@gmail.com";
	//		PostJson["user"]["number"] = "9740644522";
	//		PostJson["diagnostic_test"]["id"] = quesAnsList.DisplayId.ToString();
	//
	//		for (var i = 0; i < quesAnsList.QAList.Count; i++) {
	//			//Console.WriteLine("Amount is {0} and type is {1}", quesAnsList[i].amount, quesAnsList[i].type);
	//
	//			PostJson["diagnostic_test"]["short_choice_questions"][quesAnsList.QAList[i].BackendId.ToString()]["time_taken"] = quesAnsList.QAList[i].getUserTimeTaken().ToString();
	//			PostJson["diagnostic_test"]["short_choice_questions"][quesAnsList.QAList[i].BackendId.ToString()]["score"] = quesAnsList.QAList[i].getUserScore().ToString();
	//		}
	//
	//		string json_string="";
	//		json_string = PostJson.ToString();
	//		Debug.Log ("PostQuesAttempt Json" + json_string);
	//		// convert json string to byte
	//		var formData = System.Text.Encoding.UTF8.GetBytes(json_string);
	//
	//		www = new WWW(postQuesAttemptUrl, formData, postHeader);
	//		yield return www; // Wait until the download is done
	//		if (www.error != null){
	//			Debug.Log("There was an error sending request: " + www.error);
	//			postQAAttemptNetworkCallResponse = false;
	//		}else{
	//			Debug.Log("WWW Response: " + www.text);
	//			postQAAttemptNetworkCallResponse = true;
	//		}
	//		//StartCoroutine(PostQuestionAttempt(www));
	//	}
	//

	// Update is called once per frame
	void Update () {

	}
}

