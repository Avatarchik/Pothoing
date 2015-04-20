using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class HandCard : MonoBehaviour
{

    public List<Card> HandCards;
    public Card card;
    public List<Card> Selecteds; //选中的牌
    public int MaxDistance = 56; //最大牌距
    public int MinDistance = 44; //最小牌距

    private float distance = 0; //实际牌距

    enum ActionType
    {
        None,
        EventAction,
        SelectAction,
        DrawAction,
    }
    ActionType actionType = ActionType.None;
    void Awake()
    {
        HandCards = new List<Card>();
    }

    void Start()
    {
        StartCoroutine(AddHandCard());
        //添加鼠标按下事件
        EventTriggerListener.Get(gameObject).onDown = OnMouseDown;
        EventTriggerListener.Get(gameObject).onUp = OnMouseUp;
        //addEvent();
    }

    private void OnMouseDown(GameObject go)
    {
        Debug.Log("-----onDown");
        if (actionType == ActionType.None)
        {
            return;
        }
        else if (actionType == ActionType.EventAction)
        {
            actionType = ActionType.SelectAction;
            beginIndex = CheckCard(Input.mousePosition.x);
            Debug.Log("-----" + beginIndex);
        }


    }
    private void OnMouseUp(GameObject go)
    {
        //actionType = ActionType.None;
        if (actionType == ActionType.SelectAction)
        {
            actionType = ActionType.EventAction;
            if (selectColor.Count > 0)
            {
                Selected();
            }
        }
        Debug.Log("-----onUp");
    }


    //判断是否在牌上
    private int beginIndex = -1;
    private int CheckCard(float x)
    {
        for (int i = 0; i < HandCards.Count; i++)
        {
            Vector3 v3 = Camera.main.WorldToScreenPoint(HandCards[i].transform.position);
            if (v3.x - 52.5 < x && x < (v3.x - 52.5) + distance)
                return i;

            if (i == HandCards.Count - 1)
            {
                if (v3.x - 52.5 < x && x < v3.x + 52.5)
                    return i;
            }
        }
        return -1;
    }

    IEnumerator AddHandCard()
    {
        for (int i = 0; i < 16; i++)
        {
            Card obj = Instantiate(card) as Card;
            HandCards.Add(obj);
            Debug.Log("----------------------------localscale" + obj.transform.localScale.x);
            SortHands();
            yield return new WaitForSeconds(0.2f);
        }
        actionType = ActionType.EventAction;//发牌完毕可以操作
        yield return 0;


    }



    //牌位置计算
    public void SortHands()
    {
        //计算牌距
        if (HandCards.Count > 0)
        {
            if (HandCards.Count > 1)
                distance = (this.GetComponent<RectTransform>().rect.width - HandCards[0].GetComponent<RectTransform>().rect.width)
                            / (HandCards.Count - 1);
            if (distance > MaxDistance) distance = MaxDistance;
            if (distance < MinDistance) distance = MinDistance;

            float handsWidth = (HandCards.Count - 1) * distance;//- 0.5f*HandCards [0].GetComponent<RectTransform> ().rect.width;

            //牌位置
            float firstPoint = -handsWidth / 2;
            for (int i = 0; i < HandCards.Count; i++)
            {
                Card obj = HandCards[i];
                obj.transform.parent = this.transform;
                obj.transform.localPosition = new Vector3((firstPoint + i * distance), 0, 0);
                obj.transform.localScale = new Vector3(1,1,1);
                Debug.Log("----------------------------localscale" + obj.transform.localScale.x);
            }

        }
    }
    //选定牌
    void Selected()
    {
        if (selectColor.Count == 0) return;
        //Selecteds.Clear();
        List<int> temp = new List<int>();
        for (int i = 0; i < HandCards.Count; i++)
        {
            if (selectedColor.Contains(i))
            {
                if (selectColor.Contains(i))
                {
                    HandCards[i].ShowMask(false);
                    Vector3 v3 = HandCards[i].transform.localPosition;
                    v3.y = 0;
                    HandCards[i].transform.localPosition = v3;
                }
                else
                {
                    temp.Add(i);
                }
            }
            else
            {
                if (selectColor.Contains(i))
                {
                    temp.Add(i);
                    HandCards[i].ShowMask(false);
                    Vector3 v3 = HandCards[i].transform.localPosition;
                    v3.y += 20;
                    HandCards[i].transform.localPosition = v3;
                }
            }
        }
        selectedColor = temp;

    }

    //甩牌操作
    void DarwAction()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        var screenSpace = Camera.main.WorldToScreenPoint(DarwParent.transform.position);
        var curPosition = Camera.main.ScreenToWorldPoint(new Vector3(x, y, screenSpace.z));

        //var curScreenSpace = new Vector3(x, y, screenSpace.z);
        //var curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
        DarwParent.transform.position = curPosition;

    }

    //选牌操作
    void SelectAction()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        int end = CheckCard(x);
        if (beginIndex > -1 && end > -1)
        {
            foreach (int s in selectColor)
            {
                //Vector3 v3 = HandCards[s].transform.localPosition;
                //v3.y = 0;
                //HandCards[s].transform.localPosition = v3;
                HandCards[s].ShowMask(false);
            }
            selectColor.Clear();
            int fb = beginIndex;
            int eb = end;
            if (beginIndex > end)
            {
                fb = end;
                eb = beginIndex;
            }

            for (; fb <= eb; fb++)
            {
                selectColor.Add(fb);
                //Vector3 v3 = HandCards[fb].transform.localPosition;
                //v3.y = 20;
                //HandCards[fb].transform.localPosition = v3;
                HandCards[fb].ShowMask(true);
            }


        }
    }

    List<int> selectedColor = new List<int>();
    private List<int> selectColor = new List<int>();
    private GameObject DarwParent;
    // Update is called once per frame
    void Update()
    {
        if (actionType == ActionType.None) return;

        if (actionType == ActionType.DrawAction && DarwParent != null)
        {
            DarwAction();
            return;

        }
        if (actionType == ActionType.SelectAction)
        {
            SelectAction();
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            if (y > 160 && DarwParent == null && selectColor.Count > 0)
            {
                actionType = ActionType.DrawAction;
                DarwParent = new GameObject();
                DarwParent.transform.parent = this.transform;
                int index = 0;
                for (int i = 0; i < HandCards.Count; i++)
                {
                    if (selectColor.Contains(i) || selectedColor.Contains(i))
                    {
                        Card obj = HandCards[i];
                        obj.transform.parent = DarwParent.transform;
                        obj.transform.localPosition = new Vector3((index * distance), 0, 0);
                        index++;
                    }
                }

                DarwParent.transform.localScale = new Vector3(0.65f, 0.65f, 1);
                //DarwAction();
            }
            return;
        }


    }
}