// C# Example
	// Builds an asset bundle from the selected objects in the project view.
	// Once compiled go to "Menu" -> "Assets" and select one of the choices
	// to build the Asset Bundle
	
using UnityEngine;
using UnityEditor;
public class ExportAssetBundles 
{
	[MenuItem("Assets/Build AssetBundle From Selection - To Android")]
	static void ExportToAndroid()
	{
		string OldPath = PlayerPrefs.GetString("SaveAsset");
		if(OldPath == null)
			OldPath = "";
		string path = EditorUtility.SaveFilePanel ("Save Resource", OldPath, Selection.activeObject.name, null);

		if(path.Length != 0)
		{
			path+=".android.unity3d";
			Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			
			BuildPipeline.BuildAssetBundle(
				Selection.activeObject,
				selection,
				path,
				BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
				BuildTarget.Android
				);
			Selection.objects = selection;
			PlayerPrefs.SetString("SaveAsset",path);
		}
	}

	[MenuItem("Assets/Build AssetBundle From Selection - To Android Array")]
	static void ExportToAndroidArray()
	{
		string OldPath = PlayerPrefs.GetString("SaveAsset");
		if(OldPath == null)
			OldPath = "";

		string path = EditorUtility.SaveFilePanel ("Save Resource", OldPath,Selection.activeObject.name, null);
		
		if(path.Length != 0)
		{

			Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			string []allpath = path.Split('/');

			foreach(Object i in selection)
			{
				string strTotal = "";
				for(int z = 0 ; z < allpath.Length -1 ;++z)
				{
					strTotal += allpath[z] + "/";
				}
				strTotal = strTotal + i.name +".android.unity3d";

				Object []Test = new Object[1];
				Test[0] = i;
				
				BuildPipeline.BuildAssetBundle(
					i,
					Test,
					strTotal,
					BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
					BuildTarget.Android
					);
			}



			PlayerPrefs.SetString("SaveAsset",path);
			Selection.objects = selection;
		}
	}


	[MenuItem("Assets/Build AssetBundle From Selection - To Android[UnCompress]")]
	static void ExportToAndroidUnCompress()
	{
		string OldPath = PlayerPrefs.GetString("SaveAsset");
		if(OldPath == null)
			OldPath = "";
		string path = EditorUtility.SaveFilePanel ("Save Resource", OldPath, Selection.activeObject.name, null);
		
		if(path.Length != 0)
		{
			path+=".android.unity3d";
			Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			
			BuildPipeline.BuildAssetBundle(
				Selection.activeObject,
				selection,
				path,
				BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle,
				BuildTarget.Android
				);
			Selection.objects = selection;
			PlayerPrefs.SetString("SaveAsset",path);
		}
	}
	
	[MenuItem("Assets/Build AssetBundle From Selection - To Android Array[UnCompress]")]
	static void ExportToAndroidArrayUnCompress()
	{
		string OldPath = PlayerPrefs.GetString("SaveAsset");
		if(OldPath == null)
			OldPath = "";
		
		string path = EditorUtility.SaveFilePanel ("Save Resource", OldPath,Selection.activeObject.name, null);
		
		if(path.Length != 0)
		{
			
			Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			string []allpath = path.Split('/');
			
			foreach(Object i in selection)
			{
				string strTotal = "";
				for(int z = 0 ; z < allpath.Length -1 ;++z)
				{
					strTotal += allpath[z] + "/";
				}
				strTotal = strTotal + i.name +".android.unity3d";
				
				Object []Test = new Object[1];
				Test[0] = i;
				
				BuildPipeline.BuildAssetBundle(
					Selection.activeObject,
					Test,
					strTotal,
					BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle,
					BuildTarget.Android
					);
			}

			PlayerPrefs.SetString("SaveAsset",path);
			Selection.objects = selection;
		}
	}

	[MenuItem("Assets/Build AssetBundle From Selection - To Iphone")]
	static void ExportToIphone()
	{
		string OldPath = PlayerPrefs.GetString("SaveAsset");
		if(OldPath == null)
			OldPath = "";
		//string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "Iphone.unity3d");
		//if(path.Length != 0)
		//{
		//	Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
		//	BuildPipeline.BuildAssetBundle(
		//		Selection.activeObject,
		//		selection,
		//		path,
		//		BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
		//		BuildTarget.iPhone
		//		);
		//	Selection.objects = selection;
		//}

		string path = EditorUtility.SaveFilePanel ("Save Resource", OldPath,Selection.activeObject.name, null);
		if(path.Length != 0)
		{
			path+=".Iphone.unity3d";

			Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			
			BuildPipeline.BuildAssetBundle(
				Selection.activeObject,
				selection,
				path,
				BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
				BuildTarget.iPhone
				);
			Selection.objects = selection;
			PlayerPrefs.SetString("SaveAsset",path);
		}
	}

