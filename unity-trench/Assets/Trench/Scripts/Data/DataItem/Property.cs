using System;
using System.Collections.Generic;

public class xmlProperty : BaseXmlReader< Dictionary< int ,INFO_PROPERTY>,INFO_PROPERTY>
{
    protected override void OnRead(INFO_PROPERTY v)
    {
        m_Info.Add(v.Id, v);
    }
}

public class Property : BaseXmlNode<Dictionary< int ,INFO_PROPERTY>,xmlProperty>
{

}
