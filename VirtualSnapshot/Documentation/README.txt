

1) Import First Person Explrotion Kit into new Project A

2) Import Virtual Snapshot into new Project B

3) Add "VirtualSnapshot" tag to Project A

4) Copy Virtual Snapshot folder into Assets folder of Project A

5) Update VirtualSnapshotDisposable, VirtualSnapshotDSLR, and VirtualSnapshotFilmSLR prefabs to ensure they are still tagged with VirtualSnapshot tag

6) Make the following updates to FPEInteractionManagerScipt.cs:




A) Make setMouseSensitivity() public (~line 2216)

B) Make restorePreviousMouseSensitivity() public

C) Add new cameraActive bool and ToggleCamera() function as follows:

private bool cameraActive = false;
public void ToggleCamera () {
	cameraActive = !cameraActive; 
}

D) For each if statement inside "#region CORE_INTERACTION_LOGIC" where "!dockingInProgress" is a condition, also add "&& !cameraActive" as another condition (4 places total, around lines 381, 659, 800, and 901)



7) Make the following updates to VirtualSnapshotScript.cs:

A) Add using Whilefun.FPEKit;

B) Change VirtualSnapshot camera key to Tab instead of C (around line 272), as it conflicts with Crouch key. Can also re-assign to an input from FPEInputManager and define a new dedicated Camera key defintion later if desired.

C) Change camera key toggle to be GetKeyDown instead of GetKey (around line 272)

D) Add the following to the bottom of the Start() function:

snapshotCamera = Camera.main;

E) Move the following from Awake() to Start():

Move this from around line 118:

startZoom = snapshotCamera.fieldOfView;
currentZoom = startZoom;
targetZoom = startZoom;
previousZoom = startZoom;

Move this from around line 198:

thePlayer = GameObject.FindGameObjectWithTag("Player");
if (!thePlayer){
	Debug.LogError("VirtualSnapshotScript:: No object in scene tagged as 'Player'");
}

getStartSensitivity();


F) Inside the beginning of the "Input.GetKeyDown(KeyCode.Tab)" statement and before if(cameraUp) statement, add the following:

if (FPEInteractionManagerScript.Instance != null)
{
	FPEInteractionManagerScript.Instance.ToggleCamera();
}

G) Edit adjustMouseLookSensitivity() to replace

thePlayer.GetComponent<MouseLookScript>().setMouseSensitivity(adjustedSensitivity.x, adjustedSensitivity.y);

with

FPEInteractionManagerScript.Instance.setMouseSensitivity(adjustedSensitivity);

H) Edit resetMouseLookSensitivity() to replace

//thePlayer.GetComponent<MouseLookScript>().setMouseSensitivity(startSensitivity.x, startSensitivity.y);

with

FPEInteractionManagerScript.Instance.restorePreviousMouseSensitivity(false);

I) Edit getStartSensitivity() so that it only contains:

startSensitivity = FPEInputManager.Instance.LookSensitivity;

J) Inside Update(), the last else block can be removed entirely.

K) In each of the VirtualSnapshotDisposable, VirtualSnapshotDSLR, and VirtualSnapshotFilmSLR prefaba, change "SnapshotUICanvas" and "CameraTransitionCanvas" from "Screen Space - Camera" to "Screen Space - Overlay"


You can add the included IntegrationTest.unity scene to your project and run it. Press tab to open the camera, take a picture of the soup can, then go pickup and examine the soup can.


IMPORTANT NOTE:

There are probably going to be some edge cases with interactions and UI priorities that are not addressed here. For example, in some cases you can take a photo while holding an object. That might not be something you want to be allowed. Same goes with lots of other things like docks, journals, etc. These should all be addressable with relative ease by just setting up rules for the camera. For example, you might want to change the new ToggleCamera() function to be a request rather than a passive toggle. For example, maybe it rejects the request to take out the camera if you're sitting down, holding something, looking at a button, etc., etc. and prompts the player with a "You cant do that right now" or similar.




