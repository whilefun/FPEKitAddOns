using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whilefun.FPEKit;

public class FPEFungusHelper : MonoBehaviour
{

    void OnEnable()
    {
        Fungus.FungusPrioritySignals.OnFungusPriorityStart += FungusPrioritySignals_OnFungusPriorityStart;
        Fungus.FungusPrioritySignals.OnFungusPriorityEnd += FungusPrioritySignals_OnFungusPriorityEnd;
    }

    void OnDisable()
    {
        Fungus.FungusPrioritySignals.OnFungusPriorityStart -= FungusPrioritySignals_OnFungusPriorityStart;
        Fungus.FungusPrioritySignals.OnFungusPriorityEnd -= FungusPrioritySignals_OnFungusPriorityEnd;
    }

    private void FungusPrioritySignals_OnFungusPriorityEnd()
    {
        FPEInteractionManagerScript.Instance.EndCutscene(true);
    }

    private void FungusPrioritySignals_OnFungusPriorityStart()
    {
        FPEInteractionManagerScript.Instance.BeginCutscene(true);
    }

}
