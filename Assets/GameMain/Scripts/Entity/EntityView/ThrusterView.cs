using UnityEngine;
using System.Collections;

public class ThrusterView : EntityView {

   

    // Tank Turn Method
    public void Turn(Rigidbody rigidbody, float turnInputValue, float speed) {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = turnInputValue * speed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        rigidbody.MoveRotation(rigidbody.rotation * turnRotation);
    }

    // Tank Move Method
    public void Move(Rigidbody rigidbody, Transform transform, float movementInputValue, float speed) {

        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * movementInputValue * speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        rigidbody.MovePosition(rigidbody.position + movement);

    }
}
