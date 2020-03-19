using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PlayAnimation : MonoBehaviour
{
    public ActorDirector ActorDirector;
    public AnimationDirector AnimationDirector;
    public AnimationId AnimationId; 

    // Start is called before the first frame update
    void Start()
    {
        GameObject main = GameObject.Find("Main");
        ActorDirector = main.GetComponent<ActorDirector>();
        AnimationDirector = main.GetComponent<AnimationDirector>();
    }

    public void Trigger()
    {
        RuntimeAnimation animation = new RuntimeAnimation();
        animation.Duration = 3f;
        animation.SyncPoint = 1.5f;
        animation.Name = AnimationId.ToString();

        TileIdx leadTile = new TileIdx(0, 0);
        TileIdx targetTile = new TileIdx(0, 1);

        //ActionBase swordAttack = ActionFactory.CreateActionFromId(
        //    MapDirector.Instance.Map[leadTile].OnTile.mActor, 
        //    ActionId.SwordAttack,
        //    new TargetSelection(new Target(targetTile)));

        //EncounterModule.ActionDirector.QueueAction(swordAttack);
    }
}
