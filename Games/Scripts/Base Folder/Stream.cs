using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Stream: IComparable<Stream>{
	public int id { get; set; }
	public  string name { get; set; }
	public Stream(int stand_id, string stream_name){
		id = stand_id;
		name = stream_name;
	}
	public int CompareTo(Stream other)
	{
		if(other == null)
		{
			return 1;
		}

		//Return the difference in power.
		return  - other.id + id ;
	}
}
