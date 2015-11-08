using UnityEngine;
using System.Collections;
using System;

public class MaxwellSource : MonoBehaviour {
	enum SourceType {
		Point
	}

	const double c = 299792458;

	[SerializeField]
	SourceType sourceType;

	[SerializeField]
	double amplitude;
	[SerializeField]
	double waveLength;
	double angularSpeed;


	public void Emit(Maxwell maxwell) {

		angularSpeed = c / (waveLength * 1e-9) * Math.PI * 2;
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
		maxwell.H[x, y] = (float)(System.Math.Sin(maxwell.time * angularSpeed) * amplitude);
	}
}
