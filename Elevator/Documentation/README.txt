Elevator Level Loading
======================

===============
1) Script Edits
===============

Some light script editing is required to make the vanilla First Person Exploration Kit code (as of v2.0.2) work with the Elevator.


----------------------
Edits to FPEDoorway.cs:
----------------------

Add this new Inspector field

[SerializeField, Tooltip("If true, player will be moved to doorway Entrance location when level load is complete. Set to false for things like automated 'elevator' level changes where player is being moved automatically.")]
private bool movePlayerToEntranceOnSceneChange = true;
public bool MovePlayerToEntranceOnSceneChange {
	get { return movePlayerToEntranceOnSceneChange; }
}


Also edit the contents of the OnTriggerEnter() function such that the calls to ChangeSceneToAndAutoSave() and ChangeSceneToNoSave() are given the new bool as a second parameter. For example:

FPESaveLoadManager.Instance.ChangeSceneToAndAutoSave(connectedSceneIndex, movePlayerToEntranceOnSceneChange);


------------------------------
Edits to FPESaveLoadManager.cs:
------------------------------

Around line 60, add a new variable:

private bool movePlayerOnSceneChange = false;

In the function movePlayerToSuitableEntranceOrStartPosition(), there is an else block with comment "// Otherwise, yield to the appropriate doorway.". Wrap the entire body of that else block inside a new nested if statement:

// Otherwise, yield to the appropriate doorway.
else
{

	if (movePlayerOnSceneChange)
	{
		<existing else block content>
	}

}

This will ensure that the player is only moved for default doorway settings. You must uncheck the FPEDoorWay objects in the ElevatorController prefab so the player isn't moved when the elevator reaches the loading trigger. This will allow the elevator platform to move the player for you. If you leave the elevator FPEDoorways with the default, the player will be placed at the scene origin and fall out of the level.


In the functions ChangeSceneToAndAutoSave() and public void ChangeSceneToNoSave(), change the functions to take a second argument: bool movePlayer

Inside the body of both functions, add the line:

movePlayerOnSceneChange = movePlayer;

This will allow the FPEDoorway objects configuration to be passed to the FPESaveLoadManager object.


Around lines 440 and 449, there are case statements for CHANGING_SCENE and CHANGING_SCENE_NOSAVE which set the variable resetPlayerLook to true. Instead, set resetPlayerLook = movePlayerOnSceneChange. This will allow non-moving doorways like the elevator shaft doorways to leave the player's look direction alone when loading the destination floor.


-----------------------
Edits to FPEMainMenu.cs:
-----------------------

Since we changed the FPESaveLoadManager ChangeSceneToNoSave() function to require a new parameter, we must also change a call to this inside FPEMainMenu. Around line 115, add "true" as the second parameter:

FPESaveLoadManager.Instance.ChangeSceneToNoSave(FPESaveLoadManager.Instance.FirstLevelSceneBuildIndex, true);

This will ensure we always move the player from main menu to the player start location when starting a new game.


======================
2) Edit Build Settings:
======================

The included scenes (MainFloor, SecondFloor, and ThirdFloor) must be added as scenes index 1, 2, and 3. The default demoMainMenu scene must remain at index 0. Change this through File > Build Settings. See BuildSettings.jpg for reference.

=====================
3) Adding More Floors:
=====================

To add or change floors, you must have:

-A scene to load that contains an ElevatorController at the same location as other scenes ((0,0,0) recommended!)
-A floor location that aligns vertically to the new scene's floor
-A doorway object with an FPEDoorway component that has the new floor scene's build index set as the Connected Scene Index
-An external doorway and set of elevator buttons for the new floor (prevents player from falling into elevator shaft, and allows player to call elevator to that floor)
-You must assign the scene floor location, external door object, and doorway inside the child objects of ElevatorController (FloorLocations, ExternalFloorDoors, and FloorDoorways respectively)

Don't forget to apply these changes to the prefab so that all scenes will have the new floor information!




