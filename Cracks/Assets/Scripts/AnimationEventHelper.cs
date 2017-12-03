using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
