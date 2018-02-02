using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ColorBand))]
public class ColorBandEditor : Editor {

	public override void OnInspectorGUI ()
	{
		// get the target class
		ColorBand _target = (ColorBand)target;

        Undo.RecordObject(_target, "Color Band Change");

        bool previousBiggerPreviewToggle = _target.biggerPreview;

		// the preview texture leaks at non deterministic times in the editor so we have to watch it
		if(_target.previewTexture == null)
		{
			_target.rebuildPreviewTexture();
		}

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Preview", GUILayout.Width(100f));
        GUILayout.Label("(bigger", GUILayout.Width(60f));
        _target.biggerPreview = EditorGUILayout.Toggle(_target.biggerPreview, GUILayout.MaxWidth(10f));
        GUILayout.Label(")", GUILayout.Width(8f));
		// force to rebuild texture when switching to bigger and back
        if (previousBiggerPreviewToggle != _target.biggerPreview)
        {
            _target.previewTexture = null;
            _target.rebuildPreviewTexture();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label(_target.previewTexture);
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

        GUI.contentColor = Color.white;

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
                _target.rebuildPreviewTexture(true); // rebuild the texture but without the alpha pattern
                byte[] bytes = _target.previewTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(saveFileName, bytes);
                _target.rebuildPreviewTexture(); // restore the texture with alpha pattern

                string saveFileAbsPath = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/" + saveFileName;
                if (!System.IO.File.Exists(saveFileAbsPath))
                    Debug.LogWarning("File not found: " + saveFileAbsPath);
                TextureImporter TI = (TextureImporter)TextureImporter.GetAtPath(saveFileAbsPath);
                //if (System.IO.File.Exists(System.IO.Directory.GetParent(Application.dataPath).ToString() + "/" + saveFileName))
                //    Debug.Log("FILE CHECK");

                AssetDatabase.ImportAsset(saveFileAbsPath, ImportAssetOptions.ForceUpdate);
                TI.textureCompression = TextureImporterCompression.Uncompressed;
                //TI.textureFormat = TextureImporterFormat.ARGB32;
                //TI.textureType = TextureImporterType.Default;
                TI.wrapMode = TextureWrapMode.Clamp;
                TI.alphaIsTransparency = true;
                TI.SaveAndReimport();
            }
		}
		EditorGUILayout.EndHorizontal();

		// When GUI changes save the ColorBand and rebuild the texture.
		if(GUI.changed)
		{
			AssetDatabase.SaveAssets();
			EditorUtility.SetDirty(_target);
			_target.rebuildPreviewTexture();
		}
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

}
