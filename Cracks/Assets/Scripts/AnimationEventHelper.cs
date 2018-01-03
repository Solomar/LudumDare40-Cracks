using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script let's you call functions from any script as an animation event as
/// long as this script is on the same object as the animator and that the script
/// with the wanted function has a reference to it to be able to add the function
/// in AnimationEventHelper's delegate.
/// </summary>
public class AnimationEventHelper : MonoBehaviour {

    // Keeping these not static in case I want to use them for other
    // call to parent animation in other objects
    public delegate void AnimatioEventCall();
    public AnimatioEventCall animationEventCalled;
    public void AddOnAnimationEvent(AnimatioEventCall function) { animationEventCalled += function; }
    public void RemoveOnAnimationEvent(AnimatioEventCall function) { animationEventCalled -= function; }

    public void CallAnimationEvent()
    {
        animationEventCalled();
    }
}
