using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TripEvent : ShrineEvent {

    public int numberOfIndicators = 3;
    public int percentByIndicator = 5;

    public int secondsToComplete = 7;

    public GameObject indicatorPrefab;

    private List<Transform> indicators = new List<Transform>();
    private List<int> indicatorOrder;

    private int streak = 0;

    private int coinsLost = 0;

    protected override void Activate()
    {
        base.Activate();
        int prevCoins = GameManager.I.GetTotalCoins();
        GameManager.I.LoseCoins(numberOfIndicators * percentByIndicator, true);

        coinsLost = prevCoins - GameManager.I.GetTotalCoins();

        List<Vector2> takenPositions = new List<Vector2>();

        for(int i = 0; i < numberOfIndicators; i++)
        {
            Vector2 pos;

            do
            {
                pos = LevelGeneration.I.GetRoomAtPos(transform.position).GetRandomPos();
            } while (takenPositions.Contains(pos) || pos.Equals(transform.position));

            takenPositions.Add(pos);

            GameObject newIndicator = Instantiate(indicatorPrefab, pos, Quaternion.identity, transform);
            indicators.Add(newIndicator.transform);

            newIndicator.GetComponent<TripEventCollider>().id = i;
        }

        indicatorOrder = new List<int>();

        for(int i = 0; i < numberOfIndicators; i++)
        {
            int newOrder = Random.Range(0, numberOfIndicators);

            while (indicatorOrder.Contains(newOrder))
            {
                newOrder = Random.Range(0, numberOfIndicators);
            }

            indicatorOrder.Add(newOrder);
            //indicatorOrder[i] = newOrder;
        }

        Timer.I.StartCountdown(secondsToComplete);
        Timer.I.onFinished += EndOfEvent;

        //InfoText.I.ShowMessage("find the sequence!");
    }

    private void EndOfEvent()
    {
        foreach(Transform indicator in indicators)
        {
            Destroy(indicator.gameObject);
        }

        int coinsRegained = Mathf.RoundToInt(coinsLost / (float)numberOfIndicators * streak);

        GameManager.I.PickupCoins(coinsRegained);

        InfoText.I.ShowFailedMessage("lost " + (coinsLost - coinsRegained).ToString() + " coins");
    }

    public void TouchIndicator(int id)
    {

        streak++;

        if (streak >= numberOfIndicators)
        {
            // event complete
            Complete();
        }

        /*if(indicatorOrder[streak] == id)
        {

            streak++;

            if(streak >= numberOfIndicators)
            {
                // event complete
                Complete();
            }
        }
        else
        {
            streak = 0;
            // reset

            foreach(Transform indicator in indicators)
            {
                indicator.GetComponent<TripEventCollider>().Reset();
            }
        }*/


    }

    protected override void Complete()
    {

        GameManager.I.PickupCoins(coinsLost);

        Timer.I.Stop();

        InfoText.I.ShowMessage("coins regained");

        foreach (Transform indicator in indicators)
        {
            Destroy(indicator.gameObject);
        }

        base.Complete();
    }

}
