using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum BoundsTest
{
	center,  //Is the center of the GameObject on screen?
	onScreen,  //Are the bounds entierly on screen?
	offScreen  //Are the bounds entierly off screen?
}

public class Utils : MonoBehaviour {

	//================ Bounds Functions ==================\\

	//creates bounds that encapsulates the two bounds passed in.
	public static Bounds BoundsUnion(Bounds b0, Bounds b1)
	{
		//If the size of the bounds is the Vector3.zero, ignore that one
		if (b0.size == Vector3.zero && b1.size != Vector3.zero)
		{
			return(b1);
		} else if (b0.size != Vector3.zero && b1.size == Vector3.zero) {
			return(b0);
		} else if (b0.size == Vector3.zero && b1.size == Vector3.zero) {
			return (b0);
		}
		//Stretch b0 to include the b1.min and b1.max
		b0.Encapsulate(b1.min);
		b0.Encapsulate(b1.max);
		return(b0);
	}

	public static Bounds CombineBoundsOfChildren(GameObject go)
	{
		//create an empty Bounds b
		Bounds b = new Bounds (Vector3.zero, Vector3.zero);
		//If this GameObject has a renderer component...
		if (go.renderer != null)
		{
			//Expand b to contain the renderer's bounds
			b = BoundsUnion(b, go.renderer.bounds);
		}
		//If this GameObject has a Collider component...
		if (go.collider != null)
		{
			//Expand b to contain the colliders bounds
			b = BoundsUnion(b, go.collider.bounds);
		}
		//Recursevly iterate through each child of this gameObject.transform
		foreach(Transform t in go.transform)
		{
			//Expand b to contain their bounds as well
			b = BoundsUnion(b, CombineBoundsOfChildren(t.gameObject));
		}
		return(b);
	}

	//Make a static read-only public property camBounds
	static public Bounds camBounds
	{
		get 
		{
			//if _camBounds hasn't been set yet
			if (_camBounds.size == Vector3.zero)
			{
				//SetCameraBounds using the default Camera
				SetCameraBounds();
			}
			return(_camBounds);
		}
	}

	//This is the private static field that camBounds uses
	static private Bounds _camBounds;

	//This function is used by camBounds to set _camBounds and can also be called directly
	public static void SetCameraBounds(Camera cam=null)
	{
		//If no camera was passed in, use the main camera
		if (cam == null)cam = Camera.main;
		//This makes a couple of important assumptions about the camera:
		//1.  The camera is Orthographic
		//2.  The camera is at a rotation of R:[0,0,0]

		//Make Vector3s at the topLeft and bottomRight of the Screen coords
		Vector3 topLeft = new Vector3 (0, 0, 0);
		Vector3 bottomRight = new Vector3 (Screen.width, Screen.height, 0);

		//Convert these to workd coordinates
		Vector3 boundTLN = cam.ScreenToWorldPoint (topLeft);
		Vector3 boundBRF = cam.ScreenToWorldPoint (bottomRight);

		//Adjust their zs to be at the near and far Camera clipping planes
		boundTLN.z += cam.nearClipPlane;
		boundBRF.z += cam.farClipPlane;

		//Find the center of the Bounds
		Vector3 center = (boundTLN + boundBRF) / 2f;
		_camBounds = new Bounds (center, Vector3.zero);
		//Expand _camBounds to encapsulate the extents.
		_camBounds.Encapsulate (boundTLN);
		_camBounds.Encapsulate (boundBRF);
	}

	//Checks to see whether the Bounds bnd are within the camBounds
	public static Vector3 ScreenBoundsCheck(Bounds bnd, BoundsTest test = BoundsTest.center)
	{
		return(BoundsInBoundsCheck (camBounds, bnd, test));
	}

