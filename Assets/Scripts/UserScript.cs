using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScript : MonoBehaviour
{
    public Transform moveTarget;
    public float speed = 10;

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        this.transform.position = Vector3.MoveTowards(this.transform.position, moveTarget.position, step);

        if (Vector3.Distance(this.transform.position, moveTarget.position) < 0.1f)
        {
            RelocateMoveTarget();
        }
    }

    // Helper Functions
    public void RelocateMoveTarget()
    {
        moveTarget.localPosition = new Vector3(Random.Range(-49.0f, 49.0f), 0.5f, Random.Range(-49.0f, 49.0f));
    }

    public void ResetUser()
    {
        this.transform.localPosition = new Vector3(0, 1, -5);
    }
}
