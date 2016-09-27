using UnityEngine;
using System.Collections;

public class ColorBand : ScriptableObject {

	public string name = "New Color Band";

	public AnimationCurve RCurve;
	public AnimationCurve GCurve;
	public AnimationCurve BCurve;
	public AnimationCurve ACurve;

    public enum COLORSPACE { RGB, HSV };
    public COLORSPACE colorSpace = COLORSPACE.RGB;

	public Texture2D previewTexture;
    public bool biggerPreview = false;

    public bool discrete = false;
    public int discreteSteps = 16;
    public enum DISCRETE_METHOD { LEFT_VALUE, RIGHT_VALUE, CENTER_VALUE };
    public DISCRETE_METHOD discreteMethod = DISCRETE_METHOD.LEFT_VALUE;

	int Wt = 128, Wt_big = 256;
	int Ht = 8, Ht_big = 8;

	public ColorBand()
	{
#if !UNITY_5_3_OR_NEWER
        previewTexture = new Texture2D(Wt, Ht);
#endif
		RCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);
		GCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);
		BCurve = AnimationCurve.Linear(timeStart:0f, valueStart:0f, timeEnd:1f, valueEnd:1f);
		ACurve = AnimationCurve.Linear(timeStart:0f, valueStart:1f, timeEnd:1f, valueEnd:1f); // Initialized as constant to 1f

#if !UNITY_5_3_OR_NEWER
		buildPreviewTexture();
#endif
	}

	void OnEnable()
	{
#if UNITY_5_3_OR_NEWER
        previewTexture = new Texture2D(Wt, Ht);
#endif
		buildPreviewTexture();
	}

    Color GetColorAt(float t, bool useAlpha = false, COLORSPACE _colorSpace = COLORSPACE.RGB)
    {
        if (_colorSpace == COLORSPACE.RGB)
        {
            if (useAlpha)
            {
                return new Color(RCurve.Evaluate(t), GCurve.Evaluate(t), BCurve.Evaluate(t), ACurve.Evaluate(t));
            }
            else
            {
                return new Color(RCurve.Evaluate(t), GCurve.Evaluate(t), BCurve.Evaluate(t));
            }
        }
        else // if(_colorSpace == COLORSPACE.HSV)
        {
            if (useAlpha)
            {
                Color ac = Color.HSVToRGB(RCurve.Evaluate(t), GCurve.Evaluate(t), BCurve.Evaluate(t));
                return new Color(ac.r, ac.g, ac.b, ACurve.Evaluate(t));
            }
            else
            {
                return Color.HSVToRGB(RCurve.Evaluate(t), GCurve.Evaluate(t), BCurve.Evaluate(t));
            }
        }
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
            if(discrete)
            {
                if (discreteMethod == DISCRETE_METHOD.LEFT_VALUE)
                    t = Mathf.Floor(t * discreteSteps) / discreteSteps;
                else if (discreteMethod == DISCRETE_METHOD.RIGHT_VALUE)
                    t = Mathf.Ceil(t * discreteSteps) / discreteSteps;
                else if (discreteMethod == DISCRETE_METHOD.CENTER_VALUE)
                    t = 0.5f * (Mathf.Floor(t * discreteSteps) / discreteSteps + Mathf.Ceil(t * discreteSteps) / discreteSteps);
            }

            Color c = GetColorAt( t, useAlpha:true, _colorSpace:colorSpace);

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

    /// <summary>
    /// Evaluates color curves at time time01
    /// </summary>
    /// <param name="time01">The time point where the color is computed. Value must be between 0 and 1.</param>
    /// <returns>The color in the current color space.</returns>
	public Color Evaluate(float time01)
	{
        if(!discrete)
        {
            //return new Color(RCurve.Evaluate(time01), GCurve.Evaluate(time01), BCurve.Evaluate(time01)); // HSV FIX
            return GetColorAt(time01, false, colorSpace);
        }
        else
        {
            float t = -1f;
            if (discreteMethod == DISCRETE_METHOD.LEFT_VALUE)
                t = Mathf.Floor(time01 * discreteSteps) / discreteSteps;
            else if (discreteMethod == DISCRETE_METHOD.RIGHT_VALUE)
                t = Mathf.Ceil(time01 * discreteSteps) / discreteSteps;
            else if (discreteMethod == DISCRETE_METHOD.CENTER_VALUE)
                t = 0.5f * (Mathf.Floor(time01 * discreteSteps) / discreteSteps + Mathf.Ceil(time01 * discreteSteps) / discreteSteps);
            return GetColorAt(t, false, colorSpace);
        }

	}

}
