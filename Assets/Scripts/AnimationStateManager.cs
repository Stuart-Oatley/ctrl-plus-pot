using System;
using System.Collections;
using UnityEngine;

[Serializable]
public enum Position {
    menu = 0,
    pottery = 1,
    painting = 2,
    pots = 3
}

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

public class CamMoveEventArgs : EventArgs {
    private Position movingTo;
    public Position MovingTo {
        get { return movingTo; }
    }

    private float animationLength;
    public float AnimationLength {
        get { return animationLength; }
    }

    public CamMoveEventArgs(Position newPosition, float animationTime) {
        movingTo = newPosition;
        animationLength = animationTime;
    }
}

public class AnimationStateManager : MonoBehaviour {
    private static Animator camAnimator;
    private static AnimationStateManager instance;
    public delegate void MovingCamEventHandler(CamMoveEventArgs e);
    public static event MovingCamEventHandler MovingCam;

    [SerializeField]
    ClipInfo[] clips;

    private Position currentPos = Position.menu;

    private bool isMoving = false;
    private void Start() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

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

    private float GetClipLength(Position destination) {
        for(int i = 0; i < clips.Length; i++) {
            if(clips[i].StartPos == currentPos && clips[i].EndPos == destination) {
                return clips[i].ClipLength;
            }
        }
        return 0f;
    }

    public static void MoveCamera(Position destination) {
        instance.MoveTheCamera(destination);
    }

}
