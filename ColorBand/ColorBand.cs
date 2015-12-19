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
            // Get time corresponding to the current position i
			float t = Mathf.Clamp01(((float)i)/((float)W));
            // Get the color by evaluating all the curves
			Color c = new Color(
				RCurve.Evaluate( t ),
				GCurve.Evaluate( t ),
				BCurve.Evaluate( t ),
				ACurve.Evaluate( t )
				);

            // To optimize the drawing a bit we only draw 1st and 4th lines (and every 4th potentially).
            // We do this because we have to draw the checkboard underneath which can be crunched to just 2 different lines.
            // Later we propagate those lines to the next 3.

            // set preview's pixel on the 1st line. Also blend with a checkboard color
            if ((i % 8) < 4)
                bgColor = Color.grey;
            else
                bgColor = Color.black;
            colors[i] = c * (c.a) + bgColor * (1f - c.a);

            // set preview's pixel on the 4th line. Also blend with a checkboard color
            if ((i % 8) >= 4)
                bgColor = Color.grey;
            else
                bgColor = Color.black;
            colors[i + W*4] = c * (c.a) + bgColor * (1f - c.a);
		}

        // Propagate pixels from 1st and every 4th line of pixels to the following lines of lixels
        // We use the power of Array.Copy to save CPU
        for (int L = 0; L < H; L+=4)
        {
            for (int l = 0; l < 4; l++)
            {
                if((L%8) < 4)
                    System.Array.Copy(colors, 0, colors, L * W + l * W, W);
                else
                    System.Array.Copy(colors, W*4, colors, L * W + l * W, W);
            }

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
