using UnityEngine;
using System.Collections;
using Unity.Netcode;
using TMPro;

public class MovingPlatform : NetworkBehaviour
{

	[SerializeField] public TMP_Text countDownText;

	public bool upDown;

	public bool sideSide;

	private float timer;
	private int state = 0; //0 - move1, 1 - pause, 2- move2, 3 - pause2

	public float waitTime = 2;

	public float time = 2;

	public float speed = 2;

	// Use this for initialization
	void Start()
	{
		state = 0;
		timer = 0;
	}

	void MovingObject()
    {
		if (sideSide == true)
		{
			if (state == 0)
			{
				timer += Time.deltaTime;
				transform.Translate(Vector3.forward * Time.deltaTime * speed);
				if (timer >= time - 0.4)
				{
					speed = 1;
				}
				if (timer >= time)
				{
					speed = 2;
					timer = 0;
					state = 1;
				}
			}
			if (state == 1)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 2;
				}
			}
			if (state == 2)
			{
				timer += Time.deltaTime;
				transform.Translate(Vector3.forward * Time.deltaTime * -speed);
				if (timer >= time - 0.4)
				{
					speed = 1;
				}
				if (timer >= time)
				{
					speed = 2;
					timer = 0;
					state = 3;
				}
			}
			if (state == 3)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 0;
				}
			}
		}

		if (upDown == true)
		{
			if (state == 0)
			{
				timer += Time.deltaTime;
				transform.Translate(Vector3.left * Time.deltaTime * speed);
				if (timer >= time)
				{
					timer = 0;
					state = 1;
				}
			}
			if (state == 1)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 2;
				}
			}
			if (state == 2)
			{
				timer += Time.deltaTime;
				transform.Translate(Vector3.left * Time.deltaTime * -speed);
				if (timer >= time)
				{
					timer = 0;
					state = 3;
				}
			}
			if (state == 3)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 0;
				}
			}
		}
	}

	[ClientRpc]

	void MovingObjectClientRpc()
    {
		if (sideSide == true)
		{
			if (state == 0)
			{
				timer += Time.deltaTime;
				transform.Translate(Vector3.forward * Time.deltaTime * speed);
				if (timer >= time - 0.4)
				{
					speed = 1;
				}
				if (timer >= time)
				{
					speed = 2;
					timer = 0;
					state = 1;
				}
			}
			if (state == 1)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 2;
				}
			}
			if (state == 2)
			{
				timer += Time.deltaTime;
				transform.Translate(Vector3.forward * Time.deltaTime * -speed);
				if (timer >= time - 0.4)
				{
					speed = 1;
				}
				if (timer >= time)
				{
					speed = 2;
					timer = 0;
					state = 3;
				}
			}
			if (state == 3)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 0;
				}
			}
		}

		if (upDown == true)
		{
			if (state == 0)
			{
				timer += Time.deltaTime;
				transform.Translate(Vector3.left * Time.deltaTime * speed);
				if (timer >= time)
				{
					timer = 0;
					state = 1;
				}
			}
			if (state == 1)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 2;
				}
			}
			if (state == 2)
			{
				timer += Time.deltaTime;
				transform.Translate(Vector3.left * Time.deltaTime * -speed);
				if (timer >= time)
				{
					timer = 0;
					state = 3;
				}
			}
			if (state == 3)
			{
				timer += Time.deltaTime;
				if (timer >= waitTime)
				{
					timer = 0;
					state = 0;
				}
			}
		}
	}
	// Update is called once per frame
	void Update()
	{
		if(countDownText.text == "0")
        {
			MovingObject();
			MovingObjectClientRpc();
		}
		

	}
}
