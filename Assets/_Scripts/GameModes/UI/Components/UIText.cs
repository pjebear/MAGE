using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class UIText : UIComponentBase
{
    public class DataProvider : IDataProvider
    {
        public Optional<string> Text;

        public DataProvider(Optional<string> text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return "UITextDP";
        }
    }

    public string Text;
    public int FontSize;
    public Color FontColor;

    public TextMeshProUGUI TMPro;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = (DataProvider)dataProvider;

        if (dp.Text.HasValue) Text = dp.Text.Value;

        TMPro.text = Text;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
