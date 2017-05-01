using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;
public class WorksheetQANetworkController : MonoBehaviour {
	WorksheetController worksheetObject;
	public GameObject worksheetGO;

	public bool getQAListNetworkCallResponse { get; set; } 
	public bool postQAAttemptNetworkCallResponse { get; set; } 
	// Use this for initialization
	void Start () {
		worksheetObject = (WorksheetController) worksheetGO.GetComponent(typeof(WorksheetController));
	}
	public void setQAListJSON(string qaJSONText,QuesAnsList quesAnsList){
		var N = JSON.Parse(qaJSONText);
		int questionCount =( N ["questions"].Count);
		Debug.Log ("Values of diagnostic test id"+N["diagnostic_test_id"].Value);
		quesAnsList.DisplayId = int.Parse(N ["diagnostic_test_id"].Value);
		int maxLives = 4;
		int maxTotalTime = 900;
		quesAnsList.setMaxParams (maxLives, maxTotalTime);
		//for loop on standardCount
		//add standard from API
		for (int i = 0; i < questionCount; i = i + 1) {
			string question=N ["questions"][i]["question_text"].Value;
			string question_image=N ["questions"][i]["question_image"].Value;
			string hint=""; 
			string answer_description=N ["questions"][i]["answer"].Value;
			int listIndex=i;
			int backendId = int.Parse(N ["questions"][i]["short_choice_question_id"].Value);
			int opCount = (N ["questions"][i]["answers"].Count);
			string[] ansOps = new string[opCount];
			string[] ansOpImgs = new string[opCount];
			int correctOp = 0;
			for (int j = 0; j < opCount; j = j + 1) {
				ansOps [j] = N ["questions"] [i] ["answers"] [j] ["answer_text"].Value;
				ansOpImgs [j] = N ["questions"] [i] ["answers"] [j] ["image"].Value;

				if ((N ["questions"] [i] ["answers"] [j] ["correct"].Value)=="true") {
					correctOp = j;
				}

			}
			Answer givenAnswer= new Answer(ansOps,opCount,correctOp,ansOpImgs);

			quesAnsList.add (question, givenAnswer, hint, answer_description,question_image, listIndex, backendId);
		}
	}
//	public IEnumerator getQAListNetworkCall(QuesAnsList quesAnsList) {
//		var standard_id = 4;
//		string getQuesListUrl = worksheetObject.getDomainAddress() + "/api/diagnostic_tests/get_test.json?standard_id="+standard_id+"&subject_id="+standard_id;
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
//			//worksheetObject.updateAPIStatus ("GetQAList",true);
//		}
//	}
//
//	public IEnumerator postQAListNetworkCall(QuesAnsList quesAnsList){
//		//worksheetObject.openWaitingPanel ();
//		string postQuesAttemptUrl = worksheetObject.getDomainAddress() + "api/diagnostic_tests/test_attempt";
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


	// Update is called once per frame
	void Update () {

	}
}
