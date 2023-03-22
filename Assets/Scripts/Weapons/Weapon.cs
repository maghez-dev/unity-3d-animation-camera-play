using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour 
{
    protected int animLayerIdx;
    public int attackCombo;

    public abstract void SetUp(GameObject playerObj);

    public int GetLayerIdx()
    {
        return animLayerIdx;
    }
}
