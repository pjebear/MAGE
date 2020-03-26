using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PartyOutfiterViewControl : UIContainerControl
{
    private Transform mCharacterSpawnPoint;

    public List<ICharacterOutfiter> CharacterOutfiters;
    private ICharacterOutfiter mOutfiter;
    public int mOutfiterIdx = 0;

    private List<int> mCharacterIds = new List<int>();
    private int mCharacterIdx = 0;
    private DB.DBCharacter mOutfitingCharacter = null;

    public void Init(Transform characterSpawnPoint)
    {
        CharacterOutfiters = new List<ICharacterOutfiter>() { new SpecializationOutfiterViewControl() };
        mCharacterSpawnPoint = characterSpawnPoint;
    }

    public void Start()
    {
        mCharacterIds = GameSystemModule.Instance.GetCharactersInParty();
        mCharacterIdx = 0;
        mOutfitingCharacter = DB.DBHelper.LoadCharacter(mCharacterIds[mCharacterIdx]);
        SpawnCharacter();

        mOutfiter = CharacterOutfiters[mOutfiterIdx];
        mOutfiter.BeginOutfitting(mOutfitingCharacter, () => { SpawnCharacter(); });
        UIManager.Instance.PostContainer(UIContainerId.OutfiterSelectView, this);
    }

    public void Cleanup()
    {
        UIManager.Instance.RemoveOverlay(UIContainerId.OutfiterSelectView);
    }

    public void HandleComponentInteraction(int containerId, IUIInteractionInfo interactionInfo)
    {
        switch (containerId)
        {
            case (int)UIContainerId.OutfiterSelectView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        switch (interactionInfo.ComponentId)
                        {
                            case (int)OutfiterSelectView.ComponentId.CharacterSelectLeftBtn:
                                {
                                    CycleCharacter(-1);
                                }
                                break;

                            case (int)OutfiterSelectView.ComponentId.CharacterSelectRightBtn:
                                {
                                    CycleCharacter(1);
                                }
                                break;
                            case (int)OutfiterSelectView.ComponentId.ExitBtn:
                                {
                                    GameModesModule.Instance.Explore();
                                }
                                break;
                        }
                    }
                }
                break;
        }
    }

    public string Name()
    {
        return "PartyOutfiterViewControl";
    }

    public IDataProvider Publish()
    {
        OutfiterSelectView.DataProvider dataProvider = new OutfiterSelectView.DataProvider();

        dataProvider.character = mOutfitingCharacter.CharacterInfo.Name;

        return dataProvider;
    }

    private void CycleCharacter(int direction)
    {
        int newIdx = mCharacterIdx + direction;
        if (newIdx < 0) newIdx = mCharacterIds.Count - 1;
        if (newIdx == mCharacterIds.Count) newIdx = 0;

        if (newIdx != mCharacterIdx)
        {
            mCharacterIdx = newIdx;
            mOutfitingCharacter = DB.DBHelper.LoadCharacter(mCharacterIds[mCharacterIdx]);

            mOutfiter.Cleanup();

            mOutfiter.BeginOutfitting(mOutfitingCharacter, () => { SpawnCharacter(); });

            SpawnCharacter();
        }

        UIManager.Instance.Publish(UIContainerId.OutfiterSelectView);
    }

    private void SpawnCharacter()
    {
        if (mCharacterSpawnPoint.childCount > 0)
        {
            GameObject.Destroy(mCharacterSpawnPoint.GetChild(0).gameObject);
        }

        GameModesModule.ActorLoader.CreateActor(CharacterUtil.ActorParamsForCharacter(mOutfitingCharacter), mCharacterSpawnPoint);
    }
}

