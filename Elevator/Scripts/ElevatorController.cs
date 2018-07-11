using UnityEngine;

using Whilefun.FPEKit;

//
// ElevatorController
// This script contains a state machine and associated logic to allow for levels to be loaded 
// through elevator movement rather than hallways or doors. The ElevatorController prefab is a 
// special object that lives in all scenes, and is always loaded.
//
// Copyright 2017 While Fun Games
// http://whilefun.com
//
public class ElevatorController : MonoBehaviour {

    private static ElevatorController _instance;
    public static ElevatorController Instance {
        get { return _instance; }
    }

    public enum eElevatorState
    {
        STOPPED = 0,
        MOVING = 1,
    };
    private eElevatorState currentElevatorState = eElevatorState.STOPPED;

    [SerializeField, Tooltip("The main moving body of the elevator")]
    private GameObject elevatorPlatform;

    [SerializeField, Tooltip("The player-blocking door. Prevents player from leaving elevator when its in motion")]
    private GameObject elevatorDoor;

    [SerializeField, Tooltip("The external doors that block player from falling into elevator shaft")]
    private GameObject[] elevatorFloorDoors;

    [SerializeField, Tooltip("The FPEDoorway objects that act as level loading triggers in the shaft")]
    private GameObject[] floorDoorways;

    [SerializeField, Tooltip("the Y locations for each floor. The elevator stops at these heights for each floor.")]
    private Transform[] floorLocations;

    private int currentFloorIndex = 0;
    private int targetFloorIndex = 0;

    // Movement
    [SerializeField, Tooltip("The rate the elevator moves up and down. Default is (0,1,0)")]
    private Vector3 defaultElevatorMovement = new Vector3(0f, 1f, 0f);
    private Vector3 currentElevatorMovement = new Vector3(0f, 1f, 0f);
    private float floorSnapDistance = 0.1f;

    // Sound for elevator ding
    [SerializeField]
    private AudioSource mySpeaker;

