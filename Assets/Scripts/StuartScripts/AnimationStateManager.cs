using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Position of the camera
/// </summary>
[Serializable]
public enum Position {
    menu = 0,
    pottery = 1,
    painting = 2,
    pots = 3
}

/// <summary>
/// Holds the Animation clip, it's length and the camera start and finish positions
/// </summary>
[Serializable]
public class ClipInfo {
    [SerializeField]
    private AnimationClip clip;
    public float ClipLength {
        get { return clip.length; }
    }
    [SerializeField]
    private Position startPos;
    public Position StartPos {
        get { return startPos; }
    }
    [SerializeField]
    private Position endPos;
    public Position EndPos {
        get { return endPos; }
    }
}

/// <summary>
/// Event args passed when the camera is animated
/// </summary>
public class CamMoveEventArgs : EventArgs {
    private Position movingTo;
    public Position MovingTo {
        get { return movingTo; }
    }

    private float animationLength;
    public float AnimationLength {
        get { return animationLength; }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="newPosition"></param>
    /// <param name="animationTime"></param>
    public CamMoveEventArgs(Position newPosition, float animationTime) {
        movingTo = newPosition;
        animationLength = animationTime;
    }
}

/// <summary>
/// Handles moving the camera
/// </summary>
public class AnimationStateManager : MonoBehaviour {
    private static Animator camAnimator;
    private static AnimationStateManager instance;
    public delegate void MovingCamEventHandler(CamMoveEventArgs e);
    public static event MovingCamEventHandler MovingCam;

    [SerializeField]
    ClipInfo[] clips;

    private Position currentPos = Position.menu;

    private bool isMoving = false;
    /// <summary>
    /// ensures the manager is a singleton
    /// </summary>
    private void Start() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    /// <summary>
    /// animates the camera and triggers the MovingCam event
    /// </summary>
    /// <param name="destination"></param>
    private void MoveTheCamera(Position destination) {
        if (!camAnimator) {
            camAnimator = FindObjectOfType<Animator>();
        }
        if(isMoving) {
            return;
        }
        camAnimator.SetInteger("TargetPosition", (int)destination);
        MovingCam?.Invoke(new CamMoveEventArgs(destination, GetClipLength(destination)));
        currentPos = destination;
    }

    /// <summary>
    /// Gets the length of the clip
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    private float GetClipLength(Position destination) {
        for(int i = 0; i < clips.Length; i++) {
            if(clips[i].StartPos == currentPos && clips[i].EndPos == destination) {
                return clips[i].ClipLength;
            }
        }
        return 0f;
    }

    /// <summary>
    /// Public method to request the camera is moved to a position
    /// </summary>
    /// <param name="destination"></param>
    public static void MoveCamera(Position destination) {
        instance.MoveTheCamera(destination);
    }

}
