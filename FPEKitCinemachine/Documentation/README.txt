Steps:
-----
1) Open FPEPlayerController prefab prefab, and add a Cinemachine External Camera component to the MainCamera child object of the prefab
2) Apply changes to the prefab
3) Create an empty GameObject and name it CM_Brain
4) Add a Cinemachine Brain component to CM_Brain. Change Default blend to Cut.
5) Create an empty GameObject and name it CM_CutsceneCamera_01
6) Add a Camera to CM_CutsceneCamera_01
7) Add a Cinemachine Virtual Camera component to CM_CutsceneCamera_01. Change the Aim to Hard Look At.
8) Create a Cube, name it CM_CutsceneTrigger_01. Set the Is Trigger value of the Box Collider is true (check the "Is Trigger" box).
9) Add the CutsceneTrigger script to CM_CutsceneTrigger_01.
10) Drag a reference to CM_CutsceneCamera_01 into the Camera To Trigger field of the CM_CutsceneTrigger_01's CutsceneTrigger script.

Run the scene. Walk into the cube and the camera view will cut to the cutscene camera for the time specified, then cut back to first person view.

From here you can get as fancy as you want with the full power of Cinemachine :)