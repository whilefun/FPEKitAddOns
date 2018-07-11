using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whilefun.FPEKit
{

    //
    // FPETeleport
    // This class acts as a level doorway with an exit trigger volume and entrance. It is 
    // similar to the FPEDoorway script, but acts strictly in a local way. It can be used 
    // to teleport the player from one location in a scene to another location in the same 
    // scene.
    //
    // Copyright 2018 While Fun Games
    // http://whilefun.com
    //
    [RequireComponent(typeof(BoxCollider))]
    public class FPETeleport : MonoBehaviour
    {

        [SerializeField, Tooltip("The transform that will act as the 'Destination' for the player. When the player uses this teleport, they are placed at the destination transform.")]
        private Transform destinationTransform;
        public Transform DestinationTransform { get { return destinationTransform; } }

        [SerializeField, Tooltip("If true, player's view will be reset to face the same way as the destination transform Z-Forward. If false, player will keep facing the same way they were when they entered the teleport.")]
        private bool forceLookReset = false;

        private BoxCollider myCollider = null;

        void Awake()
        {

            myCollider = gameObject.GetComponent<BoxCollider>();

            if (!myCollider)
            {
                myCollider = gameObject.AddComponent<BoxCollider>();
            }

            myCollider.isTrigger = true;
            myCollider.size = Vector3.one;

        }

        void OnTriggerEnter(Collider other)
        {

            if (other.CompareTag("Player") && !FPEInteractionManagerScript.Instance.PlayerSuspendedForSaveLoad)
            {

                FPEPlayer.Instance.gameObject.transform.position = destinationTransform.position;
                FPEPlayer.Instance.gameObject.transform.rotation = destinationTransform.rotation;

                if (forceLookReset)
                {
                    FPEPlayer.Instance.gameObject.GetComponent<FPEFirstPersonController>().setPlayerLookToNeutralLevelLoadedPosition();
                }

            }

        }
        
#if UNITY_EDITOR

        void OnDrawGizmos()
        {

            Color c = Color.cyan;
            c.a = 0.5f;
            Gizmos.color = c;

            Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = oldGizmosMatrix;

            Gizmos.DrawIcon(transform.position, "Whilefun/doorwayExit.png", false);

            if(destinationTransform != null)
            {

                c = Color.magenta;
                c.a = 0.5f;
                Gizmos.color = c;

                Gizmos.DrawSphere(destinationTransform.position, 0.75f);
                Gizmos.DrawWireSphere(destinationTransform.position, 0.75f);
                Gizmos.DrawIcon(destinationTransform.position, "Whilefun/doorwayEntrance.png", false);

            }

        }

#endif

    }

}