using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class AnswerOption  {
	int type;
	//type = (0,SCQ),(1,True/False), (2,FillBlanks)  ,(3,Comparision), (4,Equivalence), (5,Combination), (6,Estimation)

	//SCQ Variables
	public string optionText{ get; set;}
	public string optionImg{ get; set;}
	public bool correctFlag{ get; set;}
	public bool selectedFlag{ get; set; }
	public int optionId{ get; set; }

	//Comparision Variables
	//Using optionText,optionImg,correctFlag,optionId
	public int startOrder{get; set;}
	public int correctOrder{get; set;}
	public int inputOrder{get; set;}
	public string incorrectExplaination{ get; set; }

	//Combination
	public int correctMultiple{get; set;}
	//
	//For Analytics
	public float timeFromLastSelection {get; set;}

	public AnswerOption(string opTxt, string opImg, bool correct, int opId){
		//SCQ type questions
		optionImg = opImg;
		optionText = opTxt;
		correctFlag = correct;
		optionId = opId;
	}
	public AnswerOption(string opTxt, string opImg, int start, int correct, int opId){
		//Comparision type questions
		optionImg = opImg;
		optionText = opTxt;
		startOrder = start;
		correctOrder = correct;
		optionId = opId;
	}
}
