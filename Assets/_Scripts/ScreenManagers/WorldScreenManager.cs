using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using WorldSystem.Interface;

public class WorldScreenManager : MonoBehaviour
{

    private WorldSystemFacade rWorldSystemFacade;

    private void Awake()
    {
        rWorldSystemFacade = WorldSystemFacade.Instance();
        //load where the characer is in the game     
    }

    public void OnClick_Save()
    {
        //mGameInstance.SaveGameInstance();
    }

    public void OnClick_Encounter()
    {
        rWorldSystemFacade.BeginRandomEncounter();
    }

    public void OnClick_PartyInspectorScreen()
    {
        Debug.Log("Opening PartyInspector HUD...");
        rWorldSystemFacade.OpenPartyScreen();
    }

    public void OnClick_ItemShopScreen()
    {
        Debug.Log("Opening ItemShop HUD...");
    }

    public void OnClick_UnitRecruitmentScreen()
    {
        Debug.Log("Opening Unit Recruitment HUD...");
    }

    public void OnClick_QuitGame()
    {
        //TODO: prompt save : if (mGameInstance.InstanceDirty) //prompt user to save unsaved changes
            Debug.Log("Unsaved changes lost");
        //mGameInstance.DestroyInstance();
        SceneManager.LoadScene("TitleScreen");
    }


}
