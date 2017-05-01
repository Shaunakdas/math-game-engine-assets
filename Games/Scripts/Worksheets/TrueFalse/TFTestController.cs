using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;

public class TFTestController : MonoBehaviour {

	public static readonly string domain = "localhost:3000";
	TFQAViewController qAObject;
	string getQAListJSONText,postQAAttemptText;
	// Use this for initialization
	void Start () {
		qAObject = (TFQAViewController) gameObject.GetComponent(typeof(TFQAViewController));
		StartCoroutine (getQAListNetworkCall());
	}
	public string getDomainAddress(){
		return domain;
	}

	// Update is called once per frame
	void Update () {

	}

	public void updateButtonContent(GameObject btnGameObject,string text, bool interact){
		//Set Button Text
		btnGameObject.GetComponentInChildren<Text>().text  = text;
		btnGameObject.GetComponent<Button> ().interactable = interact;

	}
	IEnumerator getQAListNetworkCall() {
		string getQuesListUrl;
		getQuesListUrl = getDomainAddress () + "/api/worksheet/get_worksheet?worksheet_id=4";

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
			getQAListJSONText = www.downloadHandler.text; 
			qAObject.setQAList (getQAListJSONText);
		}
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
			postQAAttemptText = www.text;
		}
		//StartCoroutine(PostQuestionAttempt(www));
	}
	IEnumerator WaitMethod() {
		Debug.Log("Before Waiting 2 seconds");
		yield return new WaitForSeconds(4);
		Debug.Log("After Waiting 2 Seconds");
	}
	public IEnumerator GetAttemptDetailsAPI() {
		string authorization = authenticate("education", "education");
		UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get (getDomainAddress()+"/api/diagnostic_tests/get_attempt_details?number="+UserProfile.number);
		www.SetRequestHeader("AUTHORIZATION", authorization);
		yield return www.Send ();

		if (www.isError) {
			Debug.Log ("GetAttemptDetailsAPI error"+www.error);
		} else {
			Debug.Log("GetAttemptDetailsAPI success"+ www.downloadHandler.text);
			var the_JSON_string = www.downloadHandler.text; 
			var N = JSON.Parse(the_JSON_string);

		}
	}
	string authenticate(string username, string password)
	{
		string auth = username + ":" + password;
		auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
		auth = "Basic " + auth;
		return auth;
	}
}
