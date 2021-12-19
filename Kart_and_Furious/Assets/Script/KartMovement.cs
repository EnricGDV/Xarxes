using UnityEngine;

namespace Complete
{
    public class KartMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public float m_Speed = 12f;                 // How fast the tank moves forward and back.
        public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.

        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        private Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.

        class Key
        {
            public bool keyDown = false;
            public bool keyRepeat = false;
            public bool keyUp = false;
        }
        private Key keyW;
        private Key keyS;
        private Key keyA;
        private Key keyD;


        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }


        private void OnEnable()
        {
            // When the tank is turned on, make sure it's not kinematic.
            m_Rigidbody.isKinematic = false;

            // Also reset the input values.
            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;

            // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
            // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
            // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
        }


        private void OnDisable()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;

            // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
        }


        private void Start()
        {
            // The axes names are based on player number.
            m_MovementAxisName = "Vertical" + m_PlayerNumber;
            m_TurnAxisName = "Horizontal" + m_PlayerNumber;
        }


        private void Update()
        {
            GatherInput();
        }


        private void FixedUpdate()
        {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            Move();
            Turn();
        }


        private void Move()
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


        private void Turn()
        {
            // Determine the number of degrees to be turned based on the input, speed and time between frames.
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

            // Make this into a rotation in the y axis.
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

            // Apply this rotation to the rigidbody's rotation.
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        }

        void GatherInput()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (keyW.keyDown)
                {
                    keyW.keyDown = false;
                    keyW.keyRepeat = true;
                }
                else
                    keyW.keyDown = true;
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                keyW.keyDown = false;
                keyW.keyRepeat = false;
                keyW.keyUp = true;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (keyS.keyDown)
                {
                    keyS.keyDown = false;
                    keyS.keyRepeat = true;
                }
                else
                    keyS.keyDown = true;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                keyS.keyDown = false;
                keyS.keyRepeat = false;
                keyS.keyUp = true;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                if (keyA.keyDown)
                {
                    keyA.keyDown = false;
                    keyA.keyRepeat = true;
                }
                else
                    keyA.keyDown = true;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                keyA.keyDown = false;
                keyA.keyRepeat = false;
                keyA.keyUp = true;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (keyD.keyDown)
                {
                    keyD.keyDown = false;
                    keyD.keyRepeat = true;
                }
                else
                    keyD.keyDown = true;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                keyD.keyDown = false;
                keyD.keyRepeat = false;
                keyD.keyUp = true;
            }
        }
    }
}