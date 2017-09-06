using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarkerController : MonoBehaviour {

    public float VertDistance = .3f;
    public float RotationSpeed = 2 * Mathf.PI;
    public float HoverSpeed = 1f;
    public float HoverRange = 0.2f;
    public float OffsetFromUnit = 0.8f;

    private int direction = 1;


    public void MarkUnit(GameObject unit)
    {
        gameObject.SetActive(true);
        transform.parent = unit.transform;
        transform.localPosition = Vector3.up * OffsetFromUnit;
        direction = 1;
    }
	
	// Update is called once per frame
	void Update () {

        float offsetFromUnit = transform.localPosition.y;
        if (offsetFromUnit <= OffsetFromUnit && direction < 0)
        {
            direction = 1;
        }
        else if (offsetFromUnit >= OffsetFromUnit + HoverRange && direction > 0)
        {
            direction = -1;
        }

        transform.localPosition += Vector3.up * Time.deltaTime * HoverSpeed * direction;
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
	}

    public void Hide()
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
}
