using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Standard: IComparable<Standard>{
	public int id { get; set; }
	public int subject_id { get; set; }
	public int standard_number { get; set; }
	public  string standard_name { get; set; }
	public Standard(int stand_id,int subj_id,int stand_num, string subj_name){
		id = stand_id;
		subject_id = subj_id;
		standard_number = stand_num;
		standard_name = subj_name;
	}
	public int CompareTo(Standard other)
	{
		if(other == null)
		{
			return 1;
		}

		//Return the difference in power.
		return  - other.standard_number + standard_number ;
	}
}
