using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeGetter : MonoBehaviour {
	Text text;
	public Maxwell maxwell;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = maxwell.time * 1e15f + " fs";
	}
}
