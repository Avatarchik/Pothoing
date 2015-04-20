using System.Collections.Generic;

public class xmlRoom :BaseXmlReader<Dictionary<ushort,List<INFO_ROOM>>,INFO_ROOMCFG>
{
    protected override void OnRead(INFO_ROOMCFG v)
    {
        //throw new System.NotImplementedException();
        if (m_Info.ContainsKey(v.Type) == false) m_Info.Add(v.Type, new List<INFO_ROOM>());
        m_Info[v.Type].Add(v.GetInfoRoom());
        //m_Info[v.wServerType].Sort((l, r) => { return l.lCellScore.CompareTo(r.lCellScore); });
    }
}

public class RoomConfig : BaseXmlNode<Dictionary<ushort,List<INFO_ROOM>>,xmlRoom>
{

}
