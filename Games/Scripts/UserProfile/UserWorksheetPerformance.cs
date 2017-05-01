using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserWorksheetPerformance : MonoBehaviour {
	public int id { get; set; }
	public string Topic { get; set; }
	public string Stream { get; set; }
	public string Chapter { get; set; }
	public string Subject { get; set; }
	public string Standard { get; set; }
	public string Proficiency { get; set; }
	public List<string> BenifitList { get; set;}
	public int HighScore { get; set; }
	public int Diamonds { get; set; }
	public int MaxDiamonds { get; set; }
	public int Wins { get; set; }
	public int Attempts { get; set; }
	public List<int> ScoreList { get; set; }
	public int CurrentDifficultyLevel { get; set; }
	public int NextDifficultyLevel { get; set; }
	public float Ranking { get; set; }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
