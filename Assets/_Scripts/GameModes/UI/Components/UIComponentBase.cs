using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

class IDataProvider
{
    public static IDataProvider Empty { get { return new IDataProvider(); } }
    protected IDataProvider() { }
}

abstract class UIComponentBase : MonoBehaviour
{
    public static int INVALID_ID = -1;
    public int mId = INVALID_ID;

    protected UIContainer mContainer = null;

    // ----------------------------------------------------------------------------------------------- //
    public virtual void Init(int id, UIContainer container)
    {
        mId = id;
        mContainer = container;
    }

    void Start()
    {
       
    }



    public abstract void Publish(IDataProvider dataProvider);
}

