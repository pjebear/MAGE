using System.Collections.Generic;
using UnityEngine;


class UIList : UIContainer
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
    public Transform ElementContainer;
    public Vector2 ListDirection = Vector2.right;
    public float Padding = 10f;

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
        else if (Elements.Count > numElements)
        {
            for (int i = numElements; i < Elements.Count; ++i)
            {
                Elements[i].gameObject.SetActive(false);
            }
        }
        
        for (int i = 0; i < numElements; ++i)
        {
            Elements[i].Publish(dp.Elements[i]);
        }
    }

    protected override UIInteractionInfo ModifyInteractionInfo(UIInteractionInfo interactionInfo)
    {
        return new ListInteractionInfo(mId, interactionInfo);
    }

    private UIComponentBase AddElement()
    {
        GameObject newElement = Instantiate(ElementPrefab, ElementContainer);

        Rect elementRect = ElementPrefab.GetComponent<RectTransform>().rect;
        Vector2 ElementDimensions = new Vector2(elementRect.width, elementRect.height) + Vector2.one * Padding;

        int elementIdx = Elements.Count;
        Vector2 localPos = Vector2.zero;
        if (ListDirection == Vector2.left || ListDirection == Vector2.right)
        {
            localPos.x = ElementDimensions.x * elementIdx * (ListDirection == Vector2.left ? -1 : 1);
        }
        else
        {
            localPos.y = ElementDimensions.y * elementIdx * (ListDirection == Vector2.down ? -1 : 1);
        }

        newElement.transform.localPosition = localPos;
        newElement.SetActive(true);

        UIComponentBase component = newElement.GetComponent<UIComponentBase>();
        component.Init(elementIdx, this);
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

