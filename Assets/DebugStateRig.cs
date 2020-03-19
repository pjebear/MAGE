using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class DebugStateRig : MonoBehaviour
{
    public ActorController ActorController;
    public Text HealthText;
    public RectTransform HealthRect;
    public Text ResourceText;
    public RectTransform ResourceRect;

    public Vector3 Offset;

    private void Update()
    {
        if (ActorController != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(ActorController.transform.position) + Offset;

            float healthScale = ActorController.mActor.Resources[ResourceType.Health].Ratio;
            HealthRect.localScale = new Vector3(healthScale, 1, 0);
            HealthText.text = string.Format("{0}/{1}", ActorController.mActor.Resources[ResourceType.Health].Current, ActorController.mActor.Resources[ResourceType.Health].Max);

            float resourceScale = ActorController.mActor.Resources[ResourceType.Mana].Ratio;
            ResourceRect.localScale = new Vector3(resourceScale, 1, 0);
            HealthText.text = string.Format("{0}/{1}", ActorController.mActor.Resources[ResourceType.Mana].Current, ActorController.mActor.Resources[ResourceType.Mana].Max);
        }
    }
}
