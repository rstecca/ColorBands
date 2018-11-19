using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ColorBand))]
public class ColorBandEditor : Editor {

    public static Texture2D alphaPatternTexture;

    Color guiColor, guiContentColor, guiBackgroundColor;

	public override void OnInspectorGUI ()
	{
        guiColor = GUI.color;
        guiContentColor = GUI.contentColor;
        guiBackgroundColor = GUI.backgroundColor;

		// get the target class
		ColorBand _target = (ColorBand)target;

        Undo.RecordObject(_target, "Color Band Change");

        bool previousBiggerPreviewToggle = _target.biggerPreview;

        if (alphaPatternTexture == null)
            InitAlphaBackgroundPattern();

		// the preview texture leaks at non deterministic times in the editor so we have to watch it
		if(_target.previewTexture == null)
		{
			_target.rebuildPreviewTexture();
		}

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Preview", GUILayout.Width(100f));
        GUILayout.Label("(high precision preview", GUILayout.Width(150f));
        _target.biggerPreview = EditorGUILayout.Toggle(_target.biggerPreview, GUILayout.MaxWidth(10f));
        GUILayout.Label(")", GUILayout.Width(8f));
		// force to rebuild texture when switching to bigger and back
        if (previousBiggerPreviewToggle != _target.biggerPreview)
        {
            _target.previewTexture = null;
            _target.rebuildPreviewTexture();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        Rect r = GUILayoutUtility.GetLastRect();
        r.height = 32f;
        GUI.DrawTextureWithTexCoords(r, alphaPatternTexture, new Rect(0, 0, r.width * .75f / alphaPatternTexture.width, r.height * .75f / alphaPatternTexture.height));
        GUI.DrawTexture(r, _target.previewTexture, ScaleMode.StretchToFill, true);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //GUILayout.Label(_target.previewTexture);
        //EditorGUILayout.ColorField(new Color(0f,0f,0.5f));

        EditorGUILayout.BeginHorizontal();

		_target.name = EditorGUILayout.TextField("Name", _target.name);
		// Get filename of the colorband and set its name with the resulting string
        if (GUILayout.Button("Set as filename", GUILayout.MaxWidth(110f)))
        {
            string[] pathParts = (AssetDatabase.GetAssetPath(_target)).Split('/');
            string assetName = pathParts[pathParts.Length - 1].Split('.')[0];
            _target.name = assetName;
        }
        EditorGUILayout.EndHorizontal();

        //Color Space
        _target.colorSpace = (ColorBand.COLORSPACE) EditorGUILayout.EnumPopup("Color Space", _target.colorSpace);

		// Curve controls
        GUI.contentColor = new Color (1f, .4f, .4f);
        string rcurvename = (_target.colorSpace == ColorBand.COLORSPACE.RGB) ? "Red Curve" : "Hue Curve";
		_target.RCurve = EditorGUILayout.CurveField(rcurvename, _target.RCurve);
        GUI.contentColor =  new Color (.4f, 1f, .4f);
        string gcurvename = (_target.colorSpace == ColorBand.COLORSPACE.RGB) ? "Green Curve" : "Saturation Curve";
		_target.GCurve = EditorGUILayout.CurveField(gcurvename, _target.GCurve);
        GUI.contentColor = new Color (.4f, .4f, 1f);
        string bcurvename = (_target.colorSpace == ColorBand.COLORSPACE.RGB) ? "Blue Curve" : "Value Curve";
		_target.BCurve = EditorGUILayout.CurveField(bcurvename, _target.BCurve);
		GUI.contentColor = new Color (.85f, .85f, .95f);
		_target.ACurve = EditorGUILayout.CurveField("Alpha Curve", _target.ACurve);
        EditorGUILayout.EndVertical();

        GUI.contentColor = guiColor;

		EditorGUILayout.Space();


        EditorGUILayout.BeginHorizontal();
        _target.discrete = EditorGUILayout.Toggle("Discrete", _target.discrete);
        if (_target.discrete)
        {
            _target.discreteSteps = EditorGUILayout.IntSlider("Steps", _target.discreteSteps, 2, 256);
        }
        EditorGUILayout.EndHorizontal();
        if(_target.discrete)
        {
            _target.discreteMethod = (ColorBand.DISCRETE_METHOD)EditorGUILayout.EnumPopup("Discretization Method", _target.discreteMethod);
        }

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Save as image"))
		{
            string saveFileName = EditorUtility.SaveFilePanelInProject("Save ColorBand as PNG", _target.name, "png", "Please enter a filename to save the ColorBand to");
            if (saveFileName != "")
            {
                _target.rebuildPreviewTexture();
                byte[] bytes = _target.previewTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(saveFileName, bytes);
            }
		}
		EditorGUILayout.EndHorizontal();

        // When GUI changes save the ColorBand and rebuild the texture.
        if(GUI.changed)
		{
            _target.rebuildPreviewTexture();
            _target.applyRequired = true;
		}

        if(_target.applyRequired)
        {
            GUI.color = new Color(1f, .4f, 0f);
            if(GUILayout.Button("Apply"))
            {
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(_target);
                _target.rebuildPreviewTexture();
                _target.applyRequired = false;
            }
            GUI.color = guiColor;
            EditorGUILayout.HelpBox("Applying is required to make changes persistent.", MessageType.Warning);
        }

#if UNITY_5
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Warning: In Unity 5 there's some color inconsistency between preview and actual evaluated vaules. See Known Issues at https://github.com/rstecca/ColorBands/ for further details.", MessageType.Warning);
#endif

    }

    [MenuItem("Assets/Create/Color Band")]
	public static void CreateColorBand()
	{
		ColorBand newCB = ScriptableObject.CreateInstance<ColorBand>();
		string newfnameRoot = "New Color Band";
		string newfname = newfnameRoot;
		int i=0;
		while(System.IO.File.Exists("Assets/" + newfname + ".asset") && i<1000)
		{
			newfname = new System.Text.StringBuilder(newfnameRoot).Append(" ").Append(i.ToString("D" + 3)).ToString();
			i++;
		}
		newCB.name = newfname;
		AssetDatabase.CreateAsset(newCB, "Assets/"+newfname+".asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = newCB;
	}

    private void InitAlphaBackgroundPattern()
    {
        // GENERATE ALPHA BACKGROUND PATTERN
        if (alphaPatternTexture == null)
        {
            alphaPatternTexture = new Texture2D(8, 8);
            int w = alphaPatternTexture.width;
            int h = alphaPatternTexture.height;
            Color patternColor1 = new Color(.75f, .75f, .75f); // BRIGHT
            Color patternColor2 = new Color(.5f, .5f, .5f); // DARK
            Color bgColor;
            Color[] colors = new Color[w * h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if ((i % 8 ^ j % 8) < 4)
                        bgColor = patternColor1;
                    else
                        bgColor = patternColor2;

                    colors[i + j * w] = bgColor;
                }
            }
            alphaPatternTexture.SetPixels(colors);
            alphaPatternTexture.wrapMode = TextureWrapMode.Repeat;
            alphaPatternTexture.filterMode = FilterMode.Bilinear;
            alphaPatternTexture.Apply();
        }
    }

}
