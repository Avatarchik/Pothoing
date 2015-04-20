using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class xmlPlayerTitle :BaseXmlReader<Dictionary<int,TitleInfoData>,TitleInfoData>
{
	protected override void OnRead (TitleInfoData v)
	{
		m_Info.Add (v.Id, v);
	}
}
	
public class PlayerTitle : BaseXmlNode<Dictionary<int ,TitleInfoData>,xmlPlayerTitle>
{
		
}
