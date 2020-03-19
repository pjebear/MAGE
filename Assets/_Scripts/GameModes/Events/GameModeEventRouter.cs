using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GameModeEvent
{
    public enum EventType
    {
        UISetup_Begin,
        UISetup_Complete,
        ModeSetup_Begin,
        ModeSetup_Complete,


        ModeStart,
        ModeEnd,

        ModeTakedown_Begin,
        ModeTakedown_Complete,
        UITakedown_Begin,
        UITakedown_Complete,

        SetupBegin = UISetup_Begin,
        SetupEnd = ModeSetup_Complete,

        NUM
    }

    public EventType Type;
    private object mArg;
    public GameModeEvent(EventType type, object arg = null)
    {
        Type = type;
        mArg = arg;
    }

    public T Arg<T>()
    {
        return (T)mArg;
    }
}


class GameModeEventRouter 
    : EventRouter<GameModeEvent>
{
    public static GameModeEventRouter Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Logger.Assert(Instance != null, LogTag.GameModes, "", "", LogLevel.Warning);
        }
        else
        {
            Instance = this;
        }
    }
}

