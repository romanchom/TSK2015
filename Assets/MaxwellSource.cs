using UnityEngine;
using System.Collections;
using System;

public class MaxwellSource : MonoBehaviour {
	enum SourceType {
		Point,
		Segment
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
	GameObject other;



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
		maxwellSize = maxwell.size;

		angularSpeed = c / (waveLength * 1e-9) * Math.PI;
		switch (sourceType) {
		case SourceType.Point:
			EmitPoint(maxwell);
			break;
		case SourceType.Segment:
			EmitSegment(maxwell);
			break;
		}
	}

	void WorldToGrid(Vector3 pos, out int x, out int y) {
		pos /= 10;
		pos += new Vector3(0.5f, 0.5f, 0);
		pos *= maxwellSize;
		x = (int) pos.x;
		y = (int) pos.y;
	}

	uint maxwellSize;

	private void EmitPoint(Maxwell maxwell) {
		Vector3 pos = gameObject.transform.position;
		int x, y;
		WorldToGrid(pos, out x, out y);
		maxwell.E[x, y] = (float) (emittionFuncs[(int) waveForm](maxwell.time * angularSpeed) * amplitude);
	}

	private void EmitSegment(Maxwell maxwell) {
		int x1, y1, x2, y2;
		WorldToGrid(gameObject.transform.position, out x1, out y1);
		WorldToGrid(other.transform.position, out x2, out y2);

		int dX = x2 - x1;
		int dY = y2 - y1;
		int step;
		if(Math.Abs(dX)> Math.Abs(dY)) {
			step = Math.Abs(dX);
		} else {
			step = Math.Abs(dY);
		}

		float val = (float)(emittionFuncs[(int)waveForm](maxwell.time * angularSpeed) * amplitude);

		for (int i = 0; i < step; ++i) {
			int x = x1 + dX * i / step;
			int y = y1 + dY * i / step;
			maxwell.E[x, y] = val;
        }
	}

	private double Pulse(double t) {
		return t > 1 ? 1 : 0;
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
