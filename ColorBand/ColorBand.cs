using UnityEngine;
using System.Collections;

public class ColorBand : ScriptableObject {

	public string name = "New Color Band";

	public AnimationCurve RCurve;
	public AnimationCurve GCurve;
	public AnimationCurve BCurve;
	public AnimationCurve ACurve;

	public Texture2D previewTexture;
    public bool biggerPreview = false;

	int Wt = 128, Wt_big = 256;
	int Ht = 8, Ht_big = 8;

	public ColorBand()
	{
		previewTexture = new Texture2D(Wt, Ht);
		RCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);
		GCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);
		BCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);
		ACurve = AnimationCurve.Linear(timeStart:0f, valueStart:1f, timeEnd:1f, valueEnd:1f); // Initialized as constant to 1f

		buildPreviewTexture();
	}

	void OnEnable()
	{
		buildPreviewTexture();
	}

	/// <summary>
	/// Builds the preview texture.
	/// </summary>
	void buildPreviewTexture()
	{
		// This can happen on load project and other cases
		if(previewTexture == null)
		{
            if(biggerPreview)
                previewTexture = new Texture2D(Wt_big, Ht_big);
            else
			    previewTexture = new Texture2D(Wt, Ht);
		}

		int W = previewTexture.width;
		int H = previewTexture.height;

		Color[] colors = new Color[W*H];

		Color bgColor = Color.black;


		for(int i=0; i<W; i++)
		{
			float t = Mathf.Clamp01(((float)i)/((float)W));
			Color c = new Color(
				RCurve.Evaluate( t ),
				GCurve.Evaluate( t ),
				BCurve.Evaluate( t ),
				ACurve.Evaluate( t )
				);

			if(c.a<0.99f)
			{
				for(int j=0; j<H; j++)
				{
					//if((i*j)%8<4)// Curious logo-like pattern :)
					if((i%8 ^ j%8) < 4)
						bgColor = Color.grey;
					else
						bgColor = Color.black;
					
					colors[i+j*W] = c * (c.a) + bgColor * (1f-c.a);
				}
			}
			else // Save some computation. Best tradeoff when alpha is not used (and is constant 1f)
			{
				for(int j=0; j<H; j++)
				{
					colors[i+j*W] = c;
				}
			}

		}

		// old code that copied the first lines to all the other ones.
		// This cannot be done with alpha because we have to draw the background texture.
//		for(int j=1; j<H; j++)
//		{
//			System.Array.Copy(colors, 0, colors, j*W, W);
//		}

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
