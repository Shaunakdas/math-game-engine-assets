using UnityEngine;
using System.Collections;

public class UserEntityResult {
	//Behind the scene
	public string UserResultTitle{get; set;}
	public string EntityTitle{get; set;}
	public int UserResultValue{get; set;}
	public int UserResultMaxValue{get; set;}

	public UserEntityResult(){
		UserResultTitle = "";
		EntityTitle = "";
		UserResultValue = 0;
		UserResultMaxValue = 1000;
	}
}
