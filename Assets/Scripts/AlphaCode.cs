using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CallListener : UnityEvent<int>
{
}

public class AlphaCode : MonoBehaviour
{
    public CallListener callListener;
    public int cmd = 0;
    // Start is called before the first frame update
    void Update()
    {
        if (callListener != null)
        {
            callListener.Invoke(cmd);
        }
    }
}
