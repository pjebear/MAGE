using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum LogTag
{
    AI,
    Assets,
    Character,
    DB,
    FlowControl,
    Combat,
    GameModes,
    GameSystems,
    Input,
    Level,
    Map,
    Messaging,
    Party,
    Story,
    UI,
    
    NUM
}

public enum LogLevel
{
    Notify,
    Warning,
    Error,

    NUM
}

static class Logger 
{
    public static bool[] LogFilters = Enumerable.Repeat<bool>(false, (int)LogTag.NUM).ToArray();
    public static LogLevel LogThreshold = LogLevel.Notify;

    public static void Fail(LogTag tag, string name, string message, LogLevel logLevel = LogLevel.Warning)
    {
        Log(tag, name, message, logLevel);
    }

    public static void Assert(bool assertionValid, LogTag tag, string name, string message, LogLevel logLevel = LogLevel.Warning)
    {
        if (!assertionValid)
        {
            Log(tag, name, message, logLevel);
        }
    }

    public static void Log(LogTag tag, string name, string message, LogLevel logLevel = LogLevel.Notify)
    {
        // Still allow log messages that are above logLevel through the filter
        if (logLevel < LogThreshold)
        {
            return;
        }

        if (LogFilters[(int)tag] && logLevel == LogLevel.Notify)
        {
            return;
        }

        switch (logLevel)
        {
            case LogLevel.Notify:
                Debug.Log(string.Format("[{0}] [{1}] {2}", tag.ToString(), name, message));
                break;
            case LogLevel.Warning:
                Debug.LogWarning(string.Format("[{0}] [{1}] {2}", tag.ToString(), name, message));
                break;
            case LogLevel.Error:
                Debug.LogError(string.Format("[{0}] [{1}] {2}", tag.ToString(), name, message));
                break;
        }
       
    }
}