    void Awake()
    {

        if (_instance != null)
        {
            Debug.LogWarning("ElevatorController:: Duplicate instance of ElevatorController, deleting second one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

    }

    void Start ()
    {

        if(floorLocations.Length != floorDoorways.Length || floorLocations.Length != elevatorFloorDoors.Length)
        {
            Debug.LogError("ElevatorController:: There are a different number of floor locations ("+ floorLocations.Length + "), floor doors ("+ elevatorFloorDoors.Length + "), or floor doorways (" + floorDoorways.Length + "). Some of your floors won't work as expected.");
        }

        // Always disable all doorways at start of scene
        deactivateAllDoorways();
		
	}
	
	void Update ()
    {

        if (currentElevatorState == eElevatorState.MOVING)
        {

            elevatorPlatform.transform.Translate(currentElevatorMovement * Time.deltaTime);

            // Moving Up
            if(currentElevatorMovement.y > 0)
            {

                if((Mathf.Abs(elevatorPlatform.transform.position.y - floorLocations[targetFloorIndex].transform.position.y) < floorSnapDistance) || (elevatorPlatform.transform.position.y > floorLocations[targetFloorIndex].transform.position.y))
                {
                    stopElevatorAtDestination();
                }

            }
            // Moving Down
            else if (currentElevatorMovement.y < 0)
            {

                if ((Mathf.Abs(elevatorPlatform.transform.position.y - floorLocations[targetFloorIndex].transform.position.y) < floorSnapDistance) || (elevatorPlatform.transform.position.y < floorLocations[targetFloorIndex].transform.position.y))
                {
                    stopElevatorAtDestination();
                }

            }

        }

        #region DEBUG_KEYS

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            Debug.Log("ElevatorController:: Debug Move Up 1 floor");
            MoveToFloor(Mathf.Min(floorLocations.Length,currentFloorIndex + 1));
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            Debug.Log("ElevatorController:: Debug Move Down 1 floor");
            MoveToFloor(Mathf.Max(0, currentFloorIndex - 1));
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("ElevatorController:: Debug Call Elevator to Main floor");
            CallToFloor(0);
        }
        #endregion


    }

    /// <summary>
    /// Moves the elevator to destination floor. Called by button inside the elevator.
    /// </summary>
    public void MoveToFloor(int floorIndex)
    {

        Debug.Log("ElevatorController:: Move requested to floor index " + floorIndex);

        if (currentElevatorState == eElevatorState.STOPPED && currentFloorIndex != floorIndex)
        {

            currentElevatorState = eElevatorState.MOVING;
            targetFloorIndex = floorIndex;

            if(targetFloorIndex > currentFloorIndex)
            {
                currentElevatorMovement = defaultElevatorMovement;
            }
            else
            {
                currentElevatorMovement = -defaultElevatorMovement;
            }

            closeExternalFloorDoors();
            closeDoor();
            grabPlayer();

            deactivateAllDoorways();

            // We must activate the scene changing FPEDoorway so that the scene will change as we approach our desired floor.
            activateFloorDoorway(targetFloorIndex);

        }

    }

    /// <summary>
    /// Calls elevator to floor. Used by button outside elevator. 
    /// </summary>
    public void CallToFloor(int floorIndex)
    {

        Debug.Log("ElevatorController:: Called to floor index " + floorIndex);

        elevatorPlatform.transform.position = floorLocations[floorIndex].transform.position;
        currentElevatorState = eElevatorState.STOPPED;
        currentFloorIndex = floorIndex;
        targetFloorIndex = floorIndex;

        openExternalFloorDoor(currentFloorIndex);
        openDoor();
        mySpeaker.Play();

    }

    /// <summary>
    /// This ensures that the player will move smoothly with the elevator platform as it moves to a new floor.
    /// </summary>
    private void grabPlayer()
    {
        FPEPlayer.Instance.gameObject.transform.parent = elevatorPlatform.transform;
    }

    /// <summary>
    /// Does the opposite of grabPlayer(), and allows the player and elevator to move independently again.
    /// </summary>
    private void releasePlayer()
    {
        FPEPlayer.Instance.gameObject.transform.parent = null;
    }

    /// <summary>
    /// Turns off all FPEDoorway objects so that we don't load the wrong floor. For example, if moving from floor 1 to 
    /// floor 3, we must move past floor 2. But if floor 2's doorway was still active we'd load in floor 2's scene as we
    /// moved past it, which is inefficient.
    /// </summary>
    private void deactivateAllDoorways()
    {

        for (int f = 0; f < floorDoorways.Length; f++)
        {
            floorDoorways[f].SetActive(false);
        }

    }

    /// <summary>
    /// Activates desitination floor's level loading FPEDoorway game object so the 
    /// destination level is loaded when we approach it.
    /// </summary>
    private void activateFloorDoorway(int floorIndex)
    {

        floorDoorways[floorIndex].SetActive(true);

        // Extra step: By default, FPEDoorways live in each loaded scene. But in this case, the doorways live 
        // in the Elevator shaft. So when a level is loaded, we must manually re-enable the doorway's BoxCollider 
        // since it gets disabled internally to FPEDoorway after the player touches it.
        floorDoorways[floorIndex].gameObject.GetComponent<BoxCollider>().enabled = true;

    }

    //
    // Currently the openDoor(), closeDoor(), openExternalFloorDoor(), and closeExternalFloorDoors() functions just 
    // toggle the cube on and off. You can make the cube mesh invisible and have a fancy door animation in addition to 
    // this cube, but the cube will keep the player from walking out of the elevator while its in motion or falling
    // into the elevator shaft if the elevator is not at their current floor.
    //
    private void openDoor()
    {
        elevatorDoor.SetActive(false);
    }

    private void closeDoor()
    {
        elevatorDoor.SetActive(true);
    }

    private void openExternalFloorDoor(int floorIndex)
    {
        elevatorFloorDoors[floorIndex].SetActive(false);
    }

    private void closeExternalFloorDoors()
    {
        for(int f = 0; f < elevatorFloorDoors.Length; f++)
        {
            elevatorFloorDoors[f].SetActive(true);
        }
    }

    private void stopElevatorAtDestination()
    {

        elevatorPlatform.transform.position = floorLocations[targetFloorIndex].transform.position;
        currentElevatorState = eElevatorState.STOPPED;
        currentFloorIndex = targetFloorIndex;

        deactivateAllDoorways();
        releasePlayer();
        openExternalFloorDoor(currentFloorIndex);
        openDoor();

        mySpeaker.Play();

    }


    /// <summary>
    /// Strictly for visual reference and debug purposes
    /// </summary>
    private void OnDrawGizmos()
    {

        Color c = Color.red;

        if (floorLocations != null)
        {

            c.a = 0.5f;
            Gizmos.color = c;

            for(int i = 0; i < floorLocations.Length; i++)
            {
                //Gizmos.DrawWireCube(floorLocations[i].position, Vector3.one * 0.6f);
                Gizmos.DrawCube(floorLocations[i].position, Vector3.one * 0.6f);
            }

            c = Color.green;
            Gizmos.color = c;

            //Gizmos.DrawWireCube(floorLocations[targetFloorIndex].position, Vector3.one * 0.6f);
            Gizmos.DrawWireCube(floorLocations[targetFloorIndex].position, Vector3.one * 0.6f);

        }

    }

}