	//Checks to see whether Bounds lilB are within Bounds bigB
	public static Vector3 BoundsInBoundsCheck(Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen)
	{
		//The behavior of this function is different based on the BoundsTest that has been selected

		//Get the cdenter of lilB
		Vector3 pos = lilB.center;

		//Initialize the offset at [0,0,0]
		Vector3 off = Vector3.zero;

		switch (test)
		{
			//The center test determines what off (offset) would have to be applied to lilB to move its center back inside bigB
			case BoundsTest.center:
				if (bigB.Contains(pos))
				{
					return(Vector3.zero);
				}
				if (pos.x > bigB.max.x)
				{
					off.x = pos.x - bigB.max.x;
				} else if (pos.x < bigB.min.x) {
					off.x = pos.x - bigB.min.x;
				}
				if (pos.y > bigB.max.y)
				{
					off.y = pos.y - bigB.max.y;
				} else if (pos.y < bigB.min.y) {
					off.y = pos.y - bigB.min.y;
				}
				if (pos.z > bigB.max.z)
				{
					off.z = pos.z - bigB.max.z;
				} else if (pos.z < bigB.min.z) {
					off.z = pos.z - bigB.min.z;
				}
				return(off);  //At this point, stopped caring about indents.

			//The onScreen test determines what off would have to be applied to keep all of lilB inside bigB
		case BoundsTest.onScreen:
			if (bigB.Contains(lilB.min) && bigB.Contains(lilB.max))
			{
				return(Vector3.zero);
			}
			if (lilB.max.x > bigB.max.x)
			{
				off.x = lilB.max.x - bigB.max.x;
			} else if (lilB.min.x < bigB.min.x) {
				off.x = lilB.min.x - bigB.min.x;
			}
			if (lilB.max.y > bigB.max.y)
			{
				off.y = lilB.max.y - bigB.max.y;
			} else if (lilB.min.y < bigB.min.y) {
				off.y = lilB.min.y - bigB.min.y;
			}
			if (lilB.max.z > bigB.max.z) 
			{
				off.z = lilB.max.z - bigB.max.z;
			} else if (lilB.min.z < bigB.min.z) {
				off.z = lilB.min.z - bigB.min.z;
			}
			return(off);

			//The offscreen test determines what off would need to be applied to move any tiny part of lilB inside of bigB
		case BoundsTest.offScreen:
			bool cMin = bigB.Contains(lilB.min);
			bool cMax = bigB.Contains(lilB.max);
			if (cMin || cMax)
			{
				return(Vector3.zero);
			}
			if (lilB.min.x > bigB.max.x)
			{
				off.x = lilB.min.x - bigB.max.x;
			} else if (lilB.max.x < bigB.max.x) {
				off.x = lilB.max.x - bigB.min.x;
			}
			if (lilB.min.y > bigB.max.y)
			{
				off.y = lilB.min.y - bigB.max.y;
			} else if (lilB.max.y < bigB.max.y) {
				off.y = lilB.max.y - bigB.min.y;
			}
			if (lilB.min.z > bigB.max.z)
			{
				off.z = lilB.min.z - bigB.max.z;
			} else if (lilB.max.z < bigB.max.z) {
				off.z = lilB.max.z - bigB.min.z;
			}
			return(off);
		}
		return(Vector3.zero);
	}

	//================Transform Functions=========================

	//This function will interatively climb up the transform.parent tree until it either finds a parent with a tag != "Untagged" or no parent
	public static GameObject FindTaggedParent(GameObject go)
	{
		//If GameObject has a tag...
		if (go.tag != "Untagged")
		{
			//then return this gameObject
			return(go);
		}
		//If there is no parent of this Transform
		if (go.transform.parent == null)
		{
			//We've reached the top of the heiarchy with no interesting tag so we return null.
			return(null);
		}
		//Otherwise, recursively climb up the tree
		return(FindTaggedParent (go.transform.parent.gameObject));
	}
	//This version of the function handels things if a transform is passed in
	public static GameObject FindTaggedParent (Transform t)
	{
		return(FindTaggedParent(t.gameObject));
	}

	//====================Materials Functions==========================
	
	//Returns a list of all meterials on this GameObject or its children
	static public Material[] GetAllMaterials(GameObject go)
	{
		List<Material> mats = new List<Material>();
		if (go.renderer != null)
		{
			mats.Add(go.renderer.material);
		}
		foreach(Transform t in go.transform)
		{
			mats.AddRange(GetAllMaterials(t.gameObject));
		}
		return(mats.ToArray());
	}
	//last paste
}
