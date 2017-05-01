using System;
using System.Collections;
using System.Collections.Generic;
public class AttemptedQuesInfoList{
	List<AttemptedQuesInfo> attemptedQuesList= new List<AttemptedQuesInfo>();
	public void addAttemptedQues (int index,int solved,int difficulty){
		attemptedQuesList.Add(new AttemptedQuesInfo(index,solved,difficulty));
	}
	public bool lastSolvedLevelRepeat(){
		return false;
	}

}


