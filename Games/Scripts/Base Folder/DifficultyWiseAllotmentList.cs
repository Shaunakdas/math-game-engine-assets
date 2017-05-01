using System;
using System.Collections;
using System.Collections.Generic;
public class DifficultyWiseAllotmentList{
	List<DifficultyWiseAllotment> allotmentList = new List<DifficultyWiseAllotment>();
	int maxLevels=0;
	public DifficultyWiseAllotmentList (){
		
	}
	public void addDifficultyLevel(int level,int ques_allotted, int ques_attempted){
		//Change question allotted and attempted based on whether level is already present in allotment list or notcommon
		DifficultyWiseAllotment allotment = allotmentList.Find (e => e.difficulty_level == level);
		if (allotment != null) {
			allotment.changeAllotment (ques_allotted, ques_attempted);
		} else {
			maxLevels += 1;
			allotmentList.Add (new DifficultyWiseAllotment (level, ques_allotted, ques_attempted));
		}
	}
	public void registerAttempt(int level){
		DifficultyWiseAllotment allotment = allotmentList.Find (e => e.difficulty_level  == level);
		if (allotment != null) {
			int ques_allotted = allotment.allotted_ques;
			int ques_attempted = allotment.attempted_ques;	
			if (ques_allotted > ques_attempted) {
				allotment.changeAllotment (ques_allotted, ques_attempted+1);
			}

		} else {
			
		}
	}
	public int differenceByLevel(int level){
		DifficultyWiseAllotment allotment = allotmentList.Find (e => e.difficulty_level  == level);
		if (allotment != null) {
			return allotment.allotted_ques - allotment.attempted_ques;

		} else {
			return 0;
		}
	}

}

