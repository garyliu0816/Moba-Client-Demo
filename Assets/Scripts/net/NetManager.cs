using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// 主线程
public class NetManager
{
    private static NetManager instance;
    private static readonly object lockObj = new object();

    private List<Action> actionList = new List<Action>();
    private Mutex mutex = new Mutex();

    private NetManager()
    {
        GameObject obj = new GameObject("NetManager");
        obj.AddComponent<NetUpdate>();
        GameObject.DontDestroyOnLoad(obj);
    }

    public static NetManager GetInstance()
    {
        if (instance == null)
        {
            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = new NetManager();
                }
            }
        }
        return instance;
    }

    public void Destory()
    {
        instance = null;
    }

    public void AddAction(Action _action)
    {
        mutex.WaitOne();
        actionList.Add(_action);
        mutex.ReleaseMutex();
    }

    public void DoForAction()
    {
        mutex.WaitOne();
        for (int i = 0; i < actionList.Count; i++)
        {
            actionList[i]();
        }
        actionList.Clear();
        mutex.ReleaseMutex();
    }
}


public class NetUpdate : MonoBehaviour
{
    void Update()
    {
        NetManager.GetInstance().DoForAction();
    }

    void OnApplicationQuit()
    {
        TcpClient.Instance.DisConnectServer();
    }
}
