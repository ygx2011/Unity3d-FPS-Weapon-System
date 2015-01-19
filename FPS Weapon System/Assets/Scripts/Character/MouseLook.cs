#pragma warning disable 108

using UnityEngine;
using System.Collections;


[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;
    float rotationX = 0F;

    private Transform camera;

    void Awake()
    {
        camera = Camera.main.transform;
    }

	void FixedUpdate ()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
            var mouseX = Input.GetAxis("Mouse X");
			rotationX = transform.localEulerAngles.y + mouseX * sensitivityX;		
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
            camera.localEulerAngles = new Vector3(-rotationY, 0, 0);

		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            camera.localEulerAngles = new Vector3(-rotationY, 0, 0);
        }

    }

}