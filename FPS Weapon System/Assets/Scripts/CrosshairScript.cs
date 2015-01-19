using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Camera-Control/Crosshair")]
[ExecuteInEditMode()]
public class CrosshairScript : MonoBehaviour {

    public Texture2D crosshair;
    /*
	public Texture2D crosshairTop;
	public Texture2D crosshairRight;
	public Texture2D crosshairButtom;
	public Texture2D crosshairLeft;
    
	
	public float offsetTopLeft = 12;
	public float offsetButtomRight = 6;

    public float maxTopLeftOffset = 20f;
    public float maxButtomRightOffset = 14f;
    */
	//private bool aiming = false;

	void Awake(){

	}

	void OnGUI () {
		var centerW = Screen.width / 2;
		var centerH = Screen.height / 2;
        /*
        var r = Screen.width / Screen.height;
        offsetTopLeft *= r;
        offsetButtomRight *= r;
         */
        /*
		if(!aiming) {
         */
            GUI.DrawTexture(new Rect(centerW, centerH, crosshair.width, crosshair.height), crosshair);
           /* 
			GUI.DrawTexture (new Rect (centerW  - offsetTopLeft, centerH, crosshairTop.height, crosshairTop.width), crosshairTop);
			GUI.DrawTexture (new Rect (centerW, centerH + offsetButtomRight, crosshairRight.height, crosshairRight.width), crosshairRight);	
			GUI.DrawTexture (new Rect (centerW + offsetButtomRight, centerH , crosshairButtom.height, crosshairButtom.width), crosshairButtom);
			GUI.DrawTexture (new Rect (centerW, centerH - offsetTopLeft, crosshairLeft.height, crosshairLeft.width), crosshairLeft);
		    */
        }

	
	
	// Update is called once per frame
	void Update () {

	}

}
