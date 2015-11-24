using UnityEngine;
using System.Collections;

public class MaterialProp : MonoBehaviour {
	public void SetBrightness(float val) {
		GetComponent<Renderer>().material.SetFloat("_Scale", val);
	}

	public void SetIndexBrightness(float val) {
		Color c = GetComponent<Renderer>().material.GetColor("_ColorPerm");
		c.b = val;
		GetComponent<Renderer>().material.SetColor("_ColorPerm", c);
	}
}
