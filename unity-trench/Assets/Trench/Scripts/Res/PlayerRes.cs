using UnityEngine;
using System.Collections;




public class PlayerRes : MonoBehaviour
{

    public enum Sex
    {
        Male,
        Lady,
        None
    }

    /// <summary>
    /// 坑主 平民
    /// </summary>
    [SerializeField]
    Sprite[] mRole;

    /// <summary>
    /// ICON
    /// </summary>
    [SerializeField]
    Sprite[] mIcon;

    public Sprite GetPlayerIcon(Sex sex)
    {
        return mIcon[(int)sex];
    }

    public Sprite GetPlayerRole(PlayerPanel.PlayerMaster role)
    {
        return mRole[(int)role];
    }
}
