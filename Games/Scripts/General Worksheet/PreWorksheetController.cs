using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;

using System;

public class PreWorksheetController : MonoBehaviour {
	public GameObject worksheetGO;
	WorksheetController worksheetObject;
	UserWorksheetPerformance userWorksheetPerfObject;
	public bool getWorksheetNetworkCallResponse { get; set; } 
	QuesAnsList quesAnsList ;

	public GameObject TopicGO,ChapterGO,HighScoreGO,DiamondsGO,AttemptsGO;
	// Use this for initialization
	void Start () {
		Debug.Log ("PreWorksheetController start() called");
		worksheetObject = (WorksheetController) worksheetGO.GetComponent(typeof(WorksheetController));
		userWorksheetPerfObject =  (UserWorksheetPerformance) worksheetGO.GetComponent(typeof(UserWorksheetPerformance));
	}
//	public void getWorksheetDetailsNetworkCall(){
//		StartCoroutine(GetWorksheet());
//	}
	public void setWorksheet(string JSONText){
		var N = JSON.Parse(JSONText);
		userWorksheetPerfObject.id = int.Parse( N["worksheet_id"].Value);
		userWorksheetPerfObject.Topic = N["second_topic"]["name"].Value;
		userWorksheetPerfObject.Stream = N["stream"]["name"].Value;
		userWorksheetPerfObject.Chapter = N["chapter"]["name"].Value;
		userWorksheetPerfObject.Subject = N["subject"]["name"].Value;
		userWorksheetPerfObject.HighScore =parseJSONStringToInt( N["score"].Value);
		userWorksheetPerfObject.Diamonds = parseJSONStringToInt( N["diamonds"].Value);
		userWorksheetPerfObject.MaxDiamonds = 3;
		userWorksheetPerfObject.Wins = parseJSONStringToInt( N[""].Value);
		userWorksheetPerfObject.Attempts = parseJSONStringToInt( N[""].Value);
		userWorksheetPerfObject.CurrentDifficultyLevel = parseJSONStringToInt( N[""].Value);
		userWorksheetPerfObject.NextDifficultyLevel = parseJSONStringToInt( N[""].Value);
		userWorksheetPerfObject.Ranking = parseJSONStringToFloat( N[""].Value);
		updatePreWorksheetView ();
		worksheetObject.updateAPIStatus ("GetWorksheet",true);
	
	}
//	IEnumerator GetWorksheet() {
//		string postQuesAttemptUrl = worksheetObject.getDomainAddress() + "api/worksheet/get_intro";
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
//		PostJson["worksheet_id"] = "1";
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
//			Debug.Log("There was an error sending request of GetWorksheet: " + www.error);
//			getWorksheetNetworkCallResponse = false;
//		}else{
//			Debug.Log("Response of GetWorksheet: " + www.text);
//			var N = JSON.Parse(www.text);
//			userWorksheetPerfObject.id = int.Parse( N["worksheet_id"].Value);
//			userWorksheetPerfObject.Topic = N["second_topic"]["name"].Value;
//			userWorksheetPerfObject.Stream = N["stream"]["name"].Value;
//			userWorksheetPerfObject.Chapter = N["chapter"]["name"].Value;
//			userWorksheetPerfObject.Subject = N["subject"]["name"].Value;
//			userWorksheetPerfObject.HighScore =parseJSONStringToInt( N["score"].Value);
//			userWorksheetPerfObject.Diamonds = parseJSONStringToInt( N["diamonds"].Value);
//			userWorksheetPerfObject.MaxDiamonds = 3;
//			userWorksheetPerfObject.Wins = parseJSONStringToInt( N[""].Value);
//			userWorksheetPerfObject.Attempts = parseJSONStringToInt( N[""].Value);
//			userWorksheetPerfObject.CurrentDifficultyLevel = parseJSONStringToInt( N[""].Value);
//			userWorksheetPerfObject.NextDifficultyLevel = parseJSONStringToInt( N[""].Value);
//			userWorksheetPerfObject.Ranking = parseJSONStringToFloat( N[""].Value);
//			updatePreWorksheetView ();
//			worksheetObject.updateAPIStatus ("GetWorksheet",true);
//		}
//	}
	int parseJSONStringToInt(string JSONstring){
		if ( (JSONstring).Length > 0) {
			return int.Parse (JSONstring);
		} else {
			return 0;
		} 
	}
	float parseJSONStringToFloat(string JSONstring){
		if ((JSONstring).Length > 0) {
			return float.Parse (JSONstring);
		} else {
			return 0f;
		}
	}
	void updatePreWorksheetView(){
		TopicGO.GetComponent<Text>().text  = userWorksheetPerfObject.Topic;
		ChapterGO.GetComponent<Text>().text  = userWorksheetPerfObject.Chapter;
		HighScoreGO.GetComponent<Text>().text  = ""+userWorksheetPerfObject.HighScore;
		DiamondsGO.GetComponent<Text>().text  = userWorksheetPerfObject.Diamonds+"/"+userWorksheetPerfObject.MaxDiamonds;
		AttemptsGO.GetComponent<Text>().text  = ""+userWorksheetPerfObject.Attempts;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
