using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimation : MonoBehaviour {

    
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private float angle;

    // Use this for initialization
    void Start ()
    {
        Player playerScript = GetComponent<Player>();
        navMeshAgent = playerScript.agent;
        animator = GetComponent<Animator>();
        angle = 0;

    }
	
	// Update is called once per frame
	void Update () {

        Vector3 vector3D = navMeshAgent.velocity;
        Vector2 vector2D = new Vector2(vector3D.x, vector3D.z);

        //float angle = Vector3.Angle(vectorDirection, new Vector3(0 ,1,0));

     
        if (!vector2D.Equals(new Vector2(0, 0)))
        {
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 1);

            if (vector2D.y < 0)
            {
                angle = 180 + 180 - Vector2.Angle(vector2D, new Vector2(1, 0));
            }

            else
                angle = Vector2.Angle(vector2D, new Vector2(1, 0));

        }
        else
        {
            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(1, 0);
        }
        animator.SetFloat("Angle", angle);




    }
}
