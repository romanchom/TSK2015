using UnityEngine;
using System.Collections;
using System;

public class MaxwellSource : MonoBehaviour {
	enum SourceType {
		Point
	}
	enum WaveFormType {
		Sinus,
		Pulse,
		GaussPulse,
		Sinc
	}

	const double c = 299792458;

	[SerializeField]
	SourceType sourceType;

	[SerializeField]
	WaveFormType waveForm;



	[SerializeField]
	float amplitude;
	[SerializeField]
	double waveLength;
	double angularSpeed;

	private Func<double, double>[] emittionFuncs;

	public MaxwellSource() {
		emittionFuncs = new Func<double, double>[4];
		emittionFuncs[0] = Math.Sin;
		emittionFuncs[1] = Pulse;
		emittionFuncs[2] = GaussPulse;
		emittionFuncs[3] = Sinc;
	}


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
		maxwell.E[x, y] = (float) (emittionFuncs[(int) waveForm](maxwell.time * angularSpeed) * amplitude);
	}

	private double Pulse(double t) {
		return t == 0 ? 1 : 0;
	}

	private double GaussPulse(double t) {
		t -= 5;
		return Math.Exp(-(t * t));
	}

	private double Sinc(double t) {
		t -= Math.PI * 5;
		return Math.Sin(t) / t;
	}
}
