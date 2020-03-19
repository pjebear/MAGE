using UnityEngine;


interface IInputHandler
{
    void OnMouseHoverChange(GameObject mouseHover);
    void OnKeyPressed(InputSource source, int key, InputState state);
}



