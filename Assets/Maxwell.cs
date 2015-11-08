using UnityEngine;
using System.Collections;
using System;

using scalar = System.Single;
using vector = UnityEngine.Vector2;

public class Maxwell : MonoBehaviour {
	enum SurfaceType {
		Smooth,
		AntiReflectionFinish,
		RoughSin,
		RoughTriangle
	}
	private const double e_0 = 8.854187817e-12;
	private const double u_0 = 1.2566370614e-6;

	[SerializeField]
	public uint size = 128;
	[SerializeField]
	private double timeStep;
	[SerializeField]
	private double worldSizeUM;

	[SerializeField]
	private MaxwellSource[] sources;
	[SerializeField]
	private Material material;
	[SerializeField]
	private SurfaceType surfaceType;
	[SerializeField]
	private float indexOfRefrection;
	[SerializeField]
	private double finishThicknessNM;
	[SerializeField]
	private double roughnessSpacingNM;

	private double worldScaleNM;
	private double worldScale;


	[HideInInspector]
	public scalar[,] H;
	[HideInInspector]
	public vector[,] E;
	[HideInInspector]
	public double time;

	private scalar[,] u_r;
	private scalar[,] e_r;

	private Texture2D texture;
	private Color[] texData;
	private uint sizePP;

	// Use this for initialization
	void Start () {
		sizePP = size + 1;
		worldScaleNM = worldSizeUM * 1000 / size;
		worldScale = worldScaleNM * 1e-9;

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
			uint height = (uint) (size / 2);
			for (uint y = 0; y < sizePP; ++y) {
				E[x, y] = new vector();
			}
		}

		switch (surfaceType) {
		case SurfaceType.Smooth:
			InitializeSmooth();
			break;
		case SurfaceType.AntiReflectionFinish:
			InitializeTwoMedia();
			break;
		case SurfaceType.RoughSin:
			InitializeRoughSin();
			break;
		case SurfaceType.RoughTriangle:
			InitializeRoughTriangle();
			break;
		}

		texData = new Color[size * size];

		texture = new Texture2D((int) size, (int) size, TextureFormat.RGFloat, false);
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
				texData[i * size + j].g = e_r[i, j];
			}
		}
	}

	void OnPreRender() {
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

	void InitializeSmooth() {
		float tempE_r = indexOfRefrection * indexOfRefrection;
		for (uint x = 0; x < sizePP; ++x) {
			uint height = (uint)(size / 2);
			for (uint y = 0; y < sizePP; ++y) {
				scalar val = y > height ? tempE_r : 1;
				e_r[x, y] = val;
			}
		}
	}

	void InitializeTwoMedia() {
		uint thickness = (uint) (finishThicknessNM / worldScaleNM);
		for (uint x = 0; x < sizePP; ++x) {
			uint height = (uint)(size / 2);
			
			for (uint y = 0; y < sizePP; ++y) {
				scalar val = 1;
				if (y > height) {
					val *= indexOfRefrection;
					if (y > height + thickness) val *= indexOfRefrection;
				}
				e_r[x, y] = val;
			}
		}
	}

	void InitializeRoughSin() {
		float tempE_r = indexOfRefrection * indexOfRefrection;
		uint thickness = (uint)(finishThicknessNM / worldScaleNM / 2);
		double sinArgMul = worldScale / (roughnessSpacingNM * 1e-9) * (Math.PI * 2);
        for (uint x = 0; x < sizePP; ++x) {
			uint height = (uint)(size / 2 + Math.Sin(x * sinArgMul) * thickness);
			for (uint y = 0; y < sizePP; ++y) {
				scalar val = y > height ? tempE_r : 1;
				e_r[x, y] = val;
			}
		}
	}

	void InitializeRoughTriangle() {

	}
}
