using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class KartMovement : MonoBehaviour
    {
        public ClientScript clientScript;

        public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public float m_Speed = 12f;                 // How fast the tank moves forward and back.
        public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.

        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        private Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.

        public class Key
        {
            public bool keyDown = false;
            public bool keyRepeat = false;
            public bool keyUp = false;
        }
        public Key keyW;
        public Key keyS;
        public Key keyA;
        public Key keyD;

        private void Awake()
        {
            clientScript = GameObject.Find("Client").GetComponent<ClientScript>();

            keyW = new Key();
            keyS = new Key();
            keyA = new Key();
            keyD = new Key();

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
            SendInput();
        }


        private void FixedUpdate()
        {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            Move();
            Turn();
            UpdateKeys();
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

        // Updates the key states
        void UpdateKeys()
        {
            // W
            if (keyW.keyDown) 
            {
                keyW.keyDown = false;
                keyW.keyRepeat = true;
            }
            else if (keyW.keyUp)
            {
                keyW.keyDown = false;
                keyW.keyRepeat = false;
                keyW.keyUp = false;
            }

            // S
            if (keyS.keyDown)
            {
                keyS.keyDown = false;
                keyS.keyRepeat = true;
            }
            else if (keyS.keyUp)
            {
                keyS.keyDown = false;
                keyS.keyRepeat = false;
                keyS.keyUp = false;
            }

            // A
            if (keyA.keyDown)
            {
                keyA.keyDown = false;
                keyA.keyRepeat = true;
            }
            else if (keyA.keyUp)
            {
                keyA.keyDown = false;
                keyA.keyRepeat = false;
                keyA.keyUp = false;
            }

            // D
            if (keyD.keyDown)
            {
                keyD.keyDown = false;
                keyD.keyRepeat = true;
            }
            else if (keyD.keyUp)
            {
                keyD.keyDown = false;
                keyD.keyRepeat = false;
                keyD.keyUp = false;
            }
        }

        // Sends the inputs to the server
        void SendInput()
        {
            // W
            if (Input.GetKeyDown(KeyCode.W))
            {
                clientScript.SendMessageToServer("KeyDownW");
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                clientScript.SendMessageToServer("KeyUpW");
            }

            // S
            if (Input.GetKeyDown(KeyCode.S))
            {
                clientScript.SendMessageToServer("KeyDownS");
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                clientScript.SendMessageToServer("KeyUpS");
            }

            // A
            if (Input.GetKeyDown(KeyCode.A))
            {
                clientScript.SendMessageToServer("KeyDownA");
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                clientScript.SendMessageToServer("KeyUpA");
            }

            // D
            if (Input.GetKeyDown(KeyCode.D))
            {
                clientScript.SendMessageToServer("KeyDownD");
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                clientScript.SendMessageToServer("KeyUpD");
            }
        }
    }
}