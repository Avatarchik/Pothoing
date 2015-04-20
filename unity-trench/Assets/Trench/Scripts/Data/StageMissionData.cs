using UnityEngine;
using System.Collections;

public enum MissionProgressState{
	Locked=0,
	Progressing,
	Completed,
	Failed,
	Invalid,
	Skipped
}

public class StageMissionData {

    public uint condId;
	public string title;
	public string desc;
	public int targetProgress;
	public int currentProgress;
	public MissionProgressState state;
}
