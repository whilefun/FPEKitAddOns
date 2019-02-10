using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Whilefun.FPEKit;

public class WalkableSurface : MonoBehaviour {

    [SerializeField, Tooltip("The sound bank of footsteps you want this surface to have")]
    private FPESoundBank myFootsteps;

    public FPESoundBank getFootSteps()
    {
        return myFootsteps;
    }

}
