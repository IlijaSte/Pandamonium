using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerAnimation : MonoBehaviour
{

    
    private Animator animator;
    private AIPath path;
    private float angle;

    // Use this for initialization
    void Start ()
    {
        path = transform.parent.GetComponent<AIPath>();
        animator = GetComponent<Animator>();
   
    }

    // Update is called once per frame
    public void OnEnable()
    {
        
    }
    void Update ()
    {

        Vector3 vector3D = path.velocity;
     
        Vector2 vector2D = new Vector2(vector3D.x, vector3D.y);

        //float angle = Vector3.Angle(vectorDirection, new Vector3(0 ,1,0));
      
        if (Mathf.Approximately(vector2D.x,Vector2.zero.x) && Mathf.Approximately(vector2D.y, Vector2.zero.y))
        {
            //animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 0);
        }
        else 
        {
          // animator.SetLayerWeight(0, 1);
           
            // animator.SetFloat("Angle", angle);
            animator.SetLayerWeight(1, 1);

            if (vector2D.y < 0)
            {
                angle = 180 + 180 - Vector2.Angle(vector2D, new Vector2(1, 0));
            }

            else
                angle = Vector2.Angle(vector2D, new Vector2(1, 0));

            animator.SetFloat("Angle", angle);

        }

        // print(angle);

        // print(angle);



    }
}
