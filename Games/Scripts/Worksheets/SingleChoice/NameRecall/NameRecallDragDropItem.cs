using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameRecallDragDropItem : UIDragDropItem {
	public GameObject rootGO;
	public int answerOpIndex{get; set;}
	public GameObject questionContainer;
	protected override void OnDragDropRelease (GameObject surface)
	{
		Debug.Log ("surface"+surface.name);
		if (questionContainer == surface){
			gameObject.GetComponent<UISprite> ().alpha = 0f;
			rootGO.GetComponent<NameRecallQAViewController> ().AnswerSelected (answerOpIndex);

		}
		base.OnDragDropRelease (surface);
	}

}
