using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Answer  {
	int type;
	//type = (0,SCQ),(1,True/False), (2,FillBlanks)  

	//SCQ Variables
	string[] ansOptions, ansOptionImg; int optionCount=0; int correctOption=0;
	public int userSelectedOp{ get; set;}

	//TF Variables
	bool correctbool=true;
	bool userSelctedBool=true;

	//FillBlanks Variables
	string correctFillText ="";
	string userFilledText = "";
	public Answer(){

	}
	public Answer(string[] ansOps, int opCount, int correctOp, string[] ansOpImg){
		//SCQ type questions
		type = 0;
		ansOptions = new string[opCount];
		ansOptionImg = new string[opCount];
		ansOptionImg = ansOpImg;
		ansOptions = ansOps;
		optionCount = opCount;
		correctOption = correctOp;
		userSelectedOp = -1;
	}
	public Answer(bool corrBool){
		//True False type questions
		type = 1;
		correctbool = corrBool;
	}
	public Answer(string fillText){
		//Fill in the  blanks type questions
		type = 2;
		correctFillText = fillText;
	}
	public bool correctAnswer(int option){
		return (option == correctOption);
	}


	//Getter methods
	public int getType(){
		return type;
	}
	public List<string> getAnsOptions(){
		return new List<string> (ansOptions);

	}
	public List<string> getAnsOptionImgs(){
		return new List<string> (ansOptionImg);

	}
	public bool getCorrectBool(){
		return correctbool;
	}
	public string getCorrectFillText(){
		return correctFillText;
	}
	public int getCorrectOption(){
		return correctOption;
	}
}
