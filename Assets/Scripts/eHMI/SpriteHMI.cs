using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple sprite swapping implementation of HMI base class
public class SpriteHMI : HMI
{
    [SerializeField]
    SpriteRenderer _renderer;
    [SerializeField]
    Sprite stop;
	[SerializeField]
	Sprite left;
    [SerializeField]
    Sprite right;
    [SerializeField]
    Sprite arrow;
    [SerializeField]
	Sprite disabled;

	public override void Display(HMIState state)
    {
        base.Display(state);
        Sprite spr = null;
        switch (state)
        {
            case HMIState.STOP:
                spr = stop;
                break;
            case HMIState.LEFT:
                spr = left;
                break;
            case HMIState.RIGHT:
                spr= right;
                break;
            case HMIState.ARROW:
                spr = arrow;
                break;
            default:
                spr = disabled;
                break;
        }
        _renderer.sprite = spr;
    }
}

