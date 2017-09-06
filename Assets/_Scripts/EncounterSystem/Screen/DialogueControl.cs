using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueControl : MonoBehaviour {


    public Image SpeakerImage;
    public Text SpeakerName;
    public Text DialogContent;

	// Use this for initialization
	void Start () {
        Debug.Assert(SpeakerImage != null);
        Debug.Assert(SpeakerName != null);
        Debug.Assert(DialogContent != null);
    }
	
	public void DisplayDialogue(Sprite sprite, string name, string content)
    {
        SpeakerImage.sprite = sprite;
        SpeakerName.text = name;
        DialogContent.text = content;
    }
}
