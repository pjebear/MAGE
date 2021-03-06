﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.UI.Views
{
    class OutfiterSelectView : UIContainer
    {
        public enum ComponentId
        {
            ExitBtn,
            CharacterSelectLeftBtn,
            CharacterSelectRightBtn,
            EquipBtn,
            SpecBtn,

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
        public UIButton SpecBtn;
        public UIButton EquipBtn;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = dataProvider as DataProvider;

            SelectedCharacterText.Publish(new UIText.DataProvider(dp.character));
        }

        protected override void InitChildren()
        {
            CharacterSelectLeftBtn.Init((int)ComponentId.CharacterSelectLeftBtn, this);
            CharacterSelectRightBtn.Init((int)ComponentId.CharacterSelectRightBtn, this);
            SpecBtn.Init((int)ComponentId.SpecBtn, this);
            EquipBtn.Init((int)ComponentId.EquipBtn, this);
            ExitButton.Init((int)ComponentId.ExitBtn, this);
        }
    }

}
