using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




using WorldSystem;
public class TitleScreenManager : MonoBehaviour {

    private void Awake()
    {
        
    }

    public void NextScene(string scene)
    {
        SceneManager.LoadScene(scene);
        
    }

    public void OnClick_NewGame()
    {
        
    }

    public void OnClick_LoadGame()
    {

    }
}
