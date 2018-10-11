using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RestoreAnimationParam : MonoBehaviour
{

    private class AnimParam
    {
        public AnimatorControllerParameterType type;
        public string paramName;
        object data;

        public AnimParam(Animator anim, string paramName, AnimatorControllerParameterType type)
        {
            this.type = type;
            this.paramName = paramName;
            switch (type)
            {
                case AnimatorControllerParameterType.Int:
                    this.data = (int)anim.GetInteger(paramName);
                    break;
                case AnimatorControllerParameterType.Float:
                    this.data = (float)anim.GetFloat(paramName);
                    break;
                case AnimatorControllerParameterType.Bool:
                    this.data = (bool)anim.GetBool(paramName);
                    break;
            }
        }
        public object getData()
        {
            return data;
        }
    }

    Animator anim;
    List<AnimParam> parms = new List<AnimParam>();

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnAnimDisable()
    {
        Debug.Log("Saving Animator state: " + anim.parameters.Length);
        for (int i = 0; i < anim.parameters.Length; i++)
        {
            AnimatorControllerParameter p = anim.parameters[i];
            AnimParam ap = new AnimParam(anim, p.name, p.type);
            parms.Add(ap);
        }
    }

    void OnEnable()
    {
        Debug.Log("Restoring Animator state.");
        foreach (AnimParam p in parms)
        {
            switch (p.type)
            {
                case AnimatorControllerParameterType.Int:
                    anim.SetInteger(p.paramName, (int)p.getData());
                    break;
                case AnimatorControllerParameterType.Float:
                    anim.SetFloat(p.paramName, (float)p.getData());
                    break;
                case AnimatorControllerParameterType.Bool:
                    anim.SetBool(p.paramName, (bool)p.getData());
                    break;
            }
        }
        parms.Clear();
    }
}