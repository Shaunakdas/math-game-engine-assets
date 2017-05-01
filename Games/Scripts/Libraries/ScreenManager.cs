using UnityEngine;
using System.Collections;
//Not Needed
public class ScreenManager:MonoBehaviour{
	public static float widthMultiplier {
		get { return (Screen.width / 1080f); }
	}
	public static float heightMultiplier {
		get { return (Screen.height / 1920f); }
	}
	public static float scaledXSize(float value){
		return widthMultiplier * value;
	}
	public static float scaledYSize(float value){
		return heightMultiplier * value;
	}
}
