using UnityEngine;
using System.Collections;
using UnityGameFramework.Runtime;

public class EntityView : MonoBehaviour {

    // Gives access to the application and all instances
    public Entity app {
        get {
            return GameObject.FindObjectOfType<Entity>();
        }
    }
}
