
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SailController sailController;
    [SerializeField] private Transform sailForcePoint;

    [SerializeField] private Transform sail;

    [Header("Wind")]
    [SerializeField] private Vector3 windDirection = Vector3.right;
    [SerializeField, Min(0f)] private float windSpeed = 5f;

    [Header("Sail Aerodynamics")]
    [SerializeField, Min(0f)] private float sailForceCoefficient = 0.3f;
    [SerializeField, Min(0f)] private float maxSailForce = 20f;

    [Header("Linear Movement")]
    [SerializeField, Min(0f)] private float waterDrag = 0.5f;
    [SerializeField, Min(0f)] private float airDrag = 0.1f;
    [SerializeField, Min(0f)] private float lateralDrag = 3f;
    [SerializeField, Min(0f)] private float maxSpeed = 10f;

    [Header("Turning")]
    [SerializeField, Min(0.01f)] private float yawInertia = 5f;
    [SerializeField, Min(0f)] private float yawTorqueMultiplier = 20f;
    [SerializeField, Min(0f)] private float angularDrag = 2f;
    [SerializeField, Min(0f)] private float maxTurnSpeed = 60f;

    private Vector3 velocity;
    private float yawVelocity;

    public Vector3 Velocity => velocity;
    public float Speed => velocity.magnitude;
    public float TurnSpeed => yawVelocity;

    private void Update()
    {
        if (sailController == null)
            return;

        float deltaTime = Time.deltaTime;

        if (deltaTime <= 0f)
            return;

        UpdateMovement(deltaTime);
    }

    private void UpdateMovement(float deltaTime)
    {
        Vector3 normalizedWindDirection = windDirection.normalized;
        Vector3 windVelocity = normalizedWindDirection * windSpeed;

        // Calculate wind relative to the moving ship.
        Vector3 apparentWind = windVelocity - velocity;

        float opening = sailController.Opening / 100f;

        Vector3 sailNormal = sail != null
            ? sail.right
            : transform.right;

        float normalWindSpeed = Vector3.Dot(apparentWind, sailNormal);

        // Approximate aerodynamic pressure on the sail.
        Vector3 sailForce =
            sailNormal *
            normalWindSpeed *
            Mathf.Abs(normalWindSpeed) *
            sailForceCoefficient *
            opening;

        sailForce = Vector3.ClampMagnitude(sailForce, maxSailForce);

        // Separate the force into forward and sideways components.
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float forwardForce = Vector3.Dot(sailForce, forward);
        float lateralForce = Vector3.Dot(sailForce, right);

        // Water resistance limits sideways drift.
        float lateralVelocity = Vector3.Dot(velocity, right);
        Vector3 lateralResistance = -right * lateralVelocity * lateralDrag;

        // Air and water drag oppose the current movement.
        Vector3 dragForce = -velocity * (waterDrag + airDrag);

        Vector3 totalForce =
            forward * forwardForce +
            right * lateralForce +
            lateralResistance +
            dragForce;

        // Integrate linear motion.
        velocity += totalForce * deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Calculate yaw torque from the force offset.

        Vector3 forcePoint = sail != null
            ? sail.position
            : transform.position;

        Vector3 leverArm = forcePoint - transform.position;

        float yawTorque = Vector3.Cross(
            leverArm,
            sailForce
        ).y;

        float yawAcceleration = yawTorque * yawTorqueMultiplier / yawInertia;

        Debug.DrawRay(forcePoint, sailForce, Color.red);
        Debug.DrawRay(transform.position, leverArm, Color.blue);

        Debug.Log($"Sail force: {sailForce.magnitude:F2}, " +
                $"Yaw torque: {yawTorque:F2}, " +
                $"Turn speed: {yawVelocity:F2}");

        // Integrate angular motion and apply angular resistance.
        yawVelocity += yawAcceleration * deltaTime;
        yawVelocity *= Mathf.Exp(-angularDrag * deltaTime);
        yawVelocity = Mathf.Clamp(
            yawVelocity,
            -maxTurnSpeed,
            maxTurnSpeed
        );

        // Move and rotate without Rigidbody.
        transform.position += velocity * deltaTime;
        transform.Rotate(
            Vector3.up,
            yawVelocity * deltaTime,
            Space.World
        );
    }
}
