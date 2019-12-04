Steps:
-----
1) Import First Person Exploration Kit, then import Fungus
2) Place the FungusIntegration folder alongside the others in your Assets folder
3) Edit the FPEPlayerController prefab, and attach the included FPEFungusHelper script component
4) Open the included FungusTest scene, and run
5) Walk into the circle on the floor (this triggers Fungus dialog sequence)
6) Resume normal gameplay

Note: You can also pickup the blue "MysteryObject" which will also trigger some dialog.


Creating Additional Fungus Dialog:
---------------------------------

1) Create a regular Fungus Flowchart-based dialog object, and name it "MyCustomFungusDialog".
2) Ensure that the first and last things in the flow chart are Priority Up and Priority Down, per the example scene's "MyBasicFungusDialog" object
3) Disable the game object "MyCustomFungusDialog" created in step 1 above
4) Add an FPEEventTrigger or other event-driven means to enable a game object (e.g. Pickup or Put Back event, Activation, etc.)
5) Assign your chosen even in the inspector or your script so that it enables your custom "MyCustomFungusDialog" object from step 1

When you run your scene, the assigned event will trigger the player to stop moving and the dialog will start.

Note: If you wish to have dialog play while the player is still moving, simply exclude the Priority Up and Priority Down items from your custom dialog.

