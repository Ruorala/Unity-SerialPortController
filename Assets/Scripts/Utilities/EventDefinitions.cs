using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

namespace Events
{
	[Serializable] public class CameraMoveEvent : UnityEvent<Vector3> {}
	[Serializable] public class NoiseEvent : UnityEvent<float> {}
	[Serializable] public class TargetDetectedEvent : UnityEvent<GameObject> {}
    [Serializable] public class TargetDetectedSightEvent : UnityEvent<GameObject,int> { }
	[Serializable] public class ThrowCompleteEvent : UnityEvent<Collider> {}
    [Serializable] public class CollisionEvent : UnityEvent<Collision> { }
	[Serializable] public class TriggerEvent : UnityEvent<Collider> {}
    [Serializable] public class ChildCollisionEvent : UnityEvent<GameObject, Collision> { }
    [Serializable] public class ChildTriggerEvent : UnityEvent<GameObject, Collider> { }
	[Serializable] public class EventWithBool : UnityEvent<bool> {}
    [Serializable] public class EventWithInt : UnityEvent<int> { }
    [Serializable] public class EventWithFloat : UnityEvent<float> { }
    [Serializable] public class EventWithVector2 : UnityEvent<Vector2> { }
    [Serializable] public class EventWithVector3 : UnityEvent<Vector3> { }
    [Serializable] public class EventWithIntArray : UnityEvent<int[]> { }

    [Serializable] public class EventWithChangeInventory : UnityEvent<int, bool> { }
    [Serializable] public class EventWithInventoryItem : UnityEvent<int, int> { }
    [Serializable] public class EventWithInventoryItemProgress : UnityEvent<int, float> { }

    [Serializable] public class EventWithString : UnityEvent<string> { }

    //[Serializable] public class EventWithInputCode : UnityEvent<ControlSchemes.ControlScheme.InputCodeEvent> { }
}
