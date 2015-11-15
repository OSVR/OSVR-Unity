using UnityEngine;
using System.Collections;

// MouseLook rotates the transform based on the mouse delta.
// Minimum and Maximum values can be used to constrain the possible rotation
// Attach this script to an FPS style character.
public class OsvrMouseLook : MonoBehaviour
{
    public KeyCode MouseLookToggleKey = KeyCode.M;
    public bool useMouseLook = false;
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2, RightJoystick = 3 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;
    private float lastMouseLookTime = 0;

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && Time.time - lastMouseLookTime > 0.3f)
        {
            lastMouseLookTime = Time.time;
            useMouseLook = !useMouseLook;
        }
        if (!useMouseLook)
        {
            return;
        }
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.RightJoystick)
        {
            if (Mathf.Abs(Input.GetAxis("Right Joystick")) > 0.08f)
            {
                transform.Rotate(0, Input.GetAxis("Right Joystick") * sensitivityX, 0);
            }
        }
        else if (axes == RotationAxes.MouseY)
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
    }
}