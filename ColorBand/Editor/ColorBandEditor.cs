using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ColorBand))]
public class ColorBandEditor : Editor {

	public override void OnInspectorGUI ()
	{
		// get the target class
		ColorBand _target = (ColorBand)target;

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
		// Curve controls
        GUI.contentColor = new Color (1f, .4f, .4f);
		_target.RCurve = EditorGUILayout.CurveField("Red Curve", _target.RCurve);
        GUI.contentColor =  new Color (.4f, 1f, .4f);
		_target.GCurve = EditorGUILayout.CurveField("Green Curve", _target.GCurve);
        GUI.contentColor = new Color (.4f, .4f, 1f);
		_target.BCurve = EditorGUILayout.CurveField("Blue Curve", _target.BCurve);
		GUI.contentColor = new Color (.8f, .8f, .8f);
		_target.ACurve = EditorGUILayout.CurveField("Alpha Curve", _target.ACurve);
        EditorGUILayout.EndVertical();

        GUI.contentColor = Color.white;

		EditorGUILayout.Space();

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
