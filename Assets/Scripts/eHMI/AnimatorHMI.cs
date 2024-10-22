using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Animator based HMI implementation
public class AnimatorHMI : HMI
{
    [SerializeField]
    MeshRenderer meshRenderer;
    Material material;
    [SerializeField]
    Animator animator;

    //Animator triggers
    [SerializeField]
    string stopTrigger = "stop";
    [SerializeField]
    string leftTrigger = "left";
    [SerializeField]
    string rightTrigger = "right";
    [SerializeField]
    string arrowTrigger = "arrow";
    [SerializeField]
    string disabledTrigger = "disabled";

    //texture to be set on certain state changes
    [SerializeField]
    Texture2D stop;
    [SerializeField]
    Texture2D left;
    [SerializeField]
    Texture2D right;
    [SerializeField]
    Texture2D arrow;
    [SerializeField]
    Texture2D disabled;

    private void Awake()
    {
        material = meshRenderer.material;
    }

    public override void Display(HMIState state)
    {
        base.Display(state);
        switch (state)
        {
            case HMIState.STOP:
                material.mainTexture = stop;
                animator.SetTrigger(stopTrigger);
                break;
            case HMIState.LEFT:
                material.mainTexture = left;
                animator.SetTrigger(leftTrigger);
                break;
            case HMIState.RIGHT:
                material.mainTexture = right;
                animator.SetTrigger(rightTrigger);
                break;
            case HMIState.ARROW:
                material.mainTexture = arrow;
                animator.SetTrigger(arrowTrigger);
                break;
            default:
                material.mainTexture = disabled;
                animator.SetTrigger(disabledTrigger);
                break;
        }
    }
}