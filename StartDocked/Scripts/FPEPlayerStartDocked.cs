using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whilefun.FPEKit
{

    //
    // FPEPlayerStartDocked
    // This script simply acts as a findable component for starting a 
    // level where the player should start in a docked position.
    //
    // Copyright 2017 While Fun Games
    // http://whilefun.com
    //
    public class FPEPlayerStartDocked : MonoBehaviour
    {

        [SerializeField, Tooltip("If assigned to a dock in the scene, the player will start the game docked on this Dock object. When the player undocks from the starting dock, they will 'return' to the FPEPlayerStartDocked transform.")]
        private FPEInteractableDockScript startingDock = null;
        public FPEInteractableDockScript StartingDock {
            get { return startingDock; }
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {

            Color c = Color.yellow;
            c.a = 0.5f;
            Gizmos.color = c;

            Gizmos.DrawSphere(transform.position, 0.75f);
            Gizmos.DrawWireSphere(transform.position, 0.75f);
            Gizmos.DrawIcon(transform.position, "Whilefun/playerStart.png", false);

            if(startingDock != null)
            {
                c = Color.blue;
                Gizmos.color = c;
                Gizmos.DrawLine(transform.position, startingDock.DockTransform.position);
            }

        }

#endif

    }

}