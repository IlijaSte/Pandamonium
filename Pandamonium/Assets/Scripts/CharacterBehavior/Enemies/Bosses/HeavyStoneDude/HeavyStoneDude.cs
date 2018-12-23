using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyStoneDude : Boss {

    public void GoToPool(Vector2 poolPos)
    {
        MoveToPosition(poolPos);
    }

}
