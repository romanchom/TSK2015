using UnityEngine;
using System.Collections;

public class MaxwellSource : MonoBehaviour {
	enum SourceType {
		Point
	}

	[SerializeField]
	SourceType sourceType;

	[SerializeField]
	double amplitude;
	[SerializeField]
	double frequency;
	public float val;
	
	public void Emit(Maxwell maxwell) {
		switch (sourceType) {
		case SourceType.Point:
			EmitPoint(maxwell);
			break;
		}
	}

	private void EmitPoint(Maxwell maxwell) {
		Vector3 pos = gameObject.transform.position;
		pos /= 10;
		pos += new Vector3(0.5f, 0.5f, 0);
		pos *= maxwell.size;
		uint x = (uint) pos.x;
		uint y = (uint) pos.y;
		val = (float)(System.Math.Sin(maxwell.time * frequency) * amplitude);
		maxwell.H[x, y] += val;
	}
}
