using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class EncounterCharacterInfoView : UIContainer
{
    private string TAG = "EncounterCharacterInfoView";

    public enum ComponentId
    {
       
    }


    public class DataProvider : IDataProvider
    {
        public Optional<bool> IsAlly; 
        public Optional<string> PortraitAsset;
        public Optional<string> Name;
        public Optional<string> Specialization;
        public Optional<int> Level;
        public Optional<int> Exp;
        public Optional<int> CurrentHP;
        public Optional<int> MaxHP;
        public Optional<int> CurrentMP;
        public Optional<int> MaxMP;
        public Optional<int> Might;
        public Optional<int> Finesse;
        public Optional<int> Magic;
        public Optional<int> Fortitude;
        public Optional<int> Attunement;
        public Optional<int> Block;
        public Optional<int> Dodge;
        public Optional<int> Parry;
        public Optional<UIList.DataProvider> StatusEffects;
    }

    public Image InfoBacking;
    public UIImage HeadImg;
    public UIText NameTxt;
    public UIText LevelTxt;
    public UIText ExpTxt;
    public UIText SpecializationTxt;
    public UIBar HPBar;
    public UIBar MPBar;
    public UIText MightTxt;
    public UIText FinesseTxt;
    public UIText MagicTxt;
    public UIText FortitudeTxt;
    public UIText AttunementTxt;
    public UIText BlockTxt;
    public UIText DodgeTxt;
    public UIText ParryTxt;
    public UIList StatusIcons;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = (DataProvider)dataProvider;

        // Info Backing
        if (dp.IsAlly.HasValue) InfoBacking.color = (dp.IsAlly.Value ? Color.blue : Color.red) * 0.5f;

        // Portrait
        if (dp.PortraitAsset.HasValue) HeadImg.Publish(dp.PortraitAsset.Value);

        // Name
        if (dp.Name.HasValue) NameTxt.Publish(dp.Name.Value);

        // Level
        if (dp.Level.HasValue) LevelTxt.Publish(string.Format("Lv.{0}", dp.Level.Value));

        // Exp
        if (dp.Exp.HasValue) ExpTxt.Publish(string.Format("Exp.{0}", dp.Exp.Value));

        // Specialization
        if (dp.Specialization.HasValue) SpecializationTxt.Publish(dp.Specialization.Value);

        // HP
        if (dp.CurrentHP.HasValue && dp.MaxHP.HasValue) HPBar.Publish("HP", dp.CurrentHP.Value, dp.MaxHP.Value);

        // MP
        if (dp.CurrentMP.HasValue && dp.MaxMP.HasValue) MPBar.Publish("MP", dp.CurrentMP.Value, dp.MaxMP.Value);

        // Might
        if (dp.Might.HasValue) MightTxt.Publish(dp.Might.Value.ToString());

        // Finesse
        if (dp.Finesse.HasValue) FinesseTxt.Publish(dp.Finesse.Value.ToString());
        
        // Magic
        if (dp.Magic.HasValue) MagicTxt.Publish(dp.Magic.Value.ToString());
       
        // Fortitude
        if (dp.Fortitude.HasValue) FortitudeTxt.Publish(dp.Fortitude.Value.ToString());
       
        // Attunement
        if (dp.Attunement.HasValue) AttunementTxt.Publish(dp.Attunement.Value.ToString());
        
        // Block
        if (dp.Block.HasValue) BlockTxt.Publish(dp.Block.Value.ToString());

        // Dodge
        if (dp.Dodge.HasValue) DodgeTxt.Publish(dp.Dodge.Value.ToString());
        
        // Parry
        if (dp.Parry.HasValue) ParryTxt.Publish(dp.Parry.Value.ToString());
        
        // StatusEffects
        if (dp.StatusEffects.HasValue) StatusIcons.Publish(dp.StatusEffects.Value);
    }

    protected override void InitChildren()
    {
        // empty
    }
}

