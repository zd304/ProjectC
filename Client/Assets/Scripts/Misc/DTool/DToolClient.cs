﻿using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

public enum DTool_STC
{
    DTool_STC_None = 0,
    DTool_STC_ReqObject = 1,
    DTool_STC_ReqActive = 2,
    DTool_STC_ReqObjMemory = 3,
    DTool_STC_ReqMemory = 4,
    DTool_STC_ReqUpdateHierarchy = 5,
    DTool_STC_ReqGraphicBase = 6,

    DTool_STC_Count
}

public enum DTool_CTS
{
    DTool_CTS_None = 0,
    DTool_CTS_UpdateHierarchy = 1,
    DTool_CTS_UpdateObject = 2,
    DTool_CTS_AddLog = 3,
    DTool_CTS_ObjMemory = 4,
    DTool_CTS_Memory = 5,
    DTool_CTS_GraphicBase = 6,

    DTool_CTS_Count
}

public class DToolClient : MonoBehaviour
{
    void Start()
    {
        InitServer();
    }

    void InitServer()
    {
        string strJson = "";
        string configPath = Application.persistentDataPath;

        if (!File.Exists(configPath + "/config/DTool.cfg"))
        {
            if (!Directory.Exists(configPath + "/config"))
            {
                Directory.CreateDirectory(configPath + "/config");
            }
            using (StreamWriter sw = new StreamWriter(configPath + "/config/DTool.cfg", false, Encoding.UTF8))
            {
                sw.Write("{ \"ip\": \"127.0.0.1\", \"port\": 5304 }");
                sw.Close();
            }
            Debug.LogError("Not Find " + configPath + "/config/DTool.cfg");
            return;
        }

        using (FileStream file = new FileStream(configPath + "/config/DTool.cfg", FileMode.Open))
        {
            byte[] byData = new byte[file.Length];
            char[] charData = new char[file.Length];
            file.Seek(0, SeekOrigin.Begin);
            file.Read(byData, 0, (int)file.Length);
            Decoder d = Encoding.UTF8.GetDecoder();
            d.GetChars(byData, 0, byData.Length, charData, 0);
            file.Close();

            strJson = new string(charData);
        }

        Debug.Log("Loading " + configPath + "/config/DTool.cfg");

        if (string.IsNullOrEmpty(strJson))
            return;

        Debug.Log(configPath);
        JsonData json = JsonMapper.ToObject<JsonData>(strJson);
        if (json == null)
            return;

        string strIP = json["ip"].ToString();
        int port = 5304;
        if (!int.TryParse(json["port"].ToString(), out port))
        {
            return;
        }
        IPAddress ipAdr = IPAddress.Parse(strIP);
        ipep = new IPEndPoint(ipAdr, port);

        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipep);
        }
        catch (Exception)
        {
            clientSocket = null;
        }
        if (clientSocket != null)
        {
            thread = new Thread(new ThreadStart(GoClient));
            thread.Start();

            logic = new DToolLogic(this);
            logic.Init();
            StartCoroutine(DelayInit());
        }
    }

    IEnumerator DelayInit()
    {
        yield return new WaitForEndOfFrame();

        logic.UpdateHierarchy();
    }

    void GoClient()
    {
        while (true)
        {
            if (clientSocket == null)
                return;
            byte[] receiveData = new byte[1024];
            int recv = 0;
            try
            {
                recv = clientSocket.Receive(receiveData);
            }
            catch (Exception)
            {
                clientSocket = null;
                recv = 0;
            }
            if (recv == 0)
            {
                clientSocket = null;
                continue;
            }

            lock (this)
            {
                mRecvData = receiveData;
                mRecvLen = recv;
            }

            //Debug.LogError(msg);
        }
    }

    void Connect()
    {
        if (clientSocket != null)
        {
            //clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            clientSocket = null;
            return;
        }
        clientSocket.Connect(ipep);
    }

    public void SendToServer(int ctsType, string msg, bool unlock = false)
    {
        if (mBigMsgLock && !unlock)
            return;
        if (clientSocket == null)
            return;

        byte[] intBuff = System.BitConverter.GetBytes(ctsType);
        byte[] concent = Encoding.UTF8.GetBytes(msg);
        byte[] tmp = new byte[intBuff.Length + concent.Length];
        System.Buffer.BlockCopy(intBuff, 0, tmp, 0, intBuff.Length);
        System.Buffer.BlockCopy(concent, 0, tmp, intBuff.Length, concent.Length);
        try
        {
            // int count = clientSocket.Send(tmp, tmp.Length, SocketFlags.None);
        }
        catch (SocketException e)
        {
            Debug.LogError(e.Message);
        }
    }

    void OnEnable()
    {
#if UNITY_2018
        Application.logMessageReceived += HandleLog;
#else
        Application.RegisterLogCallback(HandleLog);
#endif
    }

    void OnDisable()
    {
#if UNITY_2018
		Application.logMessageReceived -= HandleLog;
#else
        Application.RegisterLogCallback(null);
#endif

        if (clientSocket != null)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        if (thread != null)
        {
            thread.Interrupt();
            thread.Abort();
        }
    }

    void Update()
    {
        lock (this)
        {
            if (mRecvData != null && mRecvLen > 0)
            {
                int stc = System.BitConverter.ToInt32(mRecvData, 0);
                string msg = Encoding.UTF8.GetString(mRecvData, 4, mRecvLen - 4);

                logic.OnMsg((DTool_STC)stc, msg);

                mRecvData = null;
                mRecvLen = 0;
            }
        }
    }

    void HandleLog(string message, string stack, LogType type)
    {
        if (logic == null)
            return;
        logic.AddLog(message, stack, type);
    }

    public void SetBigMsgLock(bool bLock)
    {
        mBigMsgLock = bLock;
    }

    Thread thread;
    Socket clientSocket = null;
    IPEndPoint ipep;
    public DToolLogic logic = null;
    byte[] mRecvData = null;
    int mRecvLen = 0;
    bool mBigMsgLock = false;

    public Dictionary<int, GameObject> mSceneGameObjects = new Dictionary<int, GameObject>();

    static string mPersistentDataPath = "";
    public static string persistentDataPath
    {
        get
        {
            if (string.IsNullOrEmpty(mPersistentDataPath))
            {
                mPersistentDataPath = Application.persistentDataPath;
                // Android : /data/data/xxx.xxx.xxx/files
                // IOS : Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Documents
#if UNITY_EDITOR_WIN
                mPersistentDataPath = Application.dataPath;
                mPersistentDataPath = mPersistentDataPath.Substring(0, mPersistentDataPath.LastIndexOf('/'));
                mPersistentDataPath += "/Persistent";
#endif
            }
            return mPersistentDataPath;
        }
    }
}