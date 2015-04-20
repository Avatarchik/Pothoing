using UnityEngine;
using System.Collections;
using UnityEngineEx.DataFormat;
using UnityEngineEx.LogInterface;

public class SingleClass<T1> : MonoBehaviour where T1 : UnityEngine.Component
{

    static T1 mInstance = default(T1);
    public static T1 Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject parentobj = GameObject.Find("SingleClass#");  
                if (parentobj == null)
                {
                    parentobj = new GameObject();
                    parentobj.name = "SingleClass#";
                    DontDestroyOnLoad(parentobj);
                }
                
                mInstance = GameObject.FindObjectOfType(typeof(T1)) as T1;
                if (mInstance == null)
                {
                    GameObject go = new GameObject();
                    go.transform.SetParent(parentobj.transform);
                    go.name = typeof(T1).ToString();
                    mInstance = go.AddComponent<T1>();
                    DontDestroyOnLoad(go) ;
                }                                
            }
            return mInstance;
        }
    }
}

/// <summary>
/// xml 节点基类
/// </summary>
/// <typeparam name="T">容器类型</typeparam>
/// <typeparam name="T1">读取xml类的类型</typeparam>
public class BaseXmlNode<T, T1> : jXML<T> where T1 : jXML<T>
{
    protected T mInfo = System.Activator.CreateInstance<T>();
    public T INFO { get { return mInfo; } }

    public virtual void XmlLoad(string data, string node)
    {
        try
        {
            T1 xml = System.Activator.CreateInstance<T1>();
            xml.Load(mInfo, data, node);
        }
        catch(System.Exception ex)
        {
            string s = ex.Message;
            s += "|" + typeof(T1).ToString();
            Debuger.Instance.LogError(s);
        }               
    }

    public virtual void XmlLoad(TextAsset data, string node)
    {
        T1 xml = System.Activator.CreateInstance<T1>();
        xml.Load(mInfo, data.bytes, node);
    }
}

/// <summary>
/// xml读取基类
/// </summary>
/// <typeparam name="T1">容器类型</typeparam>
/// <typeparam name="T2">容器元素类型</typeparam>
public abstract class BaseXmlReader<T1, T2> : jXML<T1>
{
    protected override void SetNode()
    {
        base.SetNode();
        foreach (var e in typeof(T2).GetFields())
        {
            NodePush(e.Name);
        }
    }

    protected override void Read()
    {
        System.Reflection.FieldInfo[] propertys = typeof(T2).GetFields();

        T2 node = System.Activator.CreateInstance<T2>();
        for (int i = 0; i < propertys.Length; i++)
        {
            if (propertys[i].FieldType.ToString().IndexOf("int", System.StringComparison.OrdinalIgnoreCase) >= 0
                || propertys[i].FieldType.ToString().IndexOf("byte", System.StringComparison.OrdinalIgnoreCase) >= 0
                || propertys[i].FieldType.ToString().IndexOf("bool", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                propertys[i].SetValue(node, System.Convert.ChangeType(GetInt(i), propertys[i].FieldType));
            }
            else if (propertys[i].FieldType.ToString().IndexOf("single", System.StringComparison.OrdinalIgnoreCase) >= 0
                || propertys[i].FieldType.ToString().IndexOf("double", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                propertys[i].SetValue(node, GetFloat(i));
            }
            else
            {
                propertys[i].SetValue(node, GetString(i));
            }
        }

        OnRead(node);
    }

    protected abstract void OnRead(T2 v);
}
