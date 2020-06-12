using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ExplorationEvent
{
    public enum EventType
    {
        InteractionStart,
        InteractionEnd,
        ScenarioTriggered,

        NUM
    }

    public EventType Type;
    private object mArg;
    public ExplorationEvent(EventType type, object arg = null)
    {
        Type = type;
        mArg = arg;
    }

    public T Arg<T>()
    {
        return (T)mArg;
    }

}

class ExplorationEventRouter
    : EventRouter<ExplorationEvent>
{
    public static ExplorationEventRouter Instance;

    private void Awake()
    {
        Instance = this;
    }
}
