using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

class UIButton : UIComponentBase
{
    public class DataProvider : IDataProvider
    {
        public string Text;
        public bool IsSelectable;

        public DataProvider(string text, bool isSelectable)
        {
            Text = text;
            IsSelectable = isSelectable;
        }

        public override string ToString()
        {
            return string.Format("[{0}|{1}]", Text, IsSelectable ? "True" : "False");
        }
    }

    public Button Button;
    public TextMeshProUGUI Text;

    private void Awake()
    {
        Button.onClick.AddListener(()=> { OnPointerClick(null); });
    }

    public override void Publish(IDataProvider dataProvider)
    {
        if (dataProvider != null)
        {
            DataProvider dp = (DataProvider)dataProvider;
        
            Text.text = dp.Text;
            Button.interactable = dp.IsSelectable;
        }
    }
}

