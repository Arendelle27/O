using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using log4net;
using System.IO;

public static class UnityLogger
{

    public static void Init()
    {
        Application.logMessageReceived += onLogMessageReceived;

        FileInfo fileInfo = new System.IO.FileInfo("log4net.configer");
        log4net.Config.XmlConfigurator.ConfigureAndWatch(fileInfo);//获取log4net配置文件

        string path= Path.Combine(Directory.GetCurrentDirectory(), "Log");
        GlobalContext.Properties["ApplicationLogPath"] = path; //日志生成的路径
        GlobalContext.Properties["LogFileName"] = "log"; //生成日志的文件名
        log4net.Config.XmlConfigurator.ConfigureAndWatch(fileInfo);
        Log.Init("Unity");
    }

    private static ILog log = LogManager.GetLogger("FileLogger");

    private static void onLogMessageReceived(string condition, string stackTrace, UnityEngine.LogType type)
    {
        switch(type)
        {
            case LogType.Error:
                log.ErrorFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Assert:
                log.DebugFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Exception:
                log.FatalFormat("{0\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Warning:
                log.WarnFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            default:
                log.Info(condition);
                break;
        }
    }
}