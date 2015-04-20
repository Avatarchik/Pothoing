using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using cn.sharesdk.unity3d;

public class FinalDialog : Dialog {

    [SerializeField]
    Transform mMove;
    [SerializeField]
    GameObject mPanelContext;
    [SerializeField]
    ScoreItem mLab;

    [SerializeField]
    ResultTip
        resultTip;

    List<ScoreItem> mAllItem;

    Action mAction;

    void Start()
    {
        mAllItem = new List<ScoreItem>();
    }

    public void SetWin(bool isWin)
    {
        //resultTip.gameObject.SetActive(true);
        resultTip.SetResultTip(isWin);
        resultTip.ShowResult(isWin);
    }


    public override void Show()
    {
        base.Show();
        
    }

    public override void Hide()
    {
        base.Hide();
		if (mAction != null) {
			mAction ();
		}
        ClearAllItem();
    }


    IEnumerator AddItem(int[] allData)
    {

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < allData.Length; i++)
        {            
            Vector3 pos =  mMove.position + new Vector3(50, i * 20 - 100, 0);

            ScoreItem tLab = Instantiate(mLab, pos, Quaternion.identity) as ScoreItem;
            tLab.transform.SetParent(mPanelContext.transform);
            tLab.SetData(allData[i]);

            //yield return new WaitForSeconds(0.05f);

            mAllItem.Add(tLab);          
        }

   
//         for(int i = 0;i < mAllItem.Count;i++)
//         {
//             iTween.MoveFrom(
//                mAllItem[i].gameObject,
//                iTween.Hash(
//                    "position",  new Vector3(100, 0, 0),
//                    "islocal", false,
//                    "easetype", iTween.EaseType.easeInOutBack,
//                    "time", 0.5f
//                ));
// 
           yield return new WaitForSeconds(0.5f);
//         }
    }

    /// <summary>
    /// 设置结算统计数据
    /// </summary>
    /// <param name="allData"></param>
    public void SetStatisticsData(int[] allData)
    {
        StartCoroutine(AddItem(allData));
    }

    /// <summary>
    /// 关卡结算
    /// </summary>
    /// <param name="allData"></param>
    public void SetStageData(int[] allData)
    {
        StartCoroutine(AddItem(allData));
    }


    public void SetConuite(Action action)
    {
        mAction = action;
    }


    void ClearAllItem()
    {
        foreach (ScoreItem go in mAllItem)
        {
            go.transform.SetParent(null);

            Destroy(go.gameObject);            
        }

        mAllItem.Clear();
    }
    
	public void Share(){
		Hashtable content = new Hashtable();
		content["content"] = "this is a test string.";
		content["image"] = "http://img.baidu.com/img/image/zhenrenmeinv0207.jpg";
		content["title"] = "test title";content["description"] = "test description";
		content["url"] = "http://sharesdk.cn";
		content["type"] = Convert.ToString((int)ContentType.News);
		content["siteUrl"] = "http://sharesdk.cn";content["site"] = "ShareSDK";
		content["musicUrl"] = "http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3";
		ShareResultEvent evt = new ShareResultEvent(ShareResultHandler);
		ShareSDK.showShareMenu (null, content, 100, 100, MenuArrowDirection.Up, evt);
	}

	void ShareResultHandler (ResponseState state, PlatformType type, Hashtable shareInfo, Hashtable error, bool end)
	{
		if (state == ResponseState.Success)
		{
			print ("share result :");
			print (MiniJSON.jsonEncode(shareInfo));
		}
		else if (state == ResponseState.Fail)
		{
			print ("fail! error code = " + error["error_code"] + "; error msg = " + error["error_msg"]);
		}}
}
