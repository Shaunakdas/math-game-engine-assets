using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
public class GetClassOptions : MonoBehaviour {
	public static readonly string domain = "https://www.hotelashokachomu.com";
	List<Standard> standard_list = new List<Standard>();
	List<Stream> stream_list = new List<Stream>();
	UserProfile user;
	public GameObject diagnosticTestGO,titleGO,beginTitleGO,GCOPBtnGO,GADBtnGO;
	//GameObjects
	public GameObject StandardDropdownGO,StreamDropdownGO;
	DiagnosticTestController diagnosticTestObject;
	// Update is called once per frame
	void Update () {
	
	}


	void Start () {
		Debug.Log("GetClassOptions Start called");
		GCOPBtnGO.SetActive(false);
		StandardDropdownGO.SetActive(false);
		StreamDropdownGO.SetActive (false);
		GADBtnGO.SetActive (true);
		//StartCoroutine(GetStandard());
		diagnosticTestObject = (DiagnosticTestController) diagnosticTestGO.GetComponent(typeof(DiagnosticTestController));
		diagnosticTestObject.personalization_type = 0;
		if (UserProfile.firstName != null) {
			GameObject.Find("FirstNameInput").GetComponent<InputField>().text = UserProfile.firstName;
			//GameObject.Find("LastNameInput").GetComponent<InputField>().text = UserProfile.lastName;
			//GameObject.Find("EMailInput").GetComponent<InputField>().text = UserProfile.email;
			GameObject.Find("NumberInput").GetComponent<InputField>().text = UserProfile.number;
		}
	}
	string authenticate(string username, string password)
	{
		string auth = username + ":" + password;
		auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
		auth = "Basic " + auth;
		return auth;
	}
	public void getAttemptDetails(int input){
		if (checkInputDetails()) {
			if (input == 1) {
				submitPersonalDetails ();
			}
			StartCoroutine (diagnosticTestObject.GetAttemptDetailsAPI ());
		}
	}
	bool checkInputDetails(){

		string number = GameObject.Find("NumberInput").GetComponent<InputField>().text;
		if (number.Length ==10) {
			return true;
		} else {
			titleGO.GetComponent<Text> ().text = "Your mobile number is incorrect";
		}

		return false;
	}
//	IEnumerator GetAttemptDetailsAPI() {
//		string authorization = authenticate("education", "education");
//		UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get (domain+"/api/diagnostic_tests/get_attempt_details?number="+UserProfile.number);
//		www.SetRequestHeader("AUTHORIZATION", authorization);
//		yield return www.Send ();
//
//		if (www.isError) {
//			Debug.Log ("GetAttemptDetailsAPI error"+www.error);
//		} else {
//			GADBtnGO.SetActive (false);
//			Debug.Log("GetAttemptDetailsAPI success"+ www.downloadHandler.text);
//			var the_JSON_string = www.downloadHandler.text; 
//			var N = JSON.Parse(the_JSON_string);
//			if (N ["personalized"]["count"].Value != "0") {
//				//Transition to next page
//				diagnosticTestObject.personalization_type = 1;
//				string personalized_comment = N ["personalized"]["comment"].Value;
//				UserProfile.userStandard = getStandard(N["standards"]);
//				beginTitleGO.GetComponent<Text> ().text = personalized_comment;
//				diagnosticTestObject.openBeginPanel ();
//
//			} else {
//				diagnosticTestObject.personalization_type = 0;
//				if (N ["attempted"]["flag"].Value == "1") {
//					//Getting attempted comment
//					string attempted_comment = N ["attempted"]["comment"].Value;
//					titleGO.GetComponent<Text> ().text = attempted_comment;
//				} 
//				//for loop on standardCount
//				//add standard from API
//				standard_list.Clear();
//				standard_list = populateStandardList (N, standard_list);
//				standard_list.Sort ();
//				stream_list.Clear();
//				stream_list = populateStreamList (N["standards"][0], stream_list);
//				stream_list.Sort ();
//				//standardList.sort
//				GCOPBtnGO.SetActive(true);
//				StandardDropdownGO.SetActive(true);
//				StreamDropdownGO.SetActive (true);
//				populateStandardDropdown (standard_list);
//				populateStreamDropdown (stream_list);
//				diagnosticTestObject.updateAPIStatus ("GetStandard", true);
//				//Make dropdown visible
//			}
//
//		}
//	}	
	public void populateDropdownOptions(string JSONstring){
		var N = JSON.Parse(JSONstring);
		standard_list.Clear();
		standard_list = populateStandardList (N, standard_list);
		standard_list.Sort ();
		stream_list.Clear();
		stream_list = populateStreamList (N["standards"][0], stream_list);
		stream_list.Sort ();
		//standardList.sort
		GCOPBtnGO.SetActive(true);
		StandardDropdownGO.SetActive(true);
		StreamDropdownGO.SetActive (true);
		populateStandardDropdown (standard_list);
		populateStreamDropdown (stream_list);
		diagnosticTestObject.updateAPIStatus ("GetStandard", true);
	}
//	IEnumerator GetStandard() {
//		string authorization = authenticate("education", "education");
//		UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get ( domain+"/api/standards/get_standards");
//		www.SetRequestHeader("AUTHORIZATION", authorization);
//		yield return www.Send ();
//
//		if (www.isError) {
//			Debug.Log ("GetStandard error"+www.error);
//		} else {
//			Debug.Log("GetStandard success"+ www.downloadHandler.text);
//			var the_JSON_string = www.downloadHandler.text; 
//			var N = JSON.Parse(the_JSON_string);
//			//for loop on standardCount
//			//add standard from API
//			standard_list.Clear();
//			standard_list = populateStandardList(N,standard_list);
//			standard_list.Sort ();
//			//standardList.sort
//			populateStandardDropdown (standard_list);
//			diagnosticTestObject.updateAPIStatus ("GetStandard",true);
//			//Make dropdown visible
//
//
//		}
//	}
	//Populating Standard Lust from API Response JSON
	public List<Standard> populateStandardList(JSONNode N,List<Standard> std_list){
		for (int i = 0; i < N ["standards"].Count; i = i + 1) {
			std_list.Add (getStandard(N ["standards"] [i]));
		}
		return std_list;
	}
	public Standard getStandard(JSONNode N){
		Debug.Log (N[0].ToString());
		int standard_number = int.Parse(N ["standard_number"].Value);
		int standard_id = int.Parse(N ["standard_id"].Value);
		int subject_id = int.Parse(N ["subject_id"].Value);
		string standard_name = N ["name"].Value;
		return new Standard (standard_id, subject_id, standard_number, standard_name);
	}
	//Populating Standard Lust from API Response JSON
	public List<Stream> populateStreamList(JSONNode N,List<Stream> stream_list){
		for (int i = 0; i < N ["streams"].Count; i = i + 1) {
			stream_list.Add (getStream(N ["streams"] [i]));
		}
		return stream_list;
	}
	public Stream getStream(JSONNode N){
		int stream_id = int.Parse(N ["id"].Value);
		string stream_name = N ["name"].Value;
		return new Stream (stream_id, stream_name);
	}
	//Populating Dropdown from Standard List
	public void populateStandardDropdown(List<Standard> std_list){
		//Add standard to Dropdown
		var dropdown = StandardDropdownGO.GetComponent<Dropdown>();

		dropdown.options.Clear();
		foreach (Standard option in std_list)
		{
			Debug.Log("Adding standard option to dropdown"+option.standard_name);
			dropdown.options.Add(new Dropdown.OptionData(option.standard_name));
		}
	}
	//Populating Dropdown from Stream List
	public void populateStreamDropdown(List<Stream> stream_list){
		//Add standard to Dropdown
		var dropdown = StreamDropdownGO.GetComponent<Dropdown>();

		dropdown.options.Clear();
		foreach (Stream option in stream_list)
		{
			Debug.Log("Adding stream option to dropdown"+option.name);
			dropdown.options.Add(new Dropdown.OptionData(option.name));
		}
	}
	public void selectDropdownOption(){
		//Event to be called when dropdown selected option is changed
	}
	//Submit Personal Details
	public void submitPersonalDetails(){
		//Event to be called when standard is submitted
		string first_name = GameObject.Find("FirstNameInput").GetComponent<InputField>().text;
		//string last_name = GameObject.Find("LastNameInput").GetComponent<InputField>().text;
		string last_name = "Resonance-PCCP";
		string number = GameObject.Find("NumberInput").GetComponent<InputField>().text;
		//string email = GameObject.Find("EMailInput").GetComponent<InputField>().text;
		string email = number + "@resopccp.com";
		user = new UserProfile (first_name, last_name, email, number);
	}
	//Submit all details from user input
	public void submitStandard(){
		submitPersonalDetails ();

		var dropdown = StandardDropdownGO.GetComponent<Dropdown>();
		Debug.Log("Standard Dropdown index is "+dropdown.value);
		UserProfile.userStandard = standard_list[dropdown.value ];
		Debug.Log("Standard Dropdown id is "+UserProfile.userStandard.id);

		var stream_dropdown = StreamDropdownGO.GetComponent<Dropdown>();
		Debug.Log("Stream Dropdown index is "+stream_dropdown.value);
		UserProfile.stream = stream_list[stream_dropdown.value ];
		Debug.Log("Stream Dropdown id is "+UserProfile.stream.id);
	}
	public void updateButtonContent(string gameObjectName,string text, bool interact){
		//Set Button Text
		var beginbutton = GameObject.Find(gameObjectName).GetComponent<Button> ();
		beginbutton.GetComponentInChildren<Text>().text  = text;
		beginbutton.interactable = interact;
	}
	public void endOfScreen(){
		StandardDropdownGO.SetActive (false);
		GADBtnGO.SetActive (true);
		GCOPBtnGO.SetActive(false);
		StreamDropdownGO.SetActive(false);
		titleGO.GetComponent<Text> ().text = "Before we begin, what should I call you?";
	}
}
