using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class xmlStageLottery : BaseXmlReader<Dictionary<int, INFO_STAGELOTTERY>, INFO_STAGELOTTERY>
{
    protected override void OnRead(INFO_STAGELOTTERY v)
    {
        m_Info.Add(v.Id, v);
    }
}

public class StageLottery : BaseXmlNode<Dictionary<int, INFO_STAGELOTTERY>, xmlStageLottery>
{

}
