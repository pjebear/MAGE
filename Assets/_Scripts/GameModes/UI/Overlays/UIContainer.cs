using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace MAGE.UI.Views
{

    abstract class UIContainer : UIComponentBase
    {
        public override void Init(int id, UIContainer container)
        {
            base.Init(id, container);

            InitChildren();
        }

        protected abstract void InitChildren();

        protected virtual UIInteractionInfo ModifyInteractionInfo(UIInteractionInfo interactionInfo)
        {
            return interactionInfo;
        }

        public void HandleInteraction(UIInteractionInfo interactionInfo)
        {
            UIInteractionInfo modifiedInfo = ModifyInteractionInfo(interactionInfo);

            if (mContainer != null)
            {
                mContainer.HandleInteraction(modifiedInfo);
            }
            else
            {
                UIManager.Instance.ComponentInteracted(mId, modifiedInfo);
            }
        }
    }


}