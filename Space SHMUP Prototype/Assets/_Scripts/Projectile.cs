using UnityEngine;
using System.Collections;


public class Projectile : MonoBehaviour {

	[SerializeField]
	private WeaponType _type;
	//This public property makes the field _type & takes action when it is set
	public WeaponType type
	{
		get
		{
			return(_type);
		}
		set
		{
			SetType(value);
		}
	}

	void Awake()
	{
		//Test ti see wether this has passed off screen every 2 seconds
		InvokeRepeating ("CheckOffScreen", 2f, 2f);
	}

	public void SetType(WeaponType eType)
	{
		//Set the _type
		_type = eType;
		WeaponDefinition def = Main.GetWeaponDefinition (_type);
		renderer.material.color = def.projectileColor;
	}

	void CheckOffScreen()
	{
		if (Utils.ScreenBoundsCheck(collider.bounds, BoundsTest.offScreen) != Vector3.zero)
		{
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}//last paste
}
