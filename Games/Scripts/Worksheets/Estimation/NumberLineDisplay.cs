using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberLineDisplay : MonoBehaviour {
	EstimationQAViewController estQAView;
	//GameObject
	public GameObject numberLineGrid,userSelectorScroll,correctSelectorScroll;

	//Prefabs
	public GameObject smallMarkerPF,numberMarkerPF;

	//ScreenValues
	public float screenDimensionHeight{get; set;}
	public float screenDimensionWidth{get; set;}
	public float screenPadding{get; set;}
	public float startYPosition{get; set;}

	//Question Values
	public int startInteger{get;set;}
	public int endInteger{get;set;}
	public int numberBreak{get;set;}

	//To be calculated
	public float cellWidth{get; set;}
	public float allocatedHeight{get; set;}
	public float cellHeight{get; set;}
	public int totalBreakerCount{get;set;}

	//Post Questions
	public float correctAnswer{get;set;}
	public float errorTolerance{get; set;}
	public float userSelectorYPosition{get; set;}
	public float correctSelectorYPosition{get; set;}


	public void defaultValues(){
		screenDimensionHeight=936f; screenDimensionWidth=520f;
		startInteger=0; endInteger=10; numberBreak=5; correctAnswer = 5f;
		screenPadding=30f; startYPosition = -420f;
		errorTolerance = 25f;
	}
	public void initNumberLineCalculations(){
		//To be calculated
		cellWidth = 40f;
		allocatedHeight = (screenDimensionHeight / 2) - screenPadding + Mathf.Abs(startYPosition);
		totalBreakerCount = 1+(numberBreak*(endInteger-startInteger));
		cellHeight = allocatedHeight/totalBreakerCount;
		correctSelectorYPosition = cellHeight * numberBreak* (correctAnswer - startInteger);
	}

	public void initNumberLineDisplay(){

		numberLineGrid.GetComponent<UIGrid> ().cellHeight = cellHeight;
		numberLineGrid.GetComponent<UIGrid> ().cellWidth = cellWidth;
		//First Number Marker
		GameObject firstNumberMarkerGO = (GameObject) InstantiateNGUIGO (numberMarkerPF,numberLineGrid.transform,"StartNumberMarker");
		firstNumberMarkerGO.GetComponentInChildren<TEXDrawNGUI> ().text = endInteger.ToString();
		for (int i = (endInteger-1); i > (startInteger-1); i--) {
			//First fill small markers
			for (int j = 0; j < numberBreak-1; j++) {
				GameObject smallMarkerGO = (GameObject) InstantiateNGUIGO (smallMarkerPF,numberLineGrid.transform,"SmallMarker_"+i+"_"+j);
			}
			//Fill Number Marker
			GameObject numberMarkerGO = (GameObject) InstantiateNGUIGO (numberMarkerPF,numberLineGrid.transform,"NumberMarker"+i);
			numberMarkerGO.GetComponentInChildren<TEXDrawNGUI> ().text = ( i).ToString();
		}
		numberLineGrid.GetComponent<UIGrid> ().Reposition ();



	}
	public void UserSelected(){
		userSelectorYPosition = userSelectorScroll.transform.position.y;
		animateCorrectPosition ();
	}
	public void animateCorrectPosition(){
		Debug.Log ("animateCorrectPosition"+correctSelectorScroll.transform.position.ToString ());
		correctSelectorScroll.SetActive (true);
		resetScrollViewPosition (correctSelectorScroll);
		SpringPanel.Begin (correctSelectorScroll, new Vector3(0f,correctSelectorYPosition), 2f);
		Debug.Log ("animateCorrectPosition after spring panel"+correctSelectorScroll.transform.position.ToString ());
		correctSelectorScroll.GetComponentInChildren<TEXDrawNGUI> ().text = correctAnswer.ToString();
		StartCoroutine(MyMethod());

	}
	public void resetScrollViewPosition(GameObject scrollGO){
		scrollGO.transform.position = new Vector3 (0f, 0f);
		scrollGO.GetComponent<UIPanel> ().clipOffset = new Vector2 (0f, 0f);
	}
	IEnumerator MyMethod() {
		Debug.Log("Before Waiting 2 seconds");
		yield return new WaitForSeconds(4);
		Debug.Log("After Waiting 2 Seconds");
		destroyChildGOList (numberLineGrid);
		resetScrollViewPosition (userSelectorScroll);
		correctSelectorScroll.SetActive (false);
		estQAView.AnswerSelected ();
	}
	public bool checkUserSelectorPosition(){
		return Mathf.Abs (userSelectorYPosition - correctSelectorYPosition) < errorTolerance;
	}

	// Use this for initialization
	void Start () {

		estQAView = gameObject.GetComponent<EstimationQAViewController> ();
		defaultValues ();
		correctSelectorScroll.SetActive (false);
		UIScrollView scroll = userSelectorScroll.GetComponent<UIScrollView> ();
		scroll.onStoppedMoving += UserSelected ;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	static public GameObject InstantiateNGUIGO(GameObject prefab, Transform parent, string name) {
		GameObject child = InstantiateNGUIGO (prefab, parent);
		child.name = name;
		return child;
		//		return (GameObject)Instantiate(prefab,parent);
	}
	static public GameObject InstantiateNGUIGO(GameObject prefab, Transform parent) {
		return NGUITools.AddChild(parent.gameObject,prefab);
		//		return (GameObject)Instantiate(prefab,parent);
	}
	/// <summary>
	/// Delete all child GameObjects of a given parent GameObject.
	/// </summary>
	/// <param name="GO">Parent GameObject</param>
	public virtual void destroyChildGOList(GameObject GO){
		foreach (Transform child in GO.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}
}
