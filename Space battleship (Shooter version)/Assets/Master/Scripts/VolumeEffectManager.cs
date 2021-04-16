using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeEffectManager : MonoBehaviour
{
    public Volume[] volumes;

    public static VolumeEffectManager current;

    private void OnValidate()
    {
        current = this;
    }

    public void activateVolumeEffect(int id, bool deactivateOtherEffect)
    {
        if(id < 0)
        {
            volumes[id].enabled = false;
            if(deactivateOtherEffect)
            {
                for (int i = 0; i < volumes.Length; i++)
                {
                    volumes[i].enabled = false;
                }
            }
            return;
        }
        else
        {
            volumes[id].enabled = true;
            for (int i = 0; i < volumes.Length; i++)
            {
                if(i != id && deactivateOtherEffect)
                {
                    volumes[i].enabled = false;
                }
            }
        }
    }

    public void deactivateVolumeEffect(int id)
    {
        if (id >= 0)
        {
            volumes[id].enabled = false;
        }
        else
        {
            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i].enabled = false;
            }
        }
    }
}
