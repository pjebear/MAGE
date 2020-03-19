using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BillboardEmitter : MonoBehaviour
{
    class QueuedBillboard
    {
        public Billboard.Params billboardParams;
        public float duration;
    }

    private Queue<QueuedBillboard> mBillboardQueue = new Queue<QueuedBillboard>();
    private QueuedBillboard mCurrentBillboard;
    public Billboard BillboardPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Emitt(Billboard.Params data, float duration)
    {
        QueuedBillboard queuedBillboard = new QueuedBillboard();
        queuedBillboard.billboardParams = data;
        queuedBillboard.duration = duration;

        mBillboardQueue.Enqueue(queuedBillboard);

         ProgressQueue();
    }

    private void ProgressQueue()
    {
        if (mBillboardQueue.Count > 0)
        {
            if (mCurrentBillboard == null)
            {
                mCurrentBillboard = mBillboardQueue.Dequeue();
                Billboard billboard = Instantiate(BillboardPrefab, transform);
                billboard.Init(mCurrentBillboard.billboardParams);

                StartCoroutine(EmittFor(billboard, mCurrentBillboard.duration));
            }
        }
        else
        {
            mCurrentBillboard = null;
        }
    }

    IEnumerator EmittFor(Billboard billboard, float duration)
    {
        yield return new WaitForSeconds(duration);

        Destroy(billboard.gameObject);

        mCurrentBillboard = null;

        ProgressQueue();
    }
}
