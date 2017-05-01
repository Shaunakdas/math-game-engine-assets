using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
public class StreamListAdapter : MonoBehaviour {
	public GameObject StreamElement;
	public GameObject TopicGridLayout;
	public GameObject TopicElement;
	public GameObject AttrElement;
	public GameObject TitleElement;
	public static readonly string domain = "localhost:3000";

	// Use this for initialization
	void Start () {
		GetStreamwiseScore();
	}
	public void GetStreamwiseScore(){
		string postQuesAttemptUrl = domain + "/api/homepage/get_streamwise_score_short";
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
		string json_string="";
		json_string = PostJson.ToString();
		Debug.Log ("PostQuesAttempt Json" + json_string);
		// convert json string to byte
		var formData = System.Text.Encoding.UTF8.GetBytes(json_string);

		www = new WWW(postQuesAttemptUrl, formData, postHeader);
		StartCoroutine(PostQuestionAttempt(www));
	}

	IEnumerator PostQuestionAttempt(WWW data){
		yield return data; // Wait until the download is done
		if (data.error != null){
			Debug.Log("There was an error sending request: " + data.error);
		}else{
			// Show results as text
			Debug.Log("WWW Response: " + data.text);
			var the_JSON_string =data.text; 
			var N = JSON.Parse(the_JSON_string);
//			var versionString = N["userPrepQuotient"].Value;        // versionString will be a string containing "1.0"
//			var versionNumber = N["streams"][0]["name"].Value;      // versionNumber will be a float containing 1.0
//			Debug.Log(versionNumber);

			var streamArrayLength = N ["streams"].Count;
			Debug.Log("streamArrayLength"+streamArrayLength);
			for (int i = 0; i < streamArrayLength; i++) {
				GameObject newStreamElement = Instantiate(StreamElement) as GameObject;
				newStreamElement.name = newStreamElement.name + " item at (" + i + ")";
				newStreamElement.transform.SetParent(GameObject.Find("StreamVerticalLayout").transform)	;
				newStreamElement.transform.Find("StreamTitle").GetComponent<Text>().text =N["streams"][i]["name"].Value ;

				GameObject newTopicGridLayout = Instantiate(TopicGridLayout) as GameObject;
				newTopicGridLayout.name = newTopicGridLayout.name + " item at (" + i + ")";
				newTopicGridLayout.transform.SetParent(GameObject.Find("StreamVerticalLayout").transform)	;

				var topicArrayLength = N ["streams"][i]["topics"].Count;
				Debug.Log("topicArrayLength"+topicArrayLength);
				for (int j = 0; j < topicArrayLength; j++) {
					string topic_name = N ["streams"] [i] ["topics"] [j] ["name"].Value;
					string topic_score =""; string topic_diamonds ="";
					Debug.Log ("IF scorecard exists"+ N ["streams"] [i] ["topics"] [j] ["scorecard"] ["score"].Value.Length);
					if (N ["streams"] [i] ["topics"] [j] ["scorecard"] ["score"].Value.Length >0) {
						topic_score = N ["streams"] [i] ["topics"] [j] ["scorecard"] ["score"].Value;
						topic_diamonds = N ["streams"] [i] ["topics"] [j] ["scorecard"] ["diamonds"].Value;
					}
					GameObject newTopicElement = Instantiate(TopicElement) as GameObject;
					newTopicElement.name = newTopicElement.name + " item at (" + i +" + "+ j + ")";
					newTopicElement.transform.SetParent(GameObject.Find(newTopicGridLayout.name).transform)	;
					newTopicElement.transform.Find("TitleElement").GetComponent<Text>().text =topic_name;


					GameObject newAttrElement = Instantiate(AttrElement) as GameObject;
					newAttrElement.name = newAttrElement.name + " HighScore at (" + i +" + "+ j + ")";
					newAttrElement.transform.SetParent(GameObject.Find(newTopicElement.name).transform)	;
					newAttrElement.transform.GetComponent<Text>().text ="HIGH SCORE : "+topic_score ;

					newAttrElement = Instantiate(AttrElement) as GameObject;
					newAttrElement.name = newAttrElement.name + " Diff at (" + i +" + "+ j + ")";
					newAttrElement.transform.SetParent(GameObject.Find(newTopicElement.name).transform)	;
					newAttrElement.transform.GetComponent<Text>().text ="DIAMONDS : "+topic_diamonds ;

				}
			}




		}
	}

	// Update is called once per frame
	void Update () {
	
	}
	public void openWorksheet(){
		SceneManager.LoadScene("GeneralWorksheet");
	}
}
