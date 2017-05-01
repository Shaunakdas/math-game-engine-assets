using UnityEngine;
using System.Collections;

public class WorksheetScoreBoardController : MonoBehaviour {
	public GameObject worksheetGO;
	WorksheetController worksheetObject;
	// Use this for initialization
	void Start () {
		worksheetObject = (WorksheetController) worksheetGO.GetComponent(typeof(WorksheetController));	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
