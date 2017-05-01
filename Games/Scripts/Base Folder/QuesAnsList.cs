using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class QuesAnsList{

	//Display
	public int max_lives{get; set;} 
	public int user_lives{get; set;}
	public float user_score{get; set;}
	public float max_total_time{get; set;}
	public float user_total_time{get; set;}
	public int max_index{get; set;}
	static public int user_current_index{get; set;}
	public List<QuesAnsPair> QAList{get; set;}

	//Behind the scene
	public int DisplayId{get; set;}
	public int PersonalizationType{get; set;}
	DifficultyWiseAllotmentList difficultyallotmentList;
	AttemptedQuesInfoList attemptedQuesList;

	public QuesAnsList (){
		QAList = new List<QuesAnsPair>();
		max_lives = 0;user_lives = 0;user_score = 0f;max_total_time = 0f;user_total_time = 0f;max_index = 0;user_current_index=0;
	}
	//Adding question to QuesAnsList
	public void add(string question,Answer givenAnswer,string hint, string answer_description, string question_image, int listIndex,  int backendId){
		QuesAnsPair quesAnsPair = new QuesAnsPair (question,givenAnswer, hint, answer_description, question_image, listIndex, backendId);
		QAList.Add(quesAnsPair);
	}
	//Adding question to QuesAnsList for Answer Option
	public void add(string question,List<AnswerOption> ansOpList,string hint, string answer_description, string question_image, int listIndex,  int backendId, string questionStyle){
		QuesAnsPair quesAnsPair = new QuesAnsPair (question,ansOpList, hint, answer_description, question_image, listIndex, backendId, questionStyle);
		QAList.Add(quesAnsPair);
	}
	public void add(string question,List<AnswerOption> ansOpList,string hint, string answer_description, string question_image, int listIndex,  int backendId){
		QuesAnsPair quesAnsPair = new QuesAnsPair (question,ansOpList, hint, answer_description, question_image, listIndex, backendId);
		QAList.Add(quesAnsPair);
	}
	//Getting current QuesAnsPair
	public QuesAnsPair getCurrentQuesAnsPair(){
		return QAList[user_current_index];
	}
	public int getLives(){
		return user_lives;
	}
	public float getTotalScore(){
		return user_score;
	}
	public int getMaxIndex(){
		return QAList.Count;
	}
	public int getUserIndex(){
		return user_current_index;
	}
	public float getMaxTotalTime(){
		return max_total_time;
	}

	//Setting user_current_index
	public void setUserIndex(int count){
		user_current_index = count;
	}
	public void setUserTotalTime(int time){
		user_total_time = time;
	}
	public void setMaxParams (int maxLives, int maxTotalTime){
		max_lives = maxLives;
		max_total_time = maxTotalTime;
	}
	//Setting Lives
	public int decrementLives(int solved){
		if (solved == 2) {
			if (user_lives > 0) {
				user_lives--;
			}
		}
		return user_lives;
	}

	//Setting TotalScore
	public float addTotalScore(int solved, float timeElapsed){
		user_score += calcScore(solved,timeElapsed);
		return user_score;
	}


	//After question is attempted
	public bool postQuestionCalculations(int solved, float timeElapsed){
		QuesAnsPair quesAnsPair = getCurrentQuesAnsPair ();
		Debug.Log ("Setting attempt of question at index " + getUserIndex () + " with attempt = " + solved);
		quesAnsPair.userAttempt =solved;
		float currentScore = calcScore (solved, timeElapsed);
		quesAnsPair.setCurrentScore(currentScore);

		//Hints and total Score 


		//Next processes
		nextQuestionCalculations (solved, timeElapsed, currentScore);
		recordKeeping (solved, timeElapsed, currentScore);
		return true;
	}
	public void setUserTimeTaken( float timeElapsed){
		QuesAnsPair quesAnsPair = getCurrentQuesAnsPair ();
		quesAnsPair.setTimeTaken (timeElapsed);
	}

	//Calculations for current Score
	public float calcScore(int solved, float timeElapsed){
		Debug.Log("calcScore timeElapsed"+timeElapsed+solved);
		float maxTimeAllotted = getCurrentQuesAnsPair ().getMaxTimeAllotted ();
		float maxScore = getCurrentQuesAnsPair ().getMaxScore ();
		float score = 0f;
		Debug.Log ("Score calculations " + "maxTimeAllotted" + maxTimeAllotted + "timeElapsed" + timeElapsed + "maxScore" + maxScore + "timeElapsed" + timeElapsed);
		if (solved == 3) {
			Debug.Log("Numerator "+ ((29 * maxTimeAllotted) - (27 * timeElapsed)));
			Debug.Log("Denominator "+ (29 * maxTimeAllotted));
			Debug.Log("Denominator "+ (27 * timeElapsed));
			if (timeElapsed < (maxTimeAllotted / 3)) {
				score = maxScore;
			}else if((timeElapsed > (maxTimeAllotted / 3))&&(timeElapsed < (maxTimeAllotted))){
				score = ((29 * maxTimeAllotted - 27 * timeElapsed) * (maxScore)) / (20 * maxTimeAllotted);
			}else {
				score = maxScore / 10;
			}

			//score = (int)(29 * maxTimeAllotted - 27 * timeElapsed) * (maxScore) / (20 * timeElapsed);
		}
		Debug.Log ("Score calculated is"+score);
		return score;
	}
	//Calculations for next Question
	public void nextQuestionCalculations(int solved, float timeElapsed, float currentScore){
	}

	//Record Keeping work
	public void recordKeeping(int solved, float timeElapsed, float currentScore){
	}
}


