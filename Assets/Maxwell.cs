using UnityEngine;
using System.Collections;

using scalar = System.Single;
using vector = UnityEngine.Vector2;

public class Maxwell : MonoBehaviour {
	scalar[,] H;
	vector[,] E;

	scalar[,] u_r;
	scalar[,] e_r;

	//const scalar e_0 = 8.854187817e-12F;
	//const scalar u_0 = 1.2566370614e-6F;

	[SerializeField]
	uint size = 128;
	[SerializeField]
	scalar timeStep = 1e-30F;
	[SerializeField]
	scalar worldScale = 1e-9F;

	// Use this for initialization
	void Start () {
		uint sizePP = size + 1;

		H = new scalar[size, size];
		u_r = new scalar[size, size];

		for (uint x = 0; x < size; ++x) {
			for (uint y = 0; y < size; ++y) {
				H[x, y] = 0;
				u_r[x, y] = 1;
			}
		}

		E = new vector[sizePP, sizePP];
		e_r = new scalar[sizePP, sizePP];

		for (uint x = 0; x < sizePP; ++x) {
			for (uint y = 0; y < sizePP; ++y) {
				E[x, y] = new vector();
				e_r[x, y] = 1;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		uint sizeMM = size - 1;
		scalar stepOverScale = timeStep / worldScale;

		for(uint x = 0; x < size; ++x) {
			for(uint y = 0; y < size; ++y) {
				scalar de_xOverDy = E[x, y + 1].x - E[x, y].x;
				scalar de_yOverDx = E[x + 1, y].y - E[x, y].y;
				scalar dBdt = de_yOverDx - de_xOverDy;
				scalar deltaH = dBdt * stepOverScale / u_r[x, y];
				H[x, y] += deltaH;
			}
		}

		for (uint x = 0; x < sizeMM; ++x) {
			for (uint y = 0; y < sizeMM; ++y) {
				scalar db_zOverDx = H[x + 1, y] - H[x, y];
				scalar db_zOverDy = H[x, y + 1] - H[x, y];
				vector dDdt = new vector(db_zOverDy, -db_zOverDx);
				vector deltaE = dDdt * stepOverScale / e_r[x + 1, y + 1];
				E[x + 1, y + 1] += deltaE;
			}
		}
	}
}
