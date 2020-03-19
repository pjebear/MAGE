using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum LogTag
{
    Assets,
    Character,
    DB,
    FlowControl,
    GameModes,
    GameSystems,
    Input,
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
    public static bool[] LogFilters = Enumerable.Repeat<bool>(true, (int)LogTag.NUM).ToArray();
    public static LogLevel LogThreshold = LogLevel.Notify;

    public static void Assert(bool assertionValid, LogTag tag, string name, string message, LogLevel logLevel = LogLevel.Warning)
    {
        if (!assertionValid)
        {
            Log(tag, name, message, logLevel);
        }
    }

    public static void Log(LogTag tag, string name, string message, LogLevel logLevel = LogLevel.Notify)
    {
        if (!LogFilters[(int)tag])
        {
            return;
        }

        if (logLevel < LogThreshold)
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

