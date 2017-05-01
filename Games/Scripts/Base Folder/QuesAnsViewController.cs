using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;


public class QuesAnsViewController : MonoBehaviour {
	int currentCount =0;
	QuesAnsList quesAnsList ;
	QuesAnsPair currQuesAnsPair;
	Animator anim;


	//Constructor
	public virtual void Start () {
		quesAnsList = new QuesAnsList();
		quesAnsList.setUserIndex(currentCount);
		currQuesAnsPair = quesAnsList.getCurrentQuesAnsPair ();
		entryAnim();
	}

	//Getter methods

	public virtual int getCurrentCount(){
		return currentCount;
	}
	public virtual QuesAnsList getQuesAnsList(){
		return quesAnsList;
	}
	public virtual QuesAnsPair getCurrQuesAnsPair(){
		return currQuesAnsPair;
	}
	//Setter Methods
	public virtual void getQAListCallFinished(){
	}
	public virtual void setQAList(string qaJSONText){
	}
	public virtual void setQAList(){
	}
	public virtual string postQAAttempt(){
		return "";
	}

	//Animation methods
	public virtual void entryAnim(){
		//For entry animation
	}
	public virtual void exitAnim(){
		//For exit animation
	}
	public virtual void questionEntryAnim(){
		//For entry animation
	}
	public virtual void questionExitAnim(){
		//For exit animation
	}
	public virtual void answerOptionEntryAnim(){
		//For entry animation
	}
	public virtual void answerOptionExitAnim(){
		//For exit animation
	}
	public virtual void correctAnsAnim(){
		//For correct answer animation
	}
	public virtual void incorrectAnsAnim(){
		//For incorrect animation
	}

	//Setting Question and Answer Views
	public virtual void setQuesAnsBasedOnIndex(int index){
	}
	public  virtual void setQuesView(QuesAnsPair currQuesAnsPair){
	}
	/// <summary>
	/// Get Question Text of current Question Answer Pair
	/// </summary>
	public virtual string getQuestionText(QuesAnsPair currQuesAnsPair){
		return  StringWrapper.changeString (currQuesAnsPair.getQuesText ());
	}
	public virtual void setAnsOpView(QuesAnsPair currQuesAnsPair){
	}
	/// <summary>
	/// Get Text of answer option at given index of current Question Answer Pair
	/// </summary>
	public virtual string getAnswerOptionText(QuesAnsPair currQuesAnsPair, int answerIndex){
		return  StringWrapper.changeString(currQuesAnsPair.ansOptionList[answerIndex].optionText);
	}
	/// <summary>
	/// Get Image url of answer option at given index of current Question Answer Pair
	/// </summary>
	public virtual string getAnswerOptionImageUrl(QuesAnsPair currQuesAnsPair, int answerIndex){
		return  currQuesAnsPair.ansOptionList[answerIndex].optionImg;
	}
	/// <summary>
	/// Set Image texture of given image Gameobject based on given texture
	/// </summary>
	public void setImageTexture (GameObject itemImageGO, Texture2D itemTexture){
		Image img = itemImageGO.GetComponent<Image>();
		img.sprite = Sprite.Create (itemTexture, new Rect (0, 0, itemTexture.width, itemTexture.height),Vector2.zero);
		LayoutElement layout = itemImageGO.GetComponent<LayoutElement>();
		layout.minWidth = 1.5f*itemTexture.width; layout.minHeight = 1.5f*itemTexture.height;
		RectTransform rt = itemImageGO.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(1.5f*itemTexture.width, 1.5f*itemTexture.height);
	}
	IEnumerator LoadImage(string @Url,GameObject QAGameObject){
		WWW www = new WWW(Url);
		yield return www;
	}
	//On Answer Submission
	public virtual void AnswerSelected(int buttonNo){
	}
	public virtual void QuesEndAnimation(int increment,int updated){
	}
	public virtual void changeQuestionIndex(int increment,int updated){
	}
	/// <summary>
	/// Check if answer Text in given QuesAnsList is correct or not
	/// </summary>
	public virtual bool answerValidated(QuesAnsList currQuesAnsList,int answerIndex){
		return currQuesAnsList.getCurrentQuesAnsPair ().ansOptionList[answerIndex].correctFlag;
	}
	/// <summary>
	/// Check if answer option at given index in given QuesAnsList is correct or not
	/// </summary>
	public virtual bool answerValidated(QuesAnsList currQuesAnsList,string userText){
		return (userText == currQuesAnsList.getCurrentQuesAnsPair ().ansOptionList[0].optionText);
	}
	/// <summary>
	/// Check if answer option at given index in given QuesAnsList is correct or not
	/// </summary>
	public virtual int getSolutionFlag(QuesAnsList currQuesAnsList,int answerIndex){
		if (answerValidated(currQuesAnsList,answerIndex)) {
			return 3;
		} else {
			return 2;
		}
	}
	/// <summary>
	/// Check if answer option text in given QuesAnsList is correct or not
	/// </summary>
	public virtual int getSolutionFlag(QuesAnsList currQuesAnsList,string userText){
		if (answerValidated(currQuesAnsList,userText)) {
			return 3;
		} else {
			return 2;
		}
	}
	public virtual bool answerValidated(){
		return true;
	}
	/// <summary>
	/// Set Selection Flag of answer option at given index in given QuesAnsList
	/// </summary>
	public virtual void  setSelectionflag(QuesAnsList currQuesAnsList,int answerIndex){
		currQuesAnsList.getCurrentQuesAnsPair ().ansOptionList [answerIndex].selectedFlag = true;
	}
	public virtual void submitAnswer(){
		if (answerValidated()) {
			correctAnsAnim ();
		} else {
			incorrectAnsAnim ();
		}
		exitAnim ();
		increaseCount ();
		entryAnim ();
	}
	public virtual void increaseCount(){
		currentCount++;
		quesAnsList.setUserIndex (currentCount);
		currQuesAnsPair = quesAnsList.getCurrentQuesAnsPair ();
	}


