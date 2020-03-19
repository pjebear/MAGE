using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EffectSpawner : MonoBehaviour
{
    public Effect FirePrefab;
    public Effect HealPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEffect(EffectPlaceholder effectInfo)
    {
        Effect effect = null;
        switch (effectInfo.EffectType)
        {
            case (EffectType.Fire):
                effect = Instantiate(FirePrefab, effectInfo.SpawnParent);
                break;
            case (EffectType.Heal):
                effect = Instantiate(HealPrefab, effectInfo.SpawnParent);
                break;
            default:
                Debug.Assert(false);
                break;
        }

        StartCoroutine(SpawnEffectFor(effect, effectInfo.NumFrames / (float)AnimationConstants.FRAMES_PER_SECOND));
    }

    IEnumerator SpawnEffectFor(Effect effect, float duration)
    {
        yield return new WaitForSeconds(duration);

        Destroy(effect.gameObject);
    }
}
