using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EncounterEvent
{
    public enum EventType
    {
        EncounterBegun,
        ClockProgressed,
        TurnBegun,
        ActionResolved,
        MoveResolved,
        TurnFinished,
        EncounterOver,
        CharacterKO,

        NUM
    }

    public EventType Type;
    private object mArg;
    public EncounterEvent(EventType type, object arg = null)
    {
        Type = type;
        mArg = arg;
    }

    public T Arg<T>()
    {
        return (T)mArg;
    }

}

class EncounterEventRouter
    : EventRouter<EncounterEvent>
{
    public static EncounterEventRouter Instance;

    private void Awake()
    {
        Instance = this;
    }
}
