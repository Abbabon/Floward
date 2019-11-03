using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Text;

public class LogSaverAndSender : MonoBehaviour
{
    public bool enableSave = true;
    
    [Serializable]
    public struct Logs
    {
        public string condition;
        public string stackTrace;
        public LogType type;

        public string dateTime;

        public Logs(string condition, string stackTrace, LogType type, string dateTime)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.type = type;
            this.dateTime = dateTime;
        }
    }

    [Serializable]
    public class LogInfo
    {
        public List<Logs> logInfoList = new List<Logs>();
    }

    LogInfo logs = new LogInfo();

    void OnEnable()
    {
        //Subscribe to Log Event
        Application.logMessageReceived += LogCallback;
    }

    //Called when there is an exception
    void LogCallback(string condition, string stackTrace, LogType type)
    {
        //Create new Log
        Logs logInfo = new Logs(condition, stackTrace, type, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));

        //Add it to the List
        logs.logInfoList.Add(logInfo);
    }
    

    void OnDisable()
    {
        //Un-Subscribe from Log Event
        Application.logMessageReceived -= LogCallback;
    }

    //Save log  when focous is lost
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            //Save
            if (enableSave)
                DataSaver.saveData(logs, "savelog");
        }
    }

    //Save log on exit
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            //Save
            if (enableSave)
                DataSaver.saveData(logs, "savelog");
        }
    }
}
