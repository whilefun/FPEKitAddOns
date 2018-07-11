using UnityEngine;

//
// ElevatorButton
// This script acts as a bridge between buttons in any scene and a given instance of 
// ElevatorController. This is required as the elevator buttons to call the elevator 
// to a given floor might be loaded after ElevatorController, so a direct Inspector 
// reference assignment won't be reliable.
//
// Copyright 2017 While Fun Games
// http://whilefun.com
//
public class ElevatorButton : MonoBehaviour {

    public void CallElevator(int floorIndex)
    {
        ElevatorController.Instance.CallToFloor(floorIndex);
    }

}
