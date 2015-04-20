using System.Collections.Generic;
using UnityEngine;

public class PropertyMgr
{
    public const int PROPERTY_NULL          = 0;
    public const int PROPERTY_BOM           = 10001;          //必有炸弹
    public const int PROPERTY_RECORD        = 10002;          //记牌器
    public const int PROPERTY_GETNEXTCARD   = 10003;          //偷窥手牌
    public const int PROPERTY_STOPOUT       = 10004;          //禁手
    public const int PROPERTY_CHANGE        = 10005;          //偷梁换柱
    public const int PROPERTY_GETLASTCARD   = 10006;          //偷窥底牌
    public const int PROPERTY_SCORE2        = 10007;          //分数翻倍


    Property prop = new Property();

    public void Load()
    {
        string data = Resources.Load(COMMON_CONST.PathProperty).ToString();
        prop.XmlLoad(data, "Property");
    }

    public List<INFO_PROPERTY> GetAllProperty()
    {
        List<INFO_PROPERTY> list = new List<INFO_PROPERTY>();
        list.AddRange(prop.INFO.Values);
        return list;
    }

    public INFO_PROPERTY GetProperty(int propertyid)
    {
        foreach(var e in prop.INFO.Values)
        {
            if (e.Id == propertyid)
                return e;
        }

        return null;
    }
}
