using UnityEngine;
using System.Collections;

public class jTimeDestroyKill : MonoBehaviour
{

    [SerializeField]
    float fTimeKill = 0.0f;

    [SerializeField]
    Tweener logo;

    void Start()
    {
        int GraphicSetIdx = 0;
        if (System.Int32.Parse(DeviceManager.Instance.memtotal == null ? "2097152" : DeviceManager.Instance.memtotal) < 2 * 1024 * 1024)
        {
            PlayerPrefs.SetString("GraphicQuality", "Low");
            GraphicSetIdx = 0;
        }
        else
        {
            PlayerPrefs.SetString("GraphicQuality", "Middle");
            GraphicSetIdx = 1;
        }

        QualitySettings.SetQualityLevel(GraphicSetIdx);
        StartLogoAni();
    }

    void StartLogoAni()
    {
        //Time.timeScale = 2.2f;
		logo.FromTarget ();
        Invoke("DestroyDa", fTimeKill);
    }

    void DestroyDa()
    {
        //COMMON_FUNC.NDestroy(gameObject);
        Time.timeScale = 1.0f;
        Application.LoadLevelAsync(SceneMgr.SC_Loading);
    }
}
