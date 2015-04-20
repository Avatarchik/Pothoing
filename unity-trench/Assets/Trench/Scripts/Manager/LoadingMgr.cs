using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadingMgr : MonoBehaviour
{

    public Slider LoadingSlider;
    public Text LoadingText;
    AsyncOperation async;
    string strState = "";

    [SerializeField]
    bool IsUpdateRes = false;

	void Start() 
    {
        if (IsUpdateRes)
        { 
            List<string> wantdownGroup = new List<string>();
            wantdownGroup.Add("testdown");
            ResmgrNative.Instance.BeginInit("http://192.168.1.26/publicdown/", OnInitFinish, wantdownGroup);
            LoadingText.text = "检查资源";
        }
        else
            StartCoroutine(loadScence());
        
	}

    bool indown = false;
    void OnInitFinish(System.Exception err)
    {
        if (err == null)
        {
            ResmgrNative.Instance.taskState.Clear();
            LoadingText.text = "检查资源完成";
            List<string> wantdownGroup = new List<string>();
            wantdownGroup.Add("testdown");
            var downlist = ResmgrNative.Instance.GetNeedDownloadRes(wantdownGroup);
            //判断需要下载的资源大小
            if (ResmgrNative.Instance.taskState.tasksize > 0)
            {
                foreach (var d in downlist)
                {
                    d.Download(null);
                }
                ResmgrNative.Instance.WaitForTaskFinish(DownLoadFinish);
                indown = true;
            }
            else 
            {
                LoadingText.text = "加载场景中...";
                StartCoroutine(loadScence());
            }
            
        }
        else
            strState = null;
    }
    void DownLoadFinish()
    {
        indown = false;
        LoadingText.text = "更新完成";
        LoadingSlider.value = 0;
        LoadingText.text = "加载场景中...";
        StartCoroutine(loadScence());
    }

    //加载场景
    IEnumerator loadScence()
    {
        async = Application.LoadLevelAsync("Lobby");
        yield return async;
    }

	void Update () {
        if (IsUpdateRes)
        {
            ResmgrNative.Instance.Update();
            if(indown)
            {
                float downingSize = ResmgrNative.Instance.taskState.downingSize / 1024f;
                float tasksize = ResmgrNative.Instance.taskState.tasksize / 1024f;


                LoadingText.text = downingSize.ToString("f2") +
                                   "M / " + downingSize.ToString("f2") + "M";
                LoadingSlider.value = downingSize / tasksize * 10;
            }
        }
        
        
        if(async!= null && !async.isDone)
        {
            LoadingSlider.value = async.progress * 10;
        }
	}
}