	[MenuItem("Assets/Build AssetBundle From Selection - To IphoneArray")]
	static void ExportToIphoneArray()
	{
		//string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "Iphone.unity3d");
		//if(path.Length != 0)
		//{
		//	Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
		//	BuildPipeline.BuildAssetBundle(
		//		Selection.activeObject,
		//		selection,
		//		path,
		//		BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
		//		BuildTarget.iPhone
		//		);
		//	Selection.objects = selection;
		//}
		//

		string OldPath = PlayerPrefs.GetString("SaveAsset");
		if(OldPath == null)
			OldPath = "";

		string path = EditorUtility.SaveFilePanel ("Save Resource", OldPath,Selection.activeObject.name, null);
		
		if(path.Length != 0)
		{
			
			Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			string []allpath = path.Split('/');
			
			foreach(Object i in selection)
			{
				string strTotal = "";
				for(int z = 0 ; z < allpath.Length -1 ;++z)
				{
					strTotal += allpath[z] + "/";
				}
				strTotal = strTotal + i.name +".Iphone.unity3d";
				
				Object []Test = new Object[1];
				Test[0] = i;
				
				BuildPipeline.BuildAssetBundle(
					i,
					Test,
					strTotal,
					BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
					BuildTarget.iPhone
					);
			}
			PlayerPrefs.SetString("SaveAsset",path);
			Selection.objects = selection;
		}
	}

	[MenuItem("Assets/Build AssetBundle From Selection - To Iphone[UnCompress]")]
	static void ExportToIphoneUnCompress()
	{
		string OldPath = PlayerPrefs.GetString("SaveAsset");
		if(OldPath == null)
			OldPath = "";

		string path = EditorUtility.SaveFilePanel ("Save Resource", OldPath,Selection.activeObject.name, null);
		if(path.Length != 0)
		{
			path+=".Iphone.unity3d";
			
			Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			
			BuildPipeline.BuildAssetBundle(
				Selection.activeObject,
				selection,
				path,
				BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle,
				BuildTarget.iPhone
				);
			Selection.objects = selection;
			PlayerPrefs.SetString("SaveAsset",path);
		}
	}
	
	[MenuItem("Assets/Build AssetBundle From Selection - To IphoneArray[UnCompress]")]
	static void ExportToIphoneArrayUnCompress()
	{

		string OldPath = PlayerPrefs.GetString("SaveAsset");
		if(OldPath == null)
			OldPath = "";
		
		string path = EditorUtility.SaveFilePanel ("Save Resource", OldPath,Selection.activeObject.name, null);
		
		if(path.Length != 0)
		{
			
			Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
			string []allpath = path.Split('/');
			
			foreach(Object i in selection)
			{
				string strTotal = "";
				for(int z = 0 ; z < allpath.Length -1 ;++z)
				{
					strTotal += allpath[z] + "/";
				}
				strTotal = strTotal + i.name +".Iphone.unity3d";
				
				Object []Test = new Object[1];
				Test[0] = i;
				
				BuildPipeline.BuildAssetBundle(
					Selection.activeObject,
					Test,
					strTotal,
					BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle,
					BuildTarget.iPhone
					);
			}
			PlayerPrefs.SetString("SaveAsset",path);
			Selection.objects = selection;
		}
	}


    static void ExportScenes(RuntimePlatform platform, bool toArray, bool uncompressed = false)
    {
        BuildAssetBundleOptions option = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
        if (uncompressed) option |= BuildAssetBundleOptions.UncompressedAssetBundle;

        string OldPath = PlayerPrefs.GetString("SaveAsset");
        if (OldPath == null)
            OldPath = "";

        BuildTarget tag = BuildTarget.Android;
        string ext = ".Android.unity3d";

        if (platform == RuntimePlatform.IPhonePlayer)
        {
            tag = BuildTarget.iPhone;
            ext = ".IPhone.unity3d";
        }

        string path = EditorUtility.SaveFilePanel("Save Resource", OldPath, Selection.activeObject.name, null);

        if (path.Length != 0)
        {
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            
            System.Collections.Generic.List<string> scenenames = new System.Collections.Generic.List<string>();

            if (toArray)
            {
                string[] allpath = path.Split('/');
                string strTotal = "";
                for (int z = 0; z < allpath.Length - 1; ++z)
                {
                    strTotal += allpath[z] + "/";
                }

                foreach (Object i in selection)
                {
                    scenenames.Clear();
                    string scene = AssetDatabase.GetAssetPath(i);
                                       
                    string scenepath = strTotal + i.name + ext;

                    scenenames.Add(scene);

                    BuildPipeline.BuildStreamedSceneAssetBundle(
                    scenenames.ToArray(),
                    scenepath,
                    tag
                    );
                }
            }
            else
            {
                path += ext;
                string scene = AssetDatabase.GetAssetPath(Selection.activeObject);                          
                scenenames.Add(scene);

                BuildPipeline.BuildStreamedSceneAssetBundle(
                    scenenames.ToArray(),
                    path,
                    tag
                    );
            }
            
            PlayerPrefs.SetString("SaveAsset", path);
            Selection.objects = selection;
        }        
    }


    [MenuItem("Assets/Build Scene AssetBundle From Selection - To Android")]
    static void ExportSceneToAndroid()
    {
        ExportScenes(RuntimePlatform.Android, false, false);
    }

    [MenuItem("Assets/Build Scene AssetBundle From Selection - To AndroidArray")]
    static void ExportSceneToAndroidArray()
    {
        ExportScenes(RuntimePlatform.Android, true, false);
    }

    [MenuItem("Assets/Build Scene AssetBundle From Selection - To Android[UnCompress]")]
    static void ExportSceneToAndroidUnCompress()
    {
        ExportScenes(RuntimePlatform.Android, false, true);
    }

    [MenuItem("Assets/Build Scene AssetBundle From Selection - To AndroidArray[UnCompress]")]
    static void ExportSceneToAndroidArrayUnCompress()
    {
        ExportScenes(RuntimePlatform.Android, true, true);
    }
}