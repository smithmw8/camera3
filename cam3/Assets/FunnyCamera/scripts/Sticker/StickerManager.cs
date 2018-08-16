﻿//----------------------------------------------
// FunnyCamera - realtime camera effects
// Copyright © 2015-2016 52cwalk team
// Contact us (lycwalk@gmail.com)
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CWALK;

public class StickerManager : MonoBehaviour {

	List<StickerItemController> m_stickerItemController;
	public GameObject instanceObj;
	Transform m_SelectedItemObj;

	bool isPressed = false;
	bool m_isNeedApply = false;
	public bool isNeedApply
	{
		get 
		{
			return m_isNeedApply;
		}
	}
	Vector3 ScreenSpace = Vector3.zero;
	Vector3 positionOffset = Vector3.zero;
	bool isMoveItem = false;
	
	bool isRotatePressed = false;
	Quaternion orgRotation;
	float rotateOffset = 0;
	Vector3 orgScale;
	bool isScalePressed;
	float distanceOffset;


	// Use this for initialization
	void Start () {
		m_stickerItemController = new List<StickerItemController> ();
		isPressed = false;
	}

	/*

	void OnGUI()
	{
		if (GUI.Button (new Rect (0, 0, 200, 100), "create")) {
			CreateItem();
		}
	}
	*/
	// Update is called once per frame
	void Update () {
		if (isWork) {
			if (Input.GetKeyDown (KeyCode.A)) {
				CreateItem();
			}
			
			if (Input.GetMouseButtonDown (0)) {

				if(!AppManager.IsMouseOverUI)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					if (Physics.Raycast (ray, out hit)) {
						isPressed = true;
						if(hit.collider.tag == "StickerItem")
						{
							if(m_SelectedItemObj != hit.collider.transform)
							{
								if(this.m_SelectedItemObj != null)
								{
									this.m_SelectedItemObj.GetComponent<StickerItemController>().setActive(false);
								}
							}
							this.m_SelectedItemObj = hit.collider.transform;
							this.m_SelectedItemObj.GetComponent<StickerItemController>().setActive(true);
							isMoveItem = true;
							
							m_isNeedApply = true;
							ScreenSpace = Camera.main.WorldToScreenPoint(this.m_SelectedItemObj.transform.position); 
							
							positionOffset = this.m_SelectedItemObj.transform.position -Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,ScreenSpace.z)) ;
							
						}
						else if(hit.collider.tag == "StickerCloseBtn")
						{
							if(this.m_SelectedItemObj != null)
							{
								this.m_SelectedItemObj.GetComponent<StickerItemController>().DeleteSelfObj();
								this.m_SelectedItemObj = null;
							}
						}
						else if(hit.collider.tag == "StickerRotateScaleBtn")
						{
							if(this.m_SelectedItemObj != null)
							{
								isRotatePressed = true;
								Vector3 movePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
								Vector3	ScreenSpace = Camera.main.WorldToScreenPoint(this.m_SelectedItemObj.transform.position); 
								var dir = movePos - ScreenSpace; 
								rotateOffset = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
								orgRotation =this.m_SelectedItemObj.transform.rotation;
								
								orgScale = this.m_SelectedItemObj.transform.localScale;
								isScalePressed = true;
								movePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
								ScreenSpace = Camera.main.WorldToScreenPoint(this.m_SelectedItemObj.transform.position); 
								distanceOffset = Vector3.Distance(movePos ,ScreenSpace);

							}
						}
						else
						{
							if(this.m_SelectedItemObj != null)
							{
								this.m_SelectedItemObj.GetComponent<StickerItemController>().setActive(false);
								this.m_SelectedItemObj = null;
							}
						}
					}
					else
					{
						if(this.m_SelectedItemObj != null)
						{
							this.m_SelectedItemObj.GetComponent<StickerItemController>().setActive(false);
							this.m_SelectedItemObj = null;
						}
					}
				}

			}
		
			if (isPressed) {
				#if UNITY_EDITOR
				if(Input.GetMouseButton(0))	
				#elif UNITY_ANDROID||UNITY_IOS||UNITY_TIZEN
					if(Input.GetTouch(0).phase== TouchPhase.Moved && Input.touchCount == 1)
						#endif
				{
					if(isMoveItem)
					{
						Vector3 curScreenSpace =  new Vector3(Input.mousePosition.x,Input.mousePosition.y,ScreenSpace.z);  
						Vector3 CurPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + positionOffset;
						this.m_SelectedItemObj.transform.position = new Vector3(CurPosition.x,CurPosition.y,this.m_SelectedItemObj.transform.position.z);  
					}
					
					if(isRotatePressed)
					{
						Vector3 movePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
						var objectPos = Camera.main.WorldToScreenPoint (this.m_SelectedItemObj.transform.position);
						var dir = movePos - objectPos; 
						this.m_SelectedItemObj.transform.rotation =  Quaternion.AngleAxis ( Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg - rotateOffset + orgRotation.eulerAngles.z, 
						                                                        Vector3.forward) ;
						
						
					}
					if(isScalePressed)
					{
						Vector3 movePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
						var objectPos = Camera.main.WorldToScreenPoint (this.m_SelectedItemObj.transform.position);
						float distance = Vector3.Distance(movePos,objectPos);
						float dir = distance/distanceOffset; 
						//		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg));
						this.m_SelectedItemObj.transform.localScale = orgScale * dir;
					}
					
				}
			}

			if (Input.GetMouseButtonUp (0)) {
				

				isRotatePressed = false;
				isMoveItem = false;
				isScalePressed = false;
				isPressed = false;
			}
		}

	}

	
	void OnMouseDrag()
	{

	}
	/// <summary>
	/// Sets the item active.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void setItemActive(GameObject obj)
	{
		for(int i = 0;i!= m_stickerItemController.Count;i++)
		{
			if(m_stickerItemController[i].gameObject == obj)
			{
				m_stickerItemController[i].setActive(true);
			}
			else
			{
				m_stickerItemController[i].setActive(false);
			}
		}
	}

	/// <summary>
	/// Adds the sticker item to list 
	/// </summary>
	/// <param name="item">Item.</param>
	public void AddStickerItem(StickerItemController item)
	{
		if (item != null) {
			m_stickerItemController.Add(item);
		}
	}

	/// <summary>
	/// Removes the sticker item.
	/// </summary>
	/// <param name="item">Item.</param>
	public void RemoveStickerItem(StickerItemController item)
	{
		if (item != null) {
			m_stickerItemController.Remove(item);
		}
		if (m_stickerItemController.Count == 0) {
			m_isNeedApply = false;
		}
	}

	/// <summary>
	/// Clears all sticker items
	/// </summary>
	public void ClearAll()
	{
		for (int i = 0; i!= m_stickerItemController.Count; i++) {
			Destroy(m_stickerItemController[i].gameObject);
		}

		m_stickerItemController.Clear ();
	}

	/// <summary>
	/// Creates the item.
	/// </summary>
	public void CreateItem()
	{
		if (instanceObj != null) {
			GameObject obj = Instantiate(instanceObj,this.transform.position,Quaternion.identity) as GameObject;
			obj.transform.parent = this.transform;

			AddStickerItem(obj.GetComponent<StickerItemController>());

			obj.GetComponent<StickerItemController>().initItemByMode("sticker_16");
			obj.GetComponent<StickerItemController>().setStickerManager(this);
			m_SelectedItemObj = obj.transform;
			setItemActive(obj);
		}
	}

	/// <summary>
	/// Creates the item by sticker name
	/// </summary>
	/// <param name="stickerName">Sticker name.</param>
	public bool CreateItem(string stickerName)
	{
		if (instanceObj != null) {
			GameObject obj = Instantiate (instanceObj, this.transform.position, Quaternion.identity) as GameObject;
			obj.transform.parent = this.transform;

			bool initValue = obj.GetComponent<StickerItemController> ().initItemByMode (stickerName);
			if (initValue) {
				AddStickerItem (obj.GetComponent<StickerItemController> ());
				obj.GetComponent<StickerItemController> ().setStickerManager (this);
				m_SelectedItemObj = obj.transform;
				setItemActive (obj);
				m_isNeedApply = true;
				return true;
			} else {
				Destroy (obj);
				return false;
			}

		} else {
			return false;
		}
	}

	bool isWork = false;
	public void setWork(bool value)
	{
		isWork = value;
		m_isNeedApply = false;
		if (this.m_SelectedItemObj != null) {
			this.m_SelectedItemObj.GetComponent<StickerItemController>().setActive(false);
		}
	}

	public void sharePanelWork(bool value)
	{
		if (value) {
			if (this.m_SelectedItemObj != null) {
				this.m_SelectedItemObj.GetComponent<StickerItemController>().setActive(false);
			}
		}
	}
}
