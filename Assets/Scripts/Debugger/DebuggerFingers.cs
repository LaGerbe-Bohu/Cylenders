using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggerFingers : MonoBehaviour
{

    public List<Transform> listFingers;
    private void OnDrawGizmos()
    {
        for (int i = 0; i < listFingers.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(listFingers[i].position,.02f);
        }
    }

}
