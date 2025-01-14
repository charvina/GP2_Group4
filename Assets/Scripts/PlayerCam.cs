using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    public PostProcessVolume v;
    private Grain g;
    private GameObject creature;
    //public GameObject camera;

    public GameObject pauseMenu, gameOverScreen;

    private AudioSource audio;
    public AudioClip noise;

    LayerMask defaultMask;
    LayerMask transparentFX;
    LayerMask ignoreRaycast;
    LayerMask action;
    LayerMask water;
    LayerMask ui;
    LayerMask whatIsGround;
    LayerMask postFX;

    void Start()
    {
        defaultMask = LayerMask.NameToLayer("Default");
        transparentFX = LayerMask.NameToLayer("TransparentFx");
        ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        action = LayerMask.NameToLayer("Action");
        water = LayerMask.NameToLayer("Water");
        ui = LayerMask.NameToLayer("UI");
        whatIsGround = LayerMask.NameToLayer("whatIsGround");
        postFX = LayerMask.NameToLayer("Post Processing");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        v.profile.TryGetSettings(out g);

        audio = GetComponent<AudioSource>();
        audio.clip = noise;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            //Application.Quit();
            pauseMenu.SetActive(true);
        }

        if ((pauseMenu.activeSelf) == true || (gameOverScreen.activeSelf == true))
        {
            g.intensity.value = 0f;
            audio.Stop();
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (gameOverScreen.activeSelf == true)
            {
                //creature.SetActive(false);
                this.GetComponent<Camera>().cullingMask = (1 << ui);
            }
        }
        else
        {
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Add gamepad support
            float mouseX = Input.GetAxisRaw("Mouse X") * sensX + Input.GetAxis("RightStickHorizontal") * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensY + Input.GetAxis("RightStickVertical") * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -15f, 30f);
        }

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = GameObject.Find("Monster(Clone)").transform.position;

        if (Vector3.Distance(currentPosition, transform.position) <= 15f)
        {
            audio.Play();
            if (g.intensity.value < 1f)
            {
                g.intensity.value = g.intensity.value + 0.01f;
            }
        }
        else if (Vector3.Distance(currentPosition, transform.position) >= 20f)
        {
            g.intensity.value = 0f;
            audio.Stop();
        }
        else
        {
            
            if (g.intensity.value > 0f)
            {
                g.intensity.value = g.intensity.value - 0.01f;
            }
            else
            {
                audio.Stop();
            }
        }
    }
}
