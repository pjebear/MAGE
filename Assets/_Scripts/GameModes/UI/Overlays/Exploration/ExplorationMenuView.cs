using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using UnityEngine.Events;

class ExplorationMenuView
    : UIContainer
{
    public enum ComponentId
    {
        OutfiterBtn,
        SaveBtn,
        ExitBtn,
        RandomEncounter,
    }

    public UIButton OutfiterBtn;
    public UIButton SaveBtn;
    public UIButton ExitBtn;
    public UIButton RandomEncounter;

    public override void Publish(IDataProvider dataProvider)
    {
        // empty
    }

    protected override void InitComponents()
    {
        OutfiterBtn.Init((int)ComponentId.OutfiterBtn, this);
        SaveBtn.Init((int)ComponentId.SaveBtn, this);
        ExitBtn.Init((int)ComponentId.ExitBtn, this);
        RandomEncounter.Init((int)ComponentId.RandomEncounter, this);
    }

    protected override void InitSelf()
    {
        mId = (int)UIContainerId.ExplorationMenuView;
        mContainerName = UIContainerId.ExplorationMenuView.ToString();
    }

    protected override IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo)
    {
        return interactionInfo;
    }
}

