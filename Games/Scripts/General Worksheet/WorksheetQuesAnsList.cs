using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WorksheetQuesAnsList: QuesAnsList{
	static int currentCount=0;
	int lives=0,maxLives=0;
	int totalScore;
	DifficultyWiseAllotmentList difficultyallotmentList;
	AttemptedQuesInfoList attemptedQuesList;
	//QuesAnsPair qaPair;
	List<QuesAnsPair> qaList = new List<QuesAnsPair>();

	//Adding question to QuesAnsList
	//Adding question to QuesAnsList
	public void add(string question,Answer givenAnswer,string hint, string answer_description, string question_image, int listIndex,  int backendId){
		QuesAnsPair quesAnsPair = new QuesAnsPair (question,givenAnswer, hint, answer_description, question_image, listIndex, backendId);
		QAList.Add(quesAnsPair);
	}
	//Getting current QuesAnsPair
	public QuesAnsPair getCurrentQuesAnsPair(){
		return qaList[currentCount];
	}
	//Setting currentCount
	public void setCurrentCount(int count){
		currentCount = count;
	}

	//After question is attempted
	public void postQuestionCalculations(int solved, float timeElapsed){
		QuesAnsPair quesAnsPair = getCurrentQuesAnsPair ();
		int difficulty_level = quesAnsPair.getDifficultyflag ();
		int index = quesAnsPair.getIndex ();
		int newLevel = 0;
		registerAttempt (index,solved,difficulty_level);

		if (solved == 2) {
			//Question solved incorrectly
			newLevel = correctQuesCalculations(difficulty_level);
		} else if (solved == 3) {
			//Question solved correctly
			newLevel = inCorrectQuesCalculations(difficulty_level);
		}

	}
	//If question is attempted correctly
	public int correctQuesCalculations(int difficulty_level){
		if (difficultyallotmentList.differenceByLevel (difficulty_level) > 0) {
			//User hasn't finished the quota of questions for current difficulty level.
			return difficulty_level;
		
		} else if (difficultyallotmentList.differenceByLevel (difficulty_level) == 0) {
			//User has just finished the quota of questions for current difficulty level.
			return incrementDifficultyLevel (difficulty_level);
		
		} else if (difficultyallotmentList.differenceByLevel (difficulty_level) < 0) {
			//User has already finished the quota of questions for current difficulty level.
			if (attemptedQuesList.lastSolvedLevelRepeat ()) {
				//User has answered 2 questions of current difficulty level consecutively correctly.
				return incrementDifficultyLevel (difficulty_level);
			} else {
				//User has answered first question of current difficulty level consecutively correctly.
				return difficulty_level;
			}
		} else {
			return difficulty_level;
		}
	}
	//If question is attempted incorrectly
	public int inCorrectQuesCalculations(int difficulty_level){
		if(attemptedQuesList.lastSolvedLevelRepeat()){
			//2 incorrect questions in a row
			return decrementDifficultyLevel(difficulty_level);
		}else{
			//First incorrect question
			return difficulty_level;
		}
	}
	public int incrementDifficultyLevel(int level){
		return level;
	}
	public int decrementDifficultyLevel(int level){
		return level;
	}
	public void registerAttempt(int index, int solved, int difficulty_level){
		difficultyallotmentList.registerAttempt (difficulty_level);
		attemptedQuesList.addAttemptedQues (index, solved, difficulty_level);
	}
}


