using MAGE.GameModes.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.UI.Views
{
    class EncounterCharacterHoverView : UIContainer
    {
        public enum ComponentId
        {
            
        }

        public class DataProvider : IDataProvider
        {
            public Dictionary<Transform, CharacterHoverInspector.DataProvider> CharacterDPs = new Dictionary<Transform, CharacterHoverInspector.DataProvider>();
        }

        private List<CharacterHoverInspector> mInactiveInspectors = new List<CharacterHoverInspector>();
        private Dictionary<Transform, CharacterHoverInspector> mActiveInspectors = new Dictionary<Transform, CharacterHoverInspector>();

        public override void Init(int id, UIContainer container)
        {
            base.Init(id, container);

            CharacterHoverInspector characterHoverInspector = Resources.Load<CharacterHoverInspector>("UI/Components/CustomComponents/CharacterHoverInspector");
            for (int i = 0; i < 10; ++i)
            {
                CharacterHoverInspector instantiated = Instantiate(characterHoverInspector, transform);
                instantiated.gameObject.SetActive(false);
                mInactiveInspectors.Add(instantiated);
            }
        }

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = (DataProvider)dataProvider;

            // Remove inactive inspectors
            {
                List<Transform> inactiveInspectors = new List<Transform>();

                foreach (var inspectorTransformPair in mActiveInspectors)
                {
                    if (!dp.CharacterDPs.ContainsKey(inspectorTransformPair.Key))
                    {
                        inactiveInspectors.Add(inspectorTransformPair.Key);
                    }
                }

                foreach (Transform toRemove in inactiveInspectors)
                {
                    CharacterHoverInspector toDisable = mActiveInspectors[toRemove];
                    mActiveInspectors.Remove(toRemove);

                    toDisable.gameObject.SetActive(false);
                    mInactiveInspectors.Add(toDisable);
                }
            }

            // Add new inspectors
            foreach (var inspectorTransformPair in dp.CharacterDPs)
            {
                CharacterHoverInspector toPublish = null;
                if (!mActiveInspectors.ContainsKey(inspectorTransformPair.Key))
                {
                    if (mInactiveInspectors.Count > 0)
                    {
                        toPublish = mInactiveInspectors[mInactiveInspectors.Count - 1];
                        mInactiveInspectors.RemoveAt(mInactiveInspectors.Count - 1);
                        toPublish.gameObject.SetActive(true);
                    }
                    else
                    {
                        toPublish = Instantiate(Resources.Load<CharacterHoverInspector>("UI/Components/CustomComponents/CharacterHoverInspector"), transform);
                    }

                    mActiveInspectors.Add(inspectorTransformPair.Key, toPublish);
                }
                else
                {
                    toPublish = mActiveInspectors[inspectorTransformPair.Key];
                }

                toPublish.Publish(inspectorTransformPair.Value);
            }
        }

        private void LateUpdate()
        {
            float zoomLevel = Mathf.Clamp01(Camera.main.GetComponent<CameraController>().rTopDownCamera.m_YAxis.Value);
            float scale = 1 - zoomLevel * .3f;
            float vertOffset = 2.4f - .4f * (1 - zoomLevel);

            foreach (var inspectorTransformPair in mActiveInspectors)
            {
                inspectorTransformPair.Value.transform.position = Camera.main.WorldToScreenPoint(inspectorTransformPair.Key.position + Vector3.up * vertOffset);
                inspectorTransformPair.Value.transform.localScale = Vector3.one * scale;
            }
        }

        protected override void InitChildren()
        {
            
        }
    }
}

