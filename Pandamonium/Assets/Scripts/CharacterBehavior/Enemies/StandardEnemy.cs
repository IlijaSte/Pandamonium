using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemy : Enemy {

    protected IEnumerator Frozen(float duration)
    {
        speed /= 2;
        if(path)
            path.maxSpeed /= 2;

        float dur = 0;

        while(dur < duration)
        {
            if (isDead)
                yield break;

            ColorTransition(Color.blue);
            yield return new WaitForSeconds(0.5f);
            dur += 0.5f;
            print("tick");
        }
        

        speed *= 2;

        if (path)
            path.maxSpeed *= 2;
    }

    public void Freeze(float duration)
    {
        StartCoroutine(Frozen(duration));
    }

    protected override void Die()
    {
        base.Die();

        DropCoins();

        if (GameManager.I.currentLevel > 0 && holdsKey)
        {
            DropKey();
        }

        room.enemies.Remove(gameObject);

        if (room.enemies.Count == 0)
        {
            InfoText.I.ShowMessage("clear");

            if(room.getRoomHolder().chest != null)
                room.getRoomHolder().chest.GetComponentInChildren<Chest>().locked = false;
        }
    }
}
