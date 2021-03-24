using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    enum InteractionType
    {
        Combat,
        OpenClose,
        NPC,
    }

    enum InteractionRange
    {
        Short,
        Medium,
        Far,
        
        NUM
    }

    class Interactable : MonoBehaviour
    {
        public InteractionType InteractionType;
        public InteractionRange InteractionRange;
        public static float FAR_RANGE = 15f;
        public static float MEDIUM_RANGE = 10f;
        public static float SHORT_RANGE = 5f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, GetInteractionRange());
            Gizmos.DrawIcon(transform.position, "Enemy");
        }

        public float GetInteractionRange()
        {
            float range = FAR_RANGE;

            switch (InteractionRange)
            {
                case InteractionRange.Far: range = FAR_RANGE; break;
                case InteractionRange.Medium: range = MEDIUM_RANGE; break;
                case InteractionRange.Short: range = SHORT_RANGE; break;
            }

            return range;
        }
    }
}
