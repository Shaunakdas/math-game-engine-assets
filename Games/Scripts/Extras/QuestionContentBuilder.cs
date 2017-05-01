using UnityEngine;
using System.Collections;
using SimpleJSON;
using UnityEngine.UI;
using HtmlAgilityPack;

public class QuestionContentBuilder : MonoBehaviour {
	//Prefabs
	public GameObject StepQuestionLinePF;

	// Use this for initialization
	void Start () {
//		GameObject quesContentLine = Instantiate (StepQuestionLinePF,this.transform ) as GameObject;
//		quesContentLine.GetComponent<Text> ().text = "Finally, find out sum of predecessor and successor of 101";
		var html = new HtmlDocument();
		html.LoadHtml(@"<html><body><p>Lets find out predecessor of 101</p><p>i.e. Subtract 1 from 101</p><p> = 101-1</p><p>=<input id='i1'/></p></html></body>");
		Debug.Log (html.DocumentNode.SelectNodes("//body")[0].InnerHtml);

		HtmlNodeCollection nodes = html.DocumentNode.SelectNodes("//p");  
		foreach (HtmlNode item in nodes)  
		{  
			GameObject quesContentLine = Instantiate (StepQuestionLinePF,this.transform ) as GameObject;
			quesContentLine.GetComponent<Text> ().text = item.InnerHtml;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
