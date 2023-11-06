using UnityEngine;
using UnityEngine.InputSystem;

public class XRDeviceData : MonoBehaviour
{
    [SerializeField] InputActionProperty velocityProperty;
    [SerializeField] InputActionProperty angularVelocityProperty;
    public Vector3 Velocity { get; private set; } = Vector3.zero;
    public Vector3 AngularVelocity { get; private set; } = Vector3.zero;


    void Update()
    {
        Velocity = velocityProperty.action.ReadValue<Vector3>();
        AngularVelocity = angularVelocityProperty.action.ReadValue<Vector3>();
    }   

}
