using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class InteractionFlowControl : MonoBehaviour,
    UIContainerControl, IInputHandler
{
    private string TAG = "InteractionFlowControl";

    private IInteractable mHoveredInteractable = null;
    private IInteractable mInteractingWith = null;

    void Start()
    {
        InputManager.Instance.RegisterHandler(this, false);
    }

    void OnDestroy()
    {
        InputManager.Instance.ReleaseHandler(this);
    }

    //! UI ContainerControl
    public void HandleComponentInteraction(int containerId, IUIInteractionInfo interactionInfo)
    {
        throw new NotImplementedException();
    }

    public string Name()
    {
        return TAG;
    }

    public IDataProvider Publish()
    {
        throw new NotImplementedException();
    }

    //! IInputHandler
    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (mInteractingWith == null)
        {
            switch (source)
            {
                case InputSource.Mouse:
                    {
                        if (key == (int)MouseKey.Right
                            && state == InputState.Down
                            && mHoveredInteractable != null)
                        {
                            Interact(mHoveredInteractable);
                        }
                    }
                    break;
            }
        }
        
    }

    public void OnMouseHoverChange(GameObject mouseHover)
    {
        Debug.Log(mouseHover);
        if (mouseHover != null)
        {
            mHoveredInteractable = mouseHover.GetComponentInParent<IInteractable>();
        }
        else
        {
            mHoveredInteractable = null;
        }
    }

    private void Interact(IInteractable interactable)
    {
        GameSystemModule.Instance.PrepareEncounter(new EncounterCreateParams());
        GameModesModule.Instance.Encounter();
    }
}

