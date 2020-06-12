using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class EncounterUnitPlacementView : UIContainer
{
    public enum ComponentId
    {
        CharacterSelectLeftBtn,
        CharacterSelectRightBtn,
        AutoFillBtn,
        ConfirmBtn,

        NUM
    }

    public class DataProvider : IDataProvider
    {
        public string character = "";
    }

    public UIButton ConfirmButton;
    public UIButton AutoFillButton;
    public UIButton CharacterSelectLeftBtn;
    public UIText SelectedCharacterText;
    public UIButton CharacterSelectRightBtn;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = dataProvider as DataProvider;

        SelectedCharacterText.Publish(new UIText.DataProvider(dp.character));

        ConfirmButton.Publish(new UIButton.DataProvider("Confirm", true));
        AutoFillButton.Publish(new UIButton.DataProvider("Auto Assign", true));
    }

    protected override void InitChildren()
    {
        CharacterSelectLeftBtn.Init((int)ComponentId.CharacterSelectLeftBtn, this);
        CharacterSelectRightBtn.Init((int)ComponentId.CharacterSelectRightBtn, this);
        AutoFillButton.Init((int)ComponentId.AutoFillBtn, this);
        ConfirmButton.Init((int)ComponentId.ConfirmBtn, this);
    }
}
