﻿using UnityEngine;
using UnityEngine.UI;

public class UIJoystick : MonoBehaviour
{
	void Update()
	{
        if (GameController.mMainPlayer == null)
            return;
		Vector3 touchPos = Input.mousePosition;
		bool touchBegin = false;
		bool touchEnd = false;
		if (Input.touchCount > 0)
		{
			if (Input.touches[0].phase == TouchPhase.Began)
			{
				touchBegin = true;
			}
			if (Input.touches[0].phase == TouchPhase.Ended)
			{
				touchEnd = true;
			}
			touchPos = Input.touches[0].position;
		}
        if (Input.GetMouseButtonDown(0))
        {
            touchBegin = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            touchEnd = true;
        }

		if (touchBegin)
		{
            if (touchPos.y > (float)(Screen.height / 2)
                || touchPos.x > (float)(Screen.width / 2))
                return;
            mStartPos = touchPos;

			joystick.gameObject.SetActive(true);
			Vector3 pos = joystick.position;
			pos.x = mStartPos.x;
			pos.y = mStartPos.y;
			joystick.position = pos;
			mDraging = true;
		}

		if (touchEnd)
		{
            if (!mDraging)
            {
                PickObject.Pick(touchPos);
            }
			mStartPos = Vector2.zero;
			joystick.gameObject.SetActive(false);
			mDraging = false;

			if (GameController.mMainPlayer != null)
			{
				GameController.mMainPlayer.MoveSpeed = 0.0f;
			}
		}

		if (mDraging)
		{
			Vector2 delta = (Vector2)touchPos - mStartPos;

			float length = delta.magnitude;
			if (length > joystickRadius)
			{
				delta = delta.normalized * joystickRadius;
				length = joystickRadius;
			}

			Vector3 pos = bar.localPosition;
			pos.x = delta.x;
			pos.y = delta.y;
			bar.localPosition = pos;

			if (GameController.mMainPlayer != null && MobaMainCamera.MainCamera != null
                && !GameController.mMainPlayer.IsCannotFlag(CannotFlag.CannotControl))
			{
                float strength = 1.0f; //length / joystickRadius;
				Vector3 joystickDir = new Vector3(delta.x, 0.0f, delta.y);
				Quaternion rot = MobaMainCamera.MainCamera.transform.rotation;
				Vector3 dir = rot * joystickDir;

				GameController.mMainPlayer.Direction = dir;
				GameController.mMainPlayer.MoveSpeed = 3.0f * strength;
                GameController.mMainPlayer.IsControl = true;
            }
		}
        else
        {
            if (!GameController.mMainPlayer.IsCannotFlag(CannotFlag.CannotControl))
            {
                GameController.mMainPlayer.IsControl = false;
            }
        }
	}

	bool mDraging = false;
	Vector2 mStartPos = Vector2.zero;

	public RectTransform joystick;
	public RectTransform bar;
	public float joystickRadius = 32.0f;
}