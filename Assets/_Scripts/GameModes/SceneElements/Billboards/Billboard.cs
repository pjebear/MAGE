using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Billboard : MonoBehaviour
{
    public struct Params
    {
        public string text;
        public Transform anchor;
        public Vector3 offset;

        public Params(string text, Transform anchor, Vector3 offset)
        {
            this.text = text;
            this.anchor = anchor;
            this.offset = offset;
        }
    }

    public TMPro.TextMeshPro Text;
    public SpriteRenderer SpriteRenderer;
    public Transform Anchor;
    public Vector3 AnchorOffset;

    private void Awake()
    {
        Anchor = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOrientation();
    }

    void UpdateOrientation()
    {
        transform.position = Anchor.position + AnchorOffset;

        transform.LookAt(Camera.main.transform);
    }

    public void Init(Params data)
    {
        Text.text = data.text;
        Anchor = data.anchor != null ? data.anchor : transform;
        AnchorOffset = data.offset;

        UpdateOrientation();
    }

   
}
