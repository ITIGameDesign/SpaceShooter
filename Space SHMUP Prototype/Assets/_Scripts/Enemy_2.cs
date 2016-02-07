using UnityEngine;
using System.Collections;

public class Enemy_2 : Enemy {

	//Enemy_2 uses a Sin wave to modify a 2-point linear interpolation
	public Vector3[] points;
	public float birthTime;
	public float lifeTime = 10;
	//Determines how much the Sine wave will affect movemednt
	public float sinEccentricity = 0.6f;

	// Use this for initialization
	void Start () {
		//Initiate the points
		points = new Vector3[2];

		//Find Utils.camBounds
		Vector3 cbMin = Utils.camBounds.min;
		Vector3 cbMax = Utils.camBounds.max;

		Vector3 v = Vector3.zero;
		//pick any point on the left side of the screen
		v.x = cbMin.x - Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [0] = v;

		//Pick any point on the right side of the screen
		v = Vector3.zero;
		v.x = cbMax.x + Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [1] = v;

		//Possibly swap sides
		if (Random.value < 0.5f)
		{
			//Setting the .x of each point to its negative will moce it to the other side of the screen
			points[0].x *= -1;
			points[1].x *= -1;
		}

		//Set bitrhTime to the current time
		birthTime = Time.time;
	}
	//last paste

	public override void Move ()
	{
		//Bézier curves work based on a u value between 0 & 1
		float u = (Time.time - birthTime) / lifeTime;

		//If u>1, then it has been longer than lifeTime since birthTime
		if (u>1)
		{
			//This nemy_2 has finished it's life
			Destroy(this.gameObject);
			return;
		}

		//Adjust u by adding an easing curve based on a Sine wave
		u = u + sinEccentricity * (Mathf.Sin (u * Mathf.PI * 2));

		//Interpolate the two linear interpolation points
		pos = (1 - u) * points [0] + u * points [1];
	}
}
