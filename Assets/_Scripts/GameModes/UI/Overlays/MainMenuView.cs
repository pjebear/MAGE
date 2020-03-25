using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class MainMenuView : UIContainer
{
    public enum ComponentId
    {
        NewGameBtn,
        SaveFileBtns,

        NUM
    }

    public class DataProvider : IDataProvider
    {
        public List<string> SaveFiles = new List<string>();

        public override string ToString()
        {
            // TODO:
            return SaveFiles.Count.ToString();
        }
    }

    public UIButtonList SaveFileBtns;
    public UIButton NewGameBtn;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = dataProvider as DataProvider;

        List<UIButton.DataProvider> saveFileDPs = new List<UIButton.DataProvider>();
        foreach (string saveFile in dp.SaveFiles)
        {
            saveFileDPs.Add(new UIButton.DataProvider(saveFile, true));
        }
        SaveFileBtns.Publish(new UIButtonList.DataProvider(saveFileDPs));
    }

    protected override void InitComponents()
    {
        SaveFileBtns.Init((int)ComponentId.SaveFileBtns, this);
        NewGameBtn.Init((int)ComponentId.NewGameBtn, this);
    }

    protected override void InitSelf()
    {
        mId = (int)UIContainerId.MainMenuView;
        mContainerName = UIContainerId.MainMenuView.ToString();
    }

    protected override IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo)
    {
        return interactionInfo;
    }
}
