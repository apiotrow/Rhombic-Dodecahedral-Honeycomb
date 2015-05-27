using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Grammars {
	//a: x+, b: z+, c: x-, d: z-, starting: a
	public static Dictionary<string, string> levycurve = new Dictionary<string, string>(){
		{"a", "ab"},
		{"b", "bc"},
		{"c", "cd"},
		{"d", "da"}
	};
	
	//a: x+, b: z+, c: x-, d: z-, starting: a
	public static Dictionary<string, string> juliaSetish = new Dictionary<string, string>(){
		{"a", "bab"},
		{"b", "cbc"},
		{"c", "dcd"},
		{"d", "ada"}
	};
	
	//a: x+, b: z+, c: x-, d: z-, starting: ac
	public static Dictionary<string, string> spiraly = new Dictionary<string, string>(){
		{"a", "dad"},
		{"b", "ba"},
		{"c", "bcb"},
		{"d", "dc"}
	};
	
	//a: x+, b: z+, c: x-, d: z-, starting: ac
	public static Dictionary<string, string> rhombus = new Dictionary<string, string>(){
		{"a", "bcb"},
		{"b", "ba"},
		{"c", "dad"},
		{"d", "dc"}
	};
	
	////a: x+, b: z+, c: x-, d: z-, starting: ace
	public static Dictionary<string, string> loops = new Dictionary<string, string>(){
		{"a", "faf"},
		{"b", "ba"},
		{"c", "bcb"},
		{"d", "dc"},
		{"e", "ded"},
		{"f", "fe"}
	};

	
	public static Dictionary<string, string> rules = new Dictionary<string, string>(){
		{"a", "faf"},
		{"b", "ba"},
		{"c", "bcb"},
		{"d", "dc"},
		{"e", "ded"},
		{"f", "fe"}
	};

}
