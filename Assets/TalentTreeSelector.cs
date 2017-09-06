using Screens.Payloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldSystem.Character;
using WorldSystem.Talents;

public class TalentTreeSelector : MonoBehaviour {

    public Button[] TalentSelectorButtons;
    public int[] TalentIndices;
    public Text NumFreePointsText;

    private CharacterBase mCharacterBase;
    
    private void Awake()
    {
        Debug.Assert(TalentSelectorButtons.Length == TalentIndices.Length);
    }
    
    public void Initialize(CharacterBase character)
    {
        gameObject.SetActive(true);
        mCharacterBase = character;
        UpdateTalentTree();
    }
	
	private void UpdateTalentTree()
    {
        TalentTreePayload payload = mCharacterBase.GetTalentTreePayload();
        NumFreePointsText.text = payload.AvailablePoints.ToString();
        foreach (var pair in payload.TalentPayloads)
        {
            Button talentButton = null;
            //find which button it is
            for (int i = 0; i < TalentIndices.Length; i++)
            {
                if (TalentIndices[i] == (int)pair.Key)
                    talentButton = TalentSelectorButtons[i];
            }
            talentButton.interactable = pair.Value.IsUnlocked;
            Image unlockBar = talentButton.transform.Find("UnlockBar").GetComponent<Image>();
            Color toAssign = unlockBar.color;
            toAssign.a = pair.Value.CurrentPoints > 0 ? 1f : .2f;
            unlockBar.color = toAssign;
            talentButton.transform.Find("TalentName").GetComponent<Text>().text = pair.Key.ToString();
            talentButton.transform.Find("PointsSpent").GetComponent<Text>().text = string.Format("{0}/{1}", pair.Value.CurrentPoints, pair.Value.MaxPoints);
            talentButton.onClick.RemoveAllListeners();
            talentButton.onClick.AddListener(() => _ApplyPointToTalent(pair.Key));
        }
    }

    private void _ApplyPointToTalent(TalentIndex talentIndex)
    {
        mCharacterBase.ApplyTalentPoint(talentIndex);
        UpdateTalentTree();
    }
}
