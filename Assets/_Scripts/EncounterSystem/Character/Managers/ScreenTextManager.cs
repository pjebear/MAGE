using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem.Character.Managers
{
    using Common.ActionEnums;
    using Common.ActionTypes;
    using Common.AttributeEnums;
    using Screen;

    class ScreenTextManager
    {
        private ScreenTextDisplayQueue mScreenTextQueue = null;

        public ScreenTextManager()
        {
        }

        public void Initialize(ScreenTextDisplayQueue displayQueue)
        {
            mScreenTextQueue = displayQueue;
        }

        public void DisplayResourceChangedText(ResourceChange changedResource, InteractionResult interactionResult = InteractionResult.Hit, AvoidanceResult avoidanceResult = AvoidanceResult.Invalid)
        {
            string resourceDisplay = "";
            if (avoidanceResult != AvoidanceResult.Invalid)
            {
                resourceDisplay = avoidanceResult.ToString();
                if (changedResource.Value != 0)
                {
                    resourceDisplay += string.Format("({2}{0:0}{1})",changedResource.Value,
                        interactionResult == InteractionResult.Crit ? "!" : "",
                        changedResource.Value > 0 ? "+" : "");
                }
            }
            else
            {
                resourceDisplay = string.Format("{2}{0:0}{1}", changedResource.Value,
                         interactionResult == InteractionResult.Crit ? "!" : "",
                         changedResource.Value > 0 ? "+" : "");
            }

            Color color = Color.white;
            switch (changedResource.Resource)
            {
                case (Resource.Health):
                    color = Color.white;
                    break;
                case (Resource.Mana):
                    color = Color.blue;
                    break;
                case (Resource.Endurance):
                    color = Color.green;
                    break;
            }
            mScreenTextQueue.QueueOnScreenText(resourceDisplay, color);
        }

        public void DisplayExperienceGainedText(float experienceGained)
        {
            mScreenTextQueue.QueueOnScreenText("Exp: " + ((int)experienceGained).ToString(), Color.white);
        }

        public void DisplayProfExperienceGainedText(float experienceGained)
        {
            mScreenTextQueue.QueueOnScreenText("Pxp: " + ((int)experienceGained).ToString(), Color.white);
        }

        public void DisplayLevelUpText()
        {
            mScreenTextQueue.QueueOnScreenText("CLv Up!", Color.white);
        }

        public void DisplayProfLevelUpText()
        {
            mScreenTextQueue.QueueOnScreenText("PLv Up!", Color.white);
        }

        public void DisplayStatusChangedText(string statusChange, int numStacksAdded)
        {
            string tag = statusChange;
            if (numStacksAdded > 1 )
            {
                tag += " +" + numStacksAdded; 
            }
            mScreenTextQueue.QueueOnScreenText(tag, numStacksAdded > 0 ? Color.white : Color.grey);
        }

        public void DisplayText(string text, Color textColor)
        {
            mScreenTextQueue.QueueOnScreenText(text, textColor);
        }
    }
}
