using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z - 25f), step);        

        if (this.transform.localPosition.z < -24.5f)
        {
            Destroy(this.gameObject);
        }
    }
}
