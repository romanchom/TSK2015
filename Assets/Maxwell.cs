using UnityEngine;
using System.Collections;

using scalar = System.Single;
using vector = UnityEngine.Vector2;

public class Maxwell : MonoBehaviour {
	[HideInInspector]
	public scalar[,] H;
	[HideInInspector]
	public vector[,] E;
	[HideInInspector]
	public double time;

	scalar[,] u_r;
	scalar[,] e_r;

	Color[] texData;

	const double e_0 = 8.854187817e-12;
	const double u_0 = 1.2566370614e-6;

	[SerializeField]
	public uint size = 128;
	[SerializeField]
	double timeStep = 1e-27F;
	[SerializeField]
	double worldScale = 1e-9F;
	[SerializeField]
	MaxwellSource[] sources;

	Texture2D texture;

	[SerializeField]
	Material material;

	// Use this for initialization
	void Start () {
		uint sizePP = size + 1;

		H = new scalar[size, size];
		u_r = new scalar[size, size];

		for (uint x = 0; x < size; ++x) {
			float X = (float)(x) - size / 2;
			X *= 0.1f;
			float v = Mathf.Exp(-X * X);
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
				e_r[x, y] = x > size / 2 ? 3 : 1;
			}
		}

		texData = new Color[size * size];

		texture = new Texture2D((int) size, (int) size, TextureFormat.RFloat, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		material.mainTexture = texture;
		UpdateTex();
	}
	
	void UpdateTex() {
		foreach(var src in sources) {
			src.Emit(this);
		}

		for(uint i = 0; i < size; ++i) {
			for(uint j = 0; j < size; ++j) {
				texData[i * size + j].r = H[i, j];
			}
		}
		texture.SetPixels(texData);
		texture.Apply();
	}

	// Update is called once per frame
	void Update () {
		time += timeStep;
		uint sizeMM = size - 1;
		double stepOverScale = timeStep / worldScale;
		scalar stepOverScaleOverU_0 = (scalar) (stepOverScale / u_0);
		scalar stepOverScaleOverE_0 = (scalar)(stepOverScale / e_0);

		for (uint x = 0; x < size; ++x) {
			for(uint y = 0; y < size; ++y) {
				scalar de_xOverDy = E[x, y + 1].x - E[x, y].x;
				scalar de_yOverDx = E[x + 1, y].y - E[x, y].y;
				scalar dBdt = de_yOverDx - de_xOverDy;
				scalar deltaH = dBdt * stepOverScaleOverU_0 / u_r[x, y];
				H[x, y] += deltaH;
			}
		}


		for (uint x = 1; x < size; ++x) {
			for (uint y = 1; y < size; ++y) {
				scalar db_zOverDx = H[x, y] - H[x - 1, y];
				scalar db_zOverDy = H[x, y] - H[x, y - 1];
				vector dDdt = new vector(-db_zOverDy, db_zOverDx);
				vector deltaE = dDdt * stepOverScaleOverE_0 / e_r[x, y];
				E[x, y] += deltaE;
			}
		}

		UpdateTex();
	}
}
