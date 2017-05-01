using UnityEngine;
using System.Collections;

public class UserProfile  {
	public static  string firstName { get; set; }
	static public string lastName { get; set; }
	static public string email { get; set; }
	static public string number { get; set; }
	static public Standard userStandard { get; set; }
	static public Stream stream { get; set; }

	public UserProfile(string first_given, string last_given, string email_given, string number_given){
		Debug.Log("UserProfile "+first_given+last_given+email_given+number_given);
		firstName = first_given;
		lastName = last_given;
		email = email_given;
		number = number_given;
	}
	public UserProfile(){
		if (firstName != null) {
			//already name present
		} else {
			firstName = "";
			lastName = "";
			email = "";
			number = "";
		}

	}
}
