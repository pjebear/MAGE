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
        public List<string> SaveFiles;
    }

    public UIList SaveFileBtns;
    public UIButton NewGameBtn;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = dataProvider as DataProvider;

        List<IDataProvider> buttonDPs = new List<IDataProvider>();
        foreach (string saveFile in dp.SaveFiles)
        {
            buttonDPs.Add(new UIButton.DataProvider(saveFile, true));
        }
        SaveFileBtns.Publish(new UIList.DataProvider(buttonDPs));
    }

    protected override void InitChildren()
    {
        SaveFileBtns.Init((int)ComponentId.SaveFileBtns, this);
        NewGameBtn.Init((int)ComponentId.NewGameBtn, this);
    }
}
