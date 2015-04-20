using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageBtn : MonoBehaviour
{
	[SerializeField]
	Image[] stars;
	[SerializeField]
	Sprite star;
	[SerializeField]
	Sprite grayStar;
    [SerializeField]
    Text stageName;

    

    private uint stageId;

    
    public event System.Action<uint> clickEvent;
    public void InitStage(uint stageId, string stageName, Vector3 local)
    {
        this.stageId = stageId;
        this.stageName.text = stageName;
        this.transform.localPosition = local;

    }

    void Start()
    {
        EventTriggerListener.Get(this.gameObject).onClick += OnClick;
    }

    //点击时触发
    public void OnClick(GameObject go)
    {
        if (clickEvent != null && stageId > 0)
            this.clickEvent(stageId);

    }

	public void SetStar (int starNum)
	{
		for(int i=0;i<starNum;i++){
			stars[i].sprite=star;
			stars[i].gameObject.SendMessage("Show");
		}
	}
}
