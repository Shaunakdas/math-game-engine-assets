using System;
using System.Collections;
public class DifficultyWiseAllotment{
	public int difficulty_level{get; set;}
	public int allotted_ques{ get; set; }
	public int attempted_ques{ get; set;}

	public DifficultyWiseAllotment (int diff_level,int allotted,int attempted){
		difficulty_level = diff_level;
		allotted_ques = allotted;
		attempted_ques = attempted;
	}
	public void registerAttempt(){
		attempted_ques++;
	}
	public void changeAllotment(int allotted,int attempted){
		allotted_ques = allotted;
		attempted_ques = attempted;
	}
}


