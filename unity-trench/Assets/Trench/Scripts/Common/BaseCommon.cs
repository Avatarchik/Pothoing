using UnityEngine;
using System.Collections;
using System;

public class BaseCommon : MonoBehaviour {

    private const string PrefabPath = "Prefab/BaseCommon";
    private static BaseCommon mInstance;
    public static BaseCommon Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(BaseCommon)) as BaseCommon;
                if (mInstance == null)
                {
                    GameObject obj = GameObject.Instantiate(Resources.Load(PrefabPath)) as GameObject;
                    mInstance = obj.GetComponent<BaseCommon>();
                    DontDestroyOnLoad(obj);
                }
            }
            return mInstance;
        }
    }

    public void Init()
    {
        mInstance = BaseCommon.Instance;
    }

    //获取Bundle
    public void GetBundle(string strElement, System.Type type, Action<UnityEngine.Object> RetrunBundle)
    {

        var files = ResmgrNative.Instance.verLocal.groups["testdown"].listfiles;
        if (files != null && files.ContainsKey(strElement))
        {
            files[strElement].BeginLoadAssetBundle((bundle, tag) =>
            {
                //UnityEngine.Object[] objes = bundle.LoadAll();
                UnityEngine.Object obj = bundle.Load("MainPrefab", type);
                bundle.Unload(false);
                RetrunBundle(obj);
            });
        }
    }

    void Update()
    {
        //检查加载任务
        ResmgrNative.Instance.Update();
    }


}
