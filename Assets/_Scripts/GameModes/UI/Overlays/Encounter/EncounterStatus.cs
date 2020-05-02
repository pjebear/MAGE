using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EncounterStatus : UIContainer
{
    private string tag = "EncounterStatus";

    public enum ComponentId
    {
        ContinueBtn,
        WinBtn,
        LoseBtn,
    }


    public class DataProvider : IDataProvider
    {
        public override string ToString()
        {
            string turnOrder = "";
            if (TurnOrder.HasValue)
            {
                foreach (string turn in TurnOrder.Value)
                {
                    turnOrder += turn + ",";
                }
            }
            return string.Format("Turn: {0} Clock: {1}", turnOrder, (CurrentTurn.HasValue ? CurrentTurn.Value.ToString() : "X"));
        }

        public Optional<int> CurrentTurn;
        public Optional<string> ActorName;
        public Optional<string> ActorOwner;
        public Optional<string> ActorHealth;
        public Optional<List<string>> ActorStatuses;

        public Optional<List<string>> TurnOrder;
        public Optional<EncounterState> CurrentState;
    }

    public UIText EncounterStatusText;
    public UIText ActorStatusText;
    public UIText EncounterOverText;
    public GameObject EncounterOverObj;
    public GameObject ActorStatusObj;
    public GameObject EncounterStatusObj;

    public UIButton ContinueButton;
    public UIButton WinButton;
    public UIButton LoseButton;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = (DataProvider)dataProvider;

        string encounterStatusText = "";
        encounterStatusText += "Clock: " + dp.CurrentTurn.ValueOr(-1);
        encounterStatusText += Console.Out.NewLine;
        encounterStatusText += "TurnOrder: ";
        encounterStatusText += Console.Out.NewLine;
       
        foreach (string name in dp.TurnOrder.ValueOr(new List<string>()))
        {
            encounterStatusText += name + Console.Out.NewLine;
        }
        
        EncounterStatusText.Publish(new UIText.DataProvider(encounterStatusText));

        string actorStatusText = "";
        actorStatusText += "name: " + dp.ActorName.ValueOr("");
        actorStatusText += Console.Out.NewLine;
        actorStatusText += "health: " + dp.ActorHealth.ValueOr("");
        actorStatusText += Console.Out.NewLine;
        actorStatusText += "owner: " + dp.ActorOwner.ValueOr("");
        actorStatusText += Console.Out.NewLine;
        actorStatusText += "statuses: ";
        actorStatusText += Console.Out.NewLine;

        foreach (string status in dp.ActorStatuses.ValueOr(new List<string>()))
        {
            actorStatusText += status + Console.Out.NewLine;
        }

        ActorStatusText.Publish(new UIText.DataProvider(actorStatusText));

        if (dp.CurrentState.HasValue && dp.CurrentState.Value != EncounterState.InProgress)
        {
            EncounterOverObj.gameObject.SetActive(true);
            EncounterOverText.Publish(new UIText.DataProvider(dp.CurrentState.Value.ToString()));
        }
    }

    protected override void InitComponents()
    {
        ContinueButton.Init((int)ComponentId.ContinueBtn, this);
        WinButton.Init((int)ComponentId.WinBtn, this);
        LoseButton.Init((int)ComponentId.LoseBtn, this);

        EncounterOverObj.gameObject.SetActive(false);
    }

    protected override void InitSelf()
    {
        mId = (int)UIContainerId.EncounterStatusView;
        mContainerName = tag;
    }

    protected override IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo)
    {
        return interactionInfo;
    }
}

