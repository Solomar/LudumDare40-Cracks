# LudumDare40 - Cracks

## Cracks can be played here: https://youbestrong.itch.io/cracks

### Plugins and Unity tools used
* DOTween
* Post Processing Stack
* TextMesh Pro

### Scripts of interest
_These are the scripts that I would re-use for later jams or were cool to make even if I never have a chance to re-use them._

__AnimationEventHelper.cs__
AnimationEventHelper allowed me to use any function for an animation event no matter where the script with that function was as long as I added the function in the AnimationEventHelper's delegate. It does have some pretty flagrant limitation such as the event that is called will have a delegate that you have to add and remove functions if you want it to do more than one thing, but it was overall very useful. I mainly used it to time the UseTool() fro mthe FixingTool script to time it properly with the animation.

__CompositeColliderSpawner.cs__
The most interesting script I had to write because it used the Unity compound colliders that I hadn't used yet. I don't know where I used it again but it work very nicely! There was a quick (and not perfect implementation) of this compound collider covering the cracks in __CoveredCrack.cs__ . It isn't perfect since it really takes only the bounds in consideration as an approximation for the coverage.

__DialogueWrite.cs__
Second time I re-used this script but first time it's in a public git. It's quite straigth forward and worked perfectly by switching a normal UI text to a TextMesh Pro object to do the writing.



* Post Processing Stack
* TextMesh Pro
