using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class QuesAnsPair: IComparable<QuesAnsPair>{

	//Display variables
	public string question_text{get; set;}
	public string hint_text{get; set;}
	public string ans_desc{get; set;}
	public string ques_image{get; set;}
	public string ques_style{ get; set;}
	Answer answer;
	public float max_score{get; set;}
	public float user_score{get; set;}
	public float max_time_allotted{get; set;}
	public float user_time_taken{get; set;}



	//Behind the scene
	public int BackendId{get; set;}
	public int index{get; set;}
	//Difficulty Flag = 0-Easy,1-Medium,2-Hard
	int difficulty_flag=0;
	//Solved Flag = 0-Not displayed, 1-Displayed andQuesAnsPair Skipped, 2-solved incorrectly, 3-solved correctly
	public int userAttempt{get; set;}

	public string getQuesText(){
		return question_text;
	}
	public string getQuesImage(){
		return ques_image;
	}
	public Answer getAnswer(){
		return answer;
	}
	public string getHintText(){
		return hint_text;
	}
	public string getAnsDesc(){
		return ans_desc;
	}
	public int getIndex(){
		return index;
	}
	public int getDifficultyflag(){
		return difficulty_flag;
	}
	public float getMaxScore(){
		return max_score;
	}
	public float getMaxTimeAllotted(){
		return max_time_allotted;
	}
	public float getUserTimeTaken(){
		return user_time_taken;
	}
	public float getUserScore(){
		return user_score;
	}
	public void setQuesText(string text){
		question_text = text;
	}
	public void setAnswer(Answer trialAnswer){
		answer = trialAnswer;
	}
	public void setHintText(string text){
		hint_text = text;
	}
	public void setAnsDesc(string text){
		ans_desc = text;
	}
	public void setCurrentScore(float score){
		user_score = score;
	}
	public void setTimeTaken(float time){
		Debug.Log("QuesAnsPair time taken "+user_time_taken+"question_text"+question_text);
		user_time_taken = time;
	}
	public void initDisplayValues(){
		max_score = 1000f;user_score = 0;max_time_allotted = 90f;user_time_taken = 0;
		question_text="";hint_text="";ans_desc = "";ques_image = "";
	}
	public QuesAnsPair(string question,Answer givenAnswer,string hint, string answer_description,string question_image, int listIndex, int backendId){
		initDisplayValues ();
		question_text = question;
		answer = givenAnswer;
		hint_text = hint;
		ans_desc = answer_description;
		index = listIndex;
		BackendId = backendId;
		ques_image = question_image;
	}
	public QuesAnsPair(){
		initDisplayValues ();
	}
	public int CompareTo(QuesAnsPair other)
	{
		if(other == null)
		{
			return 1;
		}

		//Return the difference in power.
		return other.getIndex() - getIndex() ;
	}
	//For AnswerOption 
	public List<AnswerOption> ansOptionList{get; set;}
	public List<AnswerOption> selectedOpList{get; set;}
	public QuesAnsPair(string question,List<AnswerOption> ansOpList,string hint, string answer_description,string question_image, int listIndex, int backendId){
		initDisplayValues ();
		question_text = question;
		ansOptionList = new List<AnswerOption>();selectedOpList = new List<AnswerOption>();
		ansOptionList = ansOpList;
		hint_text = hint;
		ans_desc = answer_description;
		index = listIndex;
		BackendId = backendId;
		ques_image = question_image;
		userAttempt = 0;
		ques_style = "ShortChoice";
	}
	public QuesAnsPair(string question,List<AnswerOption> ansOpList,string hint, string answer_description,string question_image, int listIndex, int backendId, string questionStyle){
		initDisplayValues ();
		question_text = question;
		ansOptionList = new List<AnswerOption>();selectedOpList = new List<AnswerOption>();
		ansOptionList = ansOpList;
		hint_text = hint;
		ans_desc = answer_description;
		index = listIndex;
		BackendId = backendId;
		ques_image = question_image;
		userAttempt = 0;
		ques_style = questionStyle;
	}

}
