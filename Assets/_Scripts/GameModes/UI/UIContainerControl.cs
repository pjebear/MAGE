using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


interface UIContainerControl
{
    IDataProvider Publish(int containerId);

    void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo);
    string Name();
}