	public virtual void Update(){

	}
	//Base methods
	/// <summary>
	/// Get Child GameObject of given name
	/// </summary>
	/// <param name="parentGameObject">Parent GameObject</param>
	/// <param name="withName">Name of child GameObject</param>
	static public GameObject getChildGameObject(GameObject parentGameObject, string withName) {
		//Author: Isaac Dart, June-13.
		Transform[] ts = parentGameObject.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
		return null;
	}
	/// <summary>
	/// Instantiate NGUI Child GameObject with a given prefab and parent transform
	/// </summary>
	/// <param name="parent">Parent Transform</param>
	/// <param name="prefab">Prefab for child GameObject</param>
	static public GameObject InstantiateNGUIGO(GameObject prefab, Transform parent) {
		return NGUITools.AddChild(parent.gameObject,prefab);
		//		return (GameObject)Instantiate(prefab,parent);
	}
	/// <summary>
	/// Instantiate NGUI Child GameObject with a given prefab, parent transform and a given name
	/// </summary>
	/// <param name="parent">Parent Transform</param>
	/// <param name="prefab">Prefab for child GameObject</param>
	static public GameObject InstantiateNGUIGO(GameObject prefab, Transform parent, string name) {
		GameObject child = InstantiateNGUIGO (prefab, parent);
		child.name = name;
		return child;
		//		return (GameObject)Instantiate(prefab,parent);
	}
	/// <summary>
	/// Instantiate Unity GUI Child GameObject with a given prefab and parent transform
	/// </summary>
	/// <param name="parent">Parent Transform</param>
	/// <param name="prefab">Prefab for child GameObject</param>
	static public GameObject InstantiateUnityGO(GameObject prefab, Transform parent) {
		return (GameObject)Instantiate(prefab,parent);
		//		return (GameObject)Instantiate(prefab,parent);
	}
	/// <summary>
	/// Delete all GameObjects in a given list and clear list also.
	/// </summary>
	/// <param name="GOList">List of GameObjects</param>
	public virtual void destroyGOList(List<GameObject> GOList){
		GOList.ForEach (imageObject => Destroy (imageObject));
		GOList.Clear ();
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
	/// <summary>
	/// Get size of text based on its length.
	/// </summary>
	/// <param name="text">text</param>
	public virtual float getTextSize(string text){
		TextGenerationSettings settings = new TextGenerationSettings();
		settings.textAnchor = TextAnchor.MiddleCenter;
		settings.color = Color.red;
		settings.generationExtents = new Vector2(500.0F, 200.0F);
		settings.pivot = Vector2.zero;
		settings.richText = true;
		settings.font = Resources.Load<Font>("Arial");
		settings.fontSize = 70;
		settings.fontStyle = FontStyle.Normal;
		settings.verticalOverflow = VerticalWrapMode.Overflow;
		TextGenerator generator = new TextGenerator();
		generator.Populate(text, settings);
		float width = generator.GetPreferredWidth(text, settings);
		Debug.Log("Preferred width of " +text +" :"+ width);
		return width;
	}
	/// <summary>
	/// Get random text generated from a list of all small cap letters.
	/// </summary>
	public static char GetRandomLetter()
	{
		string chars = "abcdefghijklmnopqrstuvwxyz";
		System.Random rand = new System.Random();

		int num = rand.Next(0, chars.Length -1);
		return chars[num];
	}
}
