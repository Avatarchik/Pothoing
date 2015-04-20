using System.Collections.Generic;
using UnityEngine;

public enum enRoomType
{
    enNormal=1,   //普通场
    enBomb=2,     //炸弹场

    enNull,     //无效场次
}

public class RoomMgr
{
    RoomConfig room_config = new RoomConfig();

    public void Load()
    {
        string data = Resources.Load(COMMON_CONST.PathRoomConfig).ToString();
        room_config.XmlLoad(data, "RoomConfig");
    }

    /// <summary>
    /// 获取一类房间列表
    /// </summary>
    /// <param name="ty"></param>
    /// <returns></returns>
    public List<INFO_ROOM> GetRoomsByType(enRoomType ty)
    {
        var list = new List<INFO_ROOM>();
        if (room_config.INFO.ContainsKey((ushort)ty) == false) return null;

        list.AddRange(room_config.INFO[(ushort)ty]);

        return list;

    }

    /// <summary>
    /// 获取房间信息
    /// </summary>
    /// <param name="roomid">房间id</param>
    /// <returns></returns>
    public INFO_ROOM SearchRoomById(ushort roomid)
    {
        foreach(var e in room_config.INFO.Values)
        {
            INFO_ROOM info = e.Find(a => { return a.wServerID == roomid; });
            if (info != null) return info;
        }

        return null;
    }

    /// <summary>
    /// 获取场次类型
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public  enRoomType GetRoomType(INFO_ROOM room)
    {
        foreach(var e in room_config.INFO)
        {
            if (e.Value.Find(a => { return a.wServerID == room.wServerID; }) != null)
                return (enRoomType)e.Key;
        }

        return enRoomType.enNull;
    }
}