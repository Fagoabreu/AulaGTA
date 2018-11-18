using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class HeliControl : MonoBehaviour {
    
    //Variaveis GameObject
    [SerializeField] GameObject main_rotor_Gameobject;
    [SerializeField] GameObject static_rotor;
    [SerializeField] GameObject speed_rotor;
    [SerializeField] GameObject tail_rotor_Gameobject;
    [SerializeField] GameObject speed_tail_rotor;

    //Variaveis do Rotor
    [SerializeField] float max_rotor_force = 22241.1081f;
    [SerializeField] float max_rotor_velocity = 7200;
    private float rotor_velocity = 0f;
    private float rotor_rotation = 0f;

    //Rariaveis do rotor da calda
    [SerializeField] float max_tail_rotor_force = 1500.0f;
    [SerializeField] float max_tail_rotor_velocity = 4500;
    private float tail_rotor_velocity = 0f;
    private float tail_rotor_rotation = 0f;

    [SerializeField] float forward_rotor_torque_multiplier = 0.3f;
    [SerializeField] float sideways_rotor_torque_multiplier = 0.2f;

    [SerializeField] private bool main_rotor_active=true;
    [SerializeField] private bool tail_rotor_active = true;

    //componentes
    Rigidbody _rigidbody;
    Vector3 torqueValue;
    AudioSource audioSource;

    [SerializeField] float hover_rotor_velocity;
    [SerializeField] float hover_tail_rotor_velocity;

    private bool on;
    public bool onUse;
    public float YMax;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Move()
    {
        //se motor ligado
        if (on)
        {
            Vector3 controlTorque = new Vector3(Input.GetAxis(StaticInput.Vertical) * forward_rotor_torque_multiplier,
                1.0f, Input.GetAxis(StaticInput.Horizontal2) * sideways_rotor_torque_multiplier * Time.deltaTime);

            if (main_rotor_active)
            {
                torqueValue += (controlTorque * max_rotor_force * rotor_velocity);

                if (transform.position.y < YMax)
                {
                    _rigidbody.AddRelativeForce(Vector3.up * max_rotor_force *rotor_velocity);
                }

                if(Vector3.Angle(Vector3.up,transform.up) < 80)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0),
                        Time.deltaTime * rotor_velocity * 2f);
                }
            }

            if (tail_rotor_active)
            {
                torqueValue -= (Vector3.up * max_tail_rotor_force * tail_rotor_velocity);
            }

            _rigidbody.AddRelativeTorque(torqueValue * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        if (onUse)
        {
            if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), StaticInput.Action)))
            {
                on = !on;
            }
        }
        else
        {
            on = false;
        }

        if (on)
        {
            audioSource.pitch = rotor_velocity;

            if(speed_rotor != null && static_rotor != null && speed_tail_rotor != null)
            {
                if(rotor_velocity > 0.4f)
                {
                    speed_rotor.SetActive(true);
                    static_rotor.SetActive(false);
                    speed_tail_rotor.SetActive(true);
                }
                else
                {
                    speed_rotor.SetActive(false);
                    static_rotor.SetActive(true);
                    speed_tail_rotor.SetActive(false);
                }
            }

            if (main_rotor_active)
            {
                main_rotor_Gameobject.transform.rotation = transform.rotation * Quaternion.Euler(0, rotor_rotation, 0);
            }
            if (tail_rotor_active)
            {
                tail_rotor_Gameobject.transform.rotation = transform.rotation * Quaternion.Euler(tail_rotor_rotation,0, 0);
            }

            rotor_rotation += max_rotor_velocity * rotor_velocity * Time.deltaTime;
            tail_rotor_rotation += max_tail_rotor_velocity * rotor_velocity * Time.deltaTime;

            hover_rotor_velocity = (_rigidbody.mass * Mathf.Abs(Physics.gravity.y) / max_rotor_force);
            hover_tail_rotor_velocity = (max_rotor_force * rotor_velocity) / max_tail_rotor_force;

            if (Input.GetAxis(StaticInput.Vertical2) != 0.0f)
            {
                rotor_velocity += Input.GetAxis(StaticInput.Vertical2) * 0.001f;
            }
            else {
                rotor_velocity = Mathf.Lerp(rotor_velocity, hover_rotor_velocity, Time.deltaTime * Time.deltaTime * 5f);
            }

            tail_rotor_velocity = hover_tail_rotor_velocity - Input.GetAxis(StaticInput.Horizontal);

            if (rotor_velocity > 1.0f)
            {
                rotor_velocity = 1.0f;
            }
            else if(rotor_velocity < 0.0f)
            {
                rotor_velocity = 0.0f;
            }
        }
        else
        {
            rotor_rotation -= max_rotor_velocity * rotor_velocity * Time.deltaTime;
            rotor_velocity -= 0.03f * Time.deltaTime;
            audioSource.pitch = rotor_velocity;

            if (main_rotor_active)
            {
                main_rotor_Gameobject.transform.rotation = transform.rotation * Quaternion.Euler(0, rotor_rotation, 0);
            }
            if (tail_rotor_active)
            {
                tail_rotor_Gameobject.transform.rotation = transform.rotation * Quaternion.Euler(tail_rotor_rotation, 0, 0);
            }

            if (rotor_velocity < 0.0f)
            {
                rotor_velocity = 0.0f;
            }

            if (speed_rotor != null && static_rotor != null && speed_tail_rotor != null)
            {
                if (rotor_velocity > 0.4f)
                {
                    speed_rotor.SetActive(true);
                    static_rotor.SetActive(false);
                    speed_tail_rotor.SetActive(true);
                }
                else
                {
                    speed_rotor.SetActive(false);
                    static_rotor.SetActive(true);
                    speed_tail_rotor.SetActive(false);
                }
            }
        }
    }
}
