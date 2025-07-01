using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUser : MonoBehaviour
{
    public int producingPower;
    public int consumingPower;

    public bool isProducer;

    public void PowerOn()
    {
        if (isProducer)
        {
            PowerManager.Instance.AddPower(producingPower);
        }
        else
        {
            PowerManager.Instance.ConsumePower(consumingPower);
        }
    }
    private void OnDestroy()
    {
        if (GetComponent<Constructable>().inPreviewMode == false)
        {
            if (isProducer)
            {
                PowerManager.Instance.RemovePower(producingPower);
            }
            else
            {
                PowerManager.Instance.ReleasePower(consumingPower);
            }
        }
    }
}
