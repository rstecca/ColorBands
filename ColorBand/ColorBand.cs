﻿using UnityEngine;
using System.Collections;

public class ColorBand : ScriptableObject {

	public string name = "New Color Band";

	public AnimationCurve RCurve;
	public AnimationCurve GCurve;
	public AnimationCurve BCurve;

	//[SerializeField][HideInInspector]
	public Texture2D previewTexture;
	//[HideInInspector]
	//public Texture2D previewTexture { get { return _previewTexture; } }

	int Wt = 128;
	int Ht = 8;

	public ColorBand()
	{
		previewTexture = new Texture2D(Wt, Ht);
		RCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);
		GCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);
		BCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);

		buildPreviewTexture();
	}

	void OnEnable()
	{
		buildPreviewTexture();
	}

	void buildPreviewTexture()
	{
		// This can happen on load project and other cases
		if(previewTexture == null)
		{
			previewTexture = new Texture2D(Wt, Ht);
//			Debug.LogError("Texture is null");
		}

		int W = previewTexture.width;
		int H = previewTexture.height;

		Color[] colors = new Color[W*H];

		// set first line
		for(int i=0; i<W; i++)
		{
			Color c = new Color(
				RCurve.Evaluate( Mathf.Clamp01(((float)i)/((float)W)) ),
				GCurve.Evaluate( Mathf.Clamp01(((float)i)/((float)W)) ),
				BCurve.Evaluate( Mathf.Clamp01(((float)i)/((float)W)) )
				);
			colors[i] = c;
		}

		for(int j=1; j<H; j++)
		{
			System.Array.Copy(colors, 0, colors, j*W, W);
		}

		previewTexture.SetPixels(0,0,W, H, colors);
		previewTexture.Apply();
	}

	public void rebuildPreviewTexture()
	{
		buildPreviewTexture();
	}

	public Color Evaluate(float time01)
	{
		return new Color(RCurve.Evaluate(time01), GCurve.Evaluate(time01), BCurve.Evaluate(time01));
	}

}