using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;

public class DiagnosticTestController : MonoBehaviour {
	public GameObject getClassOptionsGO, beginGO, qAGO, userResultGO,userResultPanelGO,URPAgainBtnGO,titleGO;

	public static readonly string domain = "https://www.hotelashokachomu.com";
	public int personalization_type {get; set;}
	GetClassOptions getClassOptionsObject;
	BeginPanelHandler beginObject;
	DiagQAViewController qAObject;
	StreamResultListViewController userResultListObject;
	Animator anim;
	public GameObject GCOPBeginBtnGO,BPBeginBtnGO,RFEPBeginBtnGO,beginTitleGO;
	int beginPanelHash,duringPanelHash, waitingPanelHash, explainerPanelHash, openResultPanelHash;
	string getQAListJSONText,postQAAttemptText;
	// Use this for initialization
	void Start () {
		Debug.Log ("DiagnosticTestController start() called");
		//Initiate animation components
		anim = GetComponent<Animator>();
//		beginPanelHash = Animator.StringToHash("beginTrigger");
//		duringPanelHash = Animator.StringToHash("duringTrigger");
//		waitingPanelHash = Animator.StringToHash("waitingTrigger");
//		explainerPanelHash = Animator.StringToHash("explainerTrigger");
//		openResultPanelHash = Animator.StringToHash("openResultTrigger");

		//Initiate script components of child gameobjects
		getClassOptionsObject = (GetClassOptions) getClassOptionsGO.GetComponent(typeof(GetClassOptions));
		beginObject = (BeginPanelHandler) beginGO.GetComponent(typeof(BeginPanelHandler));
		qAObject = (DiagQAViewController) qAGO.GetComponent(typeof(DiagQAViewController));
		userResultListObject = (StreamResultListViewController) userResultPanelGO.GetComponent(typeof(StreamResultListViewController));
		//GetClassOptions userResultObject = (GetClassOptions) userResultGO.GetComponent(typeof(GetClassOptions));
	}
	public string getDomainAddress(){
		return domain;
	}
	//Calling GetClassOptionsPanel
	public void openGetClassOptionsPanel (){
		anim.Play("GetClassOptionsState");
	}
	//Calling BeginPanel
	public void openBeginPanel (){
		
		//Called when button is pressed in GetClassOptionsPanel
		anim.Play("BeginState");
		//Calling API call for qAObject
		StartCoroutine(getQAListNetworkCall());
		//updateButtonContent (GCOPBeginBtnGO, "Waiting for options!!", false);
		getClassOptionsObject.endOfScreen ();
	}

	//Calling DuringPanel
	public void openDuringPanel (){
		//Called when button is pressed in BeginPanel
		anim.Play("DuringState");
		updateButtonContent (BPBeginBtnGO, "Looking for an easy test!", false);
	}

	//Calling WaitingPanel
	public void openWaitingPanel (){
		//Called when final answer button is pressed in DuringPanel
		anim.Play("WaitingState");
		StartCoroutine(WaitMethod());
}

	//Calling ResultFormatExplainerPanel
	public void openResultFormatExplainerPanel (){
		//Called when button is pressed in openWaitingPanel

		anim.Play("ResultExplainerState");
		StartCoroutine (postQAListNetworkCall (qAObject.postQAAttempt()));

	}

	//Calling ResultFormatExplainerPanel
	public void reviewDuringPanel (){
		//Called when button is pressed in openWaitingPanel
//		anim.SetTrigger (duringPanelHash);
		anim.Play("DuringState");

	}

	//Calling UserResultPanel
	public void openResultPanel (){
		//Called when button is pressed in ResultFormatExplainerPanel
//		anim.SetTrigger (openResultPanelHash);
		anim.Play ("OpenResultState");
		userResultListObject.setQAList (qAObject.getQAList ());
		userResultListObject.setResultBarLayout ();
	}

	//Calling New Streamwise Scene
	public void openStreamwiseScene (){
		//Called when button is pressed in UserResultPanel
		userResultListObject.destroyResultBarLayout ();
		anim.Play("GetClassOptions");
		//SceneManager.LoadScene("StreamwiseScreen");
		updateButtonContent (RFEPBeginBtnGO, "Calculating your score", false);
	}
	public void generatePersonalizedTest(){
		userResultListObject.destroyResultBarLayout ();
		StartCoroutine (GetAttemptDetailsAPI());
	}
	public void getPersonalizedTestCount(int test){
		if (test > 0) {
			URPAgainBtnGO.SetActive (true);
		}
	}




	// Update is called once per frame
	void Update () {
	
	}
	public void updateAPIStatus (string APIMethodName,bool success){
		switch (APIMethodName)
		{
		case "GetStandard":
			updateButtonContent (GCOPBeginBtnGO, "Get Started", success);
			break;
		case "GetQAList":
			updateButtonContent (BPBeginBtnGO, "Lets start the test!", success);
			break;
		case "PostQuestionAttempt":
			
			updateButtonContent (RFEPBeginBtnGO, "Get Results", success);
			break;
		
		}
	}

	public void updateButtonContent(GameObject btnGameObject,string text, bool interact){
		//Set Button Text
		btnGameObject.GetComponentInChildren<Text>().text  = text;
		btnGameObject.GetComponent<Button> ().interactable = interact;

	}
	IEnumerator getQAListNetworkCall() {
		var standard_id = UserProfile.userStandard.id;
		var diagnostic_test_id = 1;
		string getQuesListUrl;
		if(personalization_type == 1){
			getQuesListUrl = getDomainAddress () + "/api/diagnostic_tests/get_test.json?standard_id="+standard_id+"&subject_id="+standard_id+"&diagnostic_test_id="+diagnostic_test_id+"&personalization_type=1&number="+UserProfile.number;
		}else{
			getQuesListUrl = getDomainAddress() + "/api/diagnostic_tests/get_test.json?standard_id="+standard_id+"&subject_id="+standard_id+"&diagnostic_test_id="+diagnostic_test_id+"&stream_id="+UserProfile.stream.id;
		}
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
			updateAPIStatus ("GetQAList", true);
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
			updateAPIStatus ("PostQuestionAttempt", true);
			userResultListObject.setUserStreamResultList (www.text);
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
			if (N ["personalized"]["count"].Value != "0") {
				//Transition to next page
				personalization_type = 1;
				string personalized_comment = N ["personalized"]["comment"].Value;
				UserProfile.userStandard = getClassOptionsObject.getStandard(N["standards"]);
				beginTitleGO.GetComponent<Text> ().text = personalized_comment;
				openBeginPanel ();

			} else {
//				openGetClassOptionsPanel ();
				personalization_type = 0;

				if (N ["attempted"]["flag"].Value == "1") {
//					//Getting attempted comment
//					string attempted_comment = N ["attempted"]["comment"].Value;
					titleGO.GetComponent<Text> ().text = N ["attempted"]["comment"].Value;
				} 
				getClassOptionsObject.populateDropdownOptions (N.ToString());
//				//Make dropdown visible
			}

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
