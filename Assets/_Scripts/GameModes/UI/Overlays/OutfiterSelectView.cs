using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class OutfiterSelectView : UIContainer
{
    public enum ComponentId
    {
        ExitBtn,
        CharacterSelectLeftBtn,
        CharacterSelectRightBtn,

        NUM
    }

    public class DataProvider : IDataProvider
    {
        public string character = "";

        public override string ToString()
        {
            // TODO:
            return character;
        }
    }

    public UIButton ExitButton;
    public UIButton CharacterSelectLeftBtn;
    public UIText SelectedCharacterText;
    public UIButton CharacterSelectRightBtn;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = dataProvider as DataProvider;

        SelectedCharacterText.Publish(new UIText.DataProvider(dp.character));
    }

    protected override void InitComponents()
    {
        CharacterSelectLeftBtn.Init((int)ComponentId.CharacterSelectLeftBtn, this);
        CharacterSelectRightBtn.Init((int)ComponentId.CharacterSelectRightBtn, this);
        ExitButton.Init((int)ComponentId.ExitBtn, this);
    }

    protected override void InitSelf()
    {
        mId = (int)UIContainerId.OutfiterSelectView;
        mContainerName = UIContainerId.OutfiterSelectView.ToString();
    }

    protected override IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo)
    {
        return interactionInfo;
    }
}
