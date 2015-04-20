using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer : SingleClass<Timer> {
    public const uint NormalTimeIDBase = 0;
    public const uint GameTimerIDBase = 900000;

    bool bRun = false;
    public class TimerParam
    {
        public uint ID;
        public float curtm;
        public float endtm;
        public System.Action<uint> call;
    }
    Dictionary<uint, TimerParam> DicTimer = new Dictionary<uint, TimerParam>();

	// Use this for initialization
	void Start () {
        bRun = true;
        StartCoroutine(OnEventTimer());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetTimer(uint id,float elipse, System.Action<uint> act )
    {
        TimerParam tm = new TimerParam();
        tm.ID = id;
        tm.curtm = 0.0f;
        tm.endtm = elipse;
        tm.call = act;

        DicTimer[id] = tm;
        //StartCoroutine()
    }

    public void KillTimer(uint id)
    {
        DicTimer.Remove(id);
    }

    IEnumerator OnEventTimer()
    {
        while (bRun)
        {
            yield return 0;
            List<uint> keyList = new List<uint>();

            foreach(var e in DicTimer)
            {
                e.Value.curtm += Time.deltaTime;
                if(e.Value.endtm<=e.Value.curtm)
                {
                    keyList.Add(e.Key);                    
                }
            }

            foreach (var k in keyList)
            {
                TimerParam param = DicTimer[k];
                DicTimer.Remove(k);
                if (param.call!=null) param.call(param.ID);                
            }

            //keyList = null;
        }
    }
}
