using UnityEngine;

/*
    Script to apply weapon movement effects according to player movement.

    Please read How_To_Use.pdf in the asset folder.

    Credit to snon200,
    For any questions: snon200@gmail.com
*/

namespace UltimateParkourFPS
{
    public class WeaponMovementEffects : MonoBehaviour
    {
        #region components
        private PlayerController playerController;
        private Rigidbody playerRigidBody;
        #endregion

        private Vector2 movementAmount; // player movement amount
        private Vector2 rotationAmount; // player rotation amount

        [Header("Sway")]
        [Tooltip("If the sway effect is enabled")]
        [SerializeField] private bool swayEnabled = true;
        [SerializeField] private float step = 0.01f;
        [SerializeField] private float maxStep = 0.06f;

        private static float smooth = 10f;

        [Header("Sway Tilt")]
        [Tooltip("If the sway rotation is enabled")]
        [SerializeField] private bool swayTiltEnabled = true;
        [SerializeField] private float rotationStep = 4f;
        [SerializeField] private float maxRotationStep = 5f;

        private static float smoothRot = 12f;

        [Header("Head Bob")]
        [Tooltip("If the headbob effect is enabled")]
        [SerializeField] private bool headBobEnabled = true;
        [SerializeField] private Vector3 directBobAmount = Vector3.one * 0.1f;
        [SerializeField] private Vector3 circularBobAmount = Vector3.one * 0.01f;
        
        private float curveSin { get => Mathf.Sin(speedCurve); }
        private float curveCos { get => Mathf.Cos(speedCurve); }

        [Header("Head Bob Tilt")]
        [Tooltip("If the headbob rotation is enabled")]
        [SerializeField] private bool headBobTiltEnabled = true;
        [SerializeField] private Vector3 multiplier = Vector3.one;
        [SerializeField] private float maxHeadBob = 20f;

        private float speedCurve;

        // Start is called before the first frame update
        private void Start()
        {
            // set player components
            Transform player = transform.root;
            playerController = player.GetComponent<PlayerController>();
            playerRigidBody = player.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        private void Update()
        {
            // get player input
            GetInput();

            // apply new position and rotation
            UpdateTransfrom();
        }
        
        // Set movement and rotation amount according to player input
        private void GetInput()
        {
            movementAmount.x = Input.GetAxis("Horizontal");
            movementAmount.y = Input.GetAxis("Vertical");
            movementAmount = movementAmount.normalized;

            rotationAmount.x = Input.GetAxis("Mouse X");
            rotationAmount.y = Input.GetAxis("Mouse Y");
        }

        // Update weapon position and rotation according to movement effects
        private void UpdateTransfrom()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, SwayAmount() + BobAmount(), Time.deltaTime * smooth);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(SwayTilt()) * Quaternion.Euler(BobTilt()), Time.deltaTime * smoothRot);
        }

        #region Sway
        // Apply weapon sway effect
        private Vector3 SwayAmount()
        {
            // check if sway is enabled
            if (!swayEnabled) { return Vector3.zero; }

            // invert sway direction from camera rotation
            Vector3 invertLook = rotationAmount * -step;
            
            // clamp sway to max step value
            invertLook.x = Mathf.Clamp(invertLook.x, -maxStep, maxStep);
            invertLook.y = Mathf.Clamp(invertLook.y, -maxStep, maxStep);

            return invertLook;
        }

        // Apply weapon sway tilt effect
        private Vector3 SwayTilt()
        {
            // check if sway tilt is enabled
            if (!swayTiltEnabled) { return Vector3.zero; }

            // invert sway direction from camera rotation
            Vector2 invertLook = rotationAmount * -rotationStep;

            // clamp sway to max step value
            invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
            invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

            return new Vector3(invertLook.y, invertLook.x, invertLook.x);
        }
        #endregion

        #region Head Bob
        // Apply head bob effect
        private Vector3 BobAmount()
        {
            // check if headbob is enabled
            if (!headBobEnabled) { return Vector3.zero; }

            Vector3 bobPosition;
            bobPosition.x = (curveCos * circularBobAmount.x * (playerController.isGrounded ? 1 : 0)) - (movementAmount.x * directBobAmount.x);
            bobPosition.y = (curveSin * circularBobAmount.y) - (playerRigidBody.linearVelocity.y / 10f * directBobAmount.y);
            bobPosition.z = -(movementAmount.y * directBobAmount.z);

            return bobPosition;
        }

        // Apply head bob tilt effect
        private Vector3 BobTilt()
        {
            // check if headbob tilt is enabled
            if (!headBobTiltEnabled) { return Vector3.zero; }

            // generate sin and cos waves
            speedCurve += Time.deltaTime * (playerController.isGrounded ? Mathf.Clamp(playerRigidBody.linearVelocity.magnitude, 0f, maxHeadBob) : 1f) + 0.01f;

            Vector3 bobEulerRotation;
            bobEulerRotation.x = movementAmount != Vector2.zero ? multiplier.x * Mathf.Sin(2 * speedCurve) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2);
            bobEulerRotation.y = movementAmount != Vector2.zero ? multiplier.y * curveCos : 0;
            bobEulerRotation.z = movementAmount != Vector2.zero ? multiplier.z * curveCos * movementAmount.x : 0;

            return bobEulerRotation;
        }
        #endregion
    }
}