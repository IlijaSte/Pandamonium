using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemy : Enemy {

    private volatile float freezeDuration = 0;

    private Coroutine freezeCoroutine;

    protected IEnumerator Frozen()
    {
        speed /= 2;
        if(path)
            path.maxSpeed /= 2;

        float dur = 0;

        while(dur < freezeDuration)
        {
            if (isDead)
                yield break;

            StartCoroutine(ColorTransition(Color.blue));
            yield return new WaitForSeconds(1f);
            dur += 1f;
        }
        

        speed *= 2;

        if (path)
            path.maxSpeed *= 2;

        freezeDuration = 0;
    }

    public void Freeze(float duration)
    {

        freezeDuration += duration;

        if (freezeCoroutine == null)
        {
            freezeCoroutine = StartCoroutine(Frozen());
        }
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

        if (room != LevelGeneration.I.bossRoom && room.enemies.Count == 0)
        {
            InfoText.I.ShowMessage("clear");

            if (room.getRoomHolder().chest != null)
                room.getRoomHolder().chest.GetComponentInChildren<Chest>().Unlock();
        }
    }
}
