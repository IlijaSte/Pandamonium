using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoom : MonoBehaviour {

    public List<PressurePlate> pressurePlates;

    private List<int> indicatorOrder;

    private int streak = 0;

    private void Start()
    {
        indicatorOrder = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            int newOrder = Random.Range(0, 4);

            while (indicatorOrder.Contains(newOrder))
            {
                newOrder = Random.Range(0, 4);
            }

            indicatorOrder.Add(newOrder);
            //indicatorOrder[i] = newOrder;
            pressurePlates[i].id = i;
        }
    }

    void Complete()
    {
        InfoText.I.ShowMessage("puzzle complete");
        Instantiate(GameManager.I.prefabHolder.blueprint, transform.position, Quaternion.identity);
    }

    public void TouchIndicator(int id)
    {

        if(indicatorOrder[streak] == id)
        {

            streak++;

            pressurePlates[id].Activate();

            if(streak >= 4)
            {
                // event complete
                Complete();
            }
        }
        else
        {
            streak = 0;
            // reset

            foreach(PressurePlate plate in pressurePlates)
            {
                plate.Deactivate();
            }
        }


    }
}
