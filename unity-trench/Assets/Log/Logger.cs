
using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// Log System
/// </summary>
public class Logger
{
    private bool hasDebug;

	private static Logger mLogger;

    private Logger()
    {
        hasDebug = true;
    }

	public static Logger getLogger
	{
		get
        {
            if(mLogger == null)
			{
				mLogger = new Logger();
			}
			return mLogger;
        }							
	}

    public void Log(System.Object obj)
	{
        if(hasDebug)
        {
            Debug.Log(obj);
        }
	}

}


