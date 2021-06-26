using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class UIGrid : UIContainer
    {
        public class DataProvider : IDataProvider
        {
            public List<IDataProvider> Elements = new List<IDataProvider>();

            public DataProvider() { }
            public DataProvider(List<IDataProvider> elements)
            {
                Elements = elements;
            }
        }

        public GameObject ElementPrefab;
        public GridLayoutGroup Grid;

        public List<UIComponentBase> Elements = new List<UIComponentBase>();

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            int numElements = dp.Elements.Count;

            if (Elements.Count < numElements)
            {
                for (int i = Elements.Count; i < numElements; ++i)
                {
                    AddElement();
                }
            }

            for (int i = 0; i < Elements.Count; ++i)
            {
                if (i < numElements)
                {
                    Elements[i].Publish(dp.Elements[i]);
                    Elements[i].gameObject.SetActive(true);
                }
                else
                {
                    Elements[i].gameObject.SetActive(false);
                }
            }
        }

        protected override UIInteractionInfo ModifyInteractionInfo(UIInteractionInfo interactionInfo)
        {
            return new ListInteractionInfo(mId, interactionInfo);
        }

        private UIComponentBase AddElement()
        {
            GameObject newElement = Instantiate(ElementPrefab, Grid.transform);

            Rect elementRect = ElementPrefab.GetComponent<RectTransform>().rect;
            Grid.cellSize = new Vector3(elementRect.width, elementRect.height);

            newElement.SetActive(true);

            UIComponentBase component = newElement.GetComponent<UIComponentBase>();
            component.IsClickable = IsClickable;
            component.IsHoverable = IsHoverable;
            component.Init(Elements.Count, this);
            Elements.Add(component);

            return component;
        }

        protected override void InitChildren()
        {
            for (int i = 0; i < Elements.Count; ++i)
            {
                Elements[i].Init(i, this);
            }
        }
    }
}