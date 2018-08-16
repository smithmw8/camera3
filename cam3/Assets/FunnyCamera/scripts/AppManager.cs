﻿//----------------------------------------------
// FunnyCamera - realtime camera effects
// Copyright © 2015-2016 52cwalk team
// Contact us (lycwalk@gmail.com)
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using CWALK;
using DG.Tweening;

public class AppManager : MonoBehaviour {

	public Material[] effectMatArr;
	public GameObject[] planeObjArr;
	public WebCameraController e_webCameraTex;

	public GameObject e_PlayVideoItemBtnUGUI;
	public GameObject e_PlayPhotoItemBtnUGUI;
	public GameObject e_PlayVideoPauseItemBtnUGUI;

	public GameObject m_PageModePanlObjUGUI;
	public GameObject e_mediaMainPanlObjUGUI;
	public GameObject e_decoratePanlObjUGUI;
	public GameObject e_stikcerScrowViewPanlObjUGUI;
	public GameObject e_frameScrowViewPanlObjUGUI;
	public GameObject e_textPanlObjUGUI;

	public GameObject e_BackBtnObjUGUI;
	public GameObject e_ShareBtnObjUGUI;
	public GameObject e_ReverseCameraBtnObjUGUI;

	public Toggle toggleobj;

	public PAGEMODE m_PageMode;

	int selectPageIndex = 0;
	int m_PrePageNum = 4;
	public Text e_PageNoLabel;
	StickerManager m_stickerController;
	FrameController m_frameController;
	TextManager m_textController;
	public TipWindowController e_tipWindowCtr;
	public GameObject e_PhotoMaskTex;
	bool m_isReco = false;
	Texture2D mPhotoTex;
	Ray ray;

	public static AppManager instance;
	public PhotoController e_photoController;
	// Use this for initialization
	void Start () {
		
		m_PageMode = PAGEMODE.FLITER_MENU;
		m_stickerController = GameObject.Find ("StickerManager").GetComponent<StickerManager>();
		m_frameController = GameObject.Find ("FrameManager").GetComponent<FrameController> ();
		m_textController = GameObject.Find("TextManager").GetComponent<TextManager> ();

		instance = this;
	}

	public void StartRecordingEvent()
	{
		e_PlayVideoPauseItemBtnUGUI.GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.6f);
		e_PlayVideoItemBtnUGUI.GetComponent<RectTransform>().DOScale(new Vector3(0,0,0),0.6f);
	}

	public void OnStopRecordingEvent() {
		e_PlayVideoPauseItemBtnUGUI.GetComponent<RectTransform>().DOScale(new Vector3(0,0,0),0.6f);
		e_PlayVideoItemBtnUGUI.GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.6f);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0) && m_PageMode == PAGEMODE.FLITER_MENU) {
			if(!AppManager.IsMouseOverUI)
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 100))
				{
					if(hit.collider.tag == "itemPlane")
					{
						PoivtMode mode = hit.collider.GetComponent<ItemPlaneController>().e_ItemPoivtMode;
						showMainItem(mode);

						m_PageModePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
						e_mediaMainPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
		
						m_PageMode = PAGEMODE.FLITER_PREVIEW;
					}
				}
			}
		}
		
		if (e_webCameraTex != null&&e_webCameraTex.isPlay ) {
			for (int i = 0; i < planeObjArr.Length; i++) {
				planeObjArr[i].GetComponent<Renderer>().material.mainTexture = e_webCameraTex.cameraTexture;
			}
		}
	}
	
	public void NextBtnClicked()
	{
		if ((selectPageIndex + 1) * m_PrePageNum >= effectMatArr.Length) {
			return;
		}
		selectPageIndex ++;
		for (int i = 0; i < planeObjArr.Length && selectPageIndex < Mathf.Round((effectMatArr.Length)/m_PrePageNum); i++) {
			planeObjArr[i].GetComponent<Renderer>().material = effectMatArr[selectPageIndex*m_PrePageNum + i];
		}

		e_PageNoLabel.text = (selectPageIndex + 1).ToString ();
	}
	
	public void PreBtnClicked()
	{
		if (selectPageIndex < 1) {
			selectPageIndex = 0;
			return;
		}
		selectPageIndex --;
		for (int i = 0; i < planeObjArr.Length; i++) {
			planeObjArr[i].GetComponent<Renderer>().material = effectMatArr[selectPageIndex*m_PrePageNum + i];
		}
	
		e_PageNoLabel.text = (selectPageIndex + 1).ToString ();

	}


	public void showMediaItem()
	{
		hideMediaItem ();
		m_PageModePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
		e_mediaMainPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
		m_PageMode = PAGEMODE.FLITER_MENU;
	}
	
	private void hideMediaItem()
	{
		for (int i = 0; i!=planeObjArr.Length; i++) {
			planeObjArr[i].GetComponent<ItemPlaneController>().hideItem();
		}
	}
	
	void showMainItem(PoivtMode mode)
	{
		for (int i = 0; i!=planeObjArr.Length; i++) {
			ItemPlaneController itc = planeObjArr[i].GetComponent<ItemPlaneController>();
			if(itc.e_ItemPoivtMode == mode)
			{
				itc.showItem();
				m_PageMode = PAGEMODE.FLITER_PREVIEW;
			}
		}
	}
	
	public static bool IsMouseOverUI
	{
		get
		{
			return IsPointerOverUIObject ();

			#if UNITY_EDITOR_WIN || UNITY_WEBPLAYER ||UNITY_EDITOR||UNITY_STANDALONE
			if(EventSystem.current.IsPointerOverGameObject())
			{
				return true;
			}
			#elif UNITY_ANDROID
			Touch touch = Input.touches[0]; 
			if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
			{
				return true;
			}
			#elif UNITY_IOS
			Touch touch = Input.touches[0]; 
			if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
			{
				return true;
			}
			#else
			Touch touch = Input.touches[0]; 
			if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
			{
			return true;
			}
			#endif


		
			return false;

		}
	}

	private static bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}




	public void setMainPlayBtnClicked()
	{
		if (m_isReco) {


		} else {
			setCameraFreeze();
		}
	}

	public void setCameraFreeze()
	{
		if (m_PageMode == PAGEMODE.FLITER_PREVIEW) {
			PhotoController.ScreenCaptureFinished += getTextureFromCaptrue;
		}
		e_photoController.startRenderCameraForEdit ();
		if(e_webCameraTex != null)
		{
			e_webCameraTex.setCameraFreeze();
		}
	
		if (e_decoratePanlObjUGUI != null) {
			e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
		}
		if (e_mediaMainPanlObjUGUI != null) {
			e_mediaMainPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
		}
		
		//open the left top back btn;
		if (e_BackBtnObjUGUI != null) {
			e_BackBtnObjUGUI.GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.6f);
		}
		//open the left top share btn;
		if (e_ShareBtnObjUGUI != null) {
			e_ShareBtnObjUGUI.GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.6f);
		}
		if (e_ReverseCameraBtnObjUGUI != null) {
			e_ReverseCameraBtnObjUGUI.GetComponent<RectTransform>().DOScale(new Vector3(0,0,0),0.4f);
		}
		m_PageMode = PAGEMODE.DECORATE_MENU;
	}

	public void setCameraActive()
	{
		if (e_webCameraTex != null) {
			e_webCameraTex.setCameraActive();
		}
	}

	public void mediaClassToggleChange(bool vlaue)
	{
		bool isChecked ;
		isChecked =  toggleobj.isOn;
		m_isReco = !isChecked;
		if (isChecked) {
			e_PlayPhotoItemBtnUGUI.GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.6f);
			e_PlayVideoItemBtnUGUI.GetComponent<RectTransform>().DOScale(new Vector3(0,0,0),0.6f);

			e_ReverseCameraBtnObjUGUI.GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.6f);

		} else {
			e_PlayPhotoItemBtnUGUI.GetComponent<RectTransform>().DOScale(new Vector3(0,0,0),0.6f);
			e_PlayVideoItemBtnUGUI.GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.6f);

			e_ReverseCameraBtnObjUGUI.GetComponent<RectTransform>().DOScale(new Vector3(0,0,0),0.6f);
		}
	}

	public void StickerBtnClicked()
	{
		if (e_decoratePanlObjUGUI != null) {
			e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
		}

		if (e_stikcerScrowViewPanlObjUGUI != null) {
			e_stikcerScrowViewPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
		}

		if (m_stickerController != null) {
			m_stickerController.setWork(true);
		}
		m_PageMode = PAGEMODE.DECORATE_STICKER;
	}

	public void FrameBtnClicked()
	{
		if (e_decoratePanlObjUGUI != null) {
			e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
		}
		
		if (e_frameScrowViewPanlObjUGUI != null) {
			e_frameScrowViewPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
		}
		m_PageMode = PAGEMODE.DECORATE_FRAME;
	}

	public void TextBtnClicked()
	{
		if (e_decoratePanlObjUGUI != null) {
			e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
		}
		
		if (e_textPanlObjUGUI != null) {
			e_textPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
		}
		if (m_textController != null) {
			m_textController.setWork(true);
		}
		m_PageMode = PAGEMODE.DECORATE_TEXT;
	}

	public void BackBtnClicked()
	{
		switch (m_PageMode) {
		case PAGEMODE.DECORATE_FRAME:
		{
			if(m_frameController.isNeedApply)
			{
				if(e_tipWindowCtr != null)
				{
					e_tipWindowCtr.Show();
				}
			}
			else
			{
				if (e_decoratePanlObjUGUI != null) {
					e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
				}
				
				if (e_frameScrowViewPanlObjUGUI != null) {
					e_frameScrowViewPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
				}
				m_frameController.ClearAll();
				m_PageMode = PAGEMODE.DECORATE_MENU;
			}

		}
		break;
		case PAGEMODE.DECORATE_STICKER:
		{
			if(m_stickerController.isNeedApply)
			{
				if(e_tipWindowCtr != null)
				{
					e_tipWindowCtr.Show();
				}
			}
			else
			{
				if (e_decoratePanlObjUGUI != null) {
					e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
				}
				if (e_stikcerScrowViewPanlObjUGUI != null) {
					e_stikcerScrowViewPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
				}
				if (m_stickerController != null) {
					m_stickerController.setWork(false);
				}
				m_PageMode = PAGEMODE.DECORATE_MENU;
			}
		}
		break;
		case PAGEMODE.DECORATE_MENU:
		{

			if (e_decoratePanlObjUGUI != null) {
				e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
			}
			if (e_mediaMainPanlObjUGUI != null) {
				e_mediaMainPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
			}
			setCameraActive();
			if(e_PhotoMaskTex != null)
			{
				e_PhotoMaskTex.SetActive(false);
			}
			if (e_BackBtnObjUGUI != null) {
				e_BackBtnObjUGUI.GetComponent<RectTransform>().DOScale(new Vector3(0,0,0),0.6f);
			}
			//open the left top share btn;
			if (e_ShareBtnObjUGUI != null) {
				e_ShareBtnObjUGUI.GetComponent<RectTransform>().DOScale(new Vector3(0,0,0),0.5f);
			}
			if (e_ReverseCameraBtnObjUGUI != null) {
				e_ReverseCameraBtnObjUGUI.GetComponent<RectTransform>().DOScale(new Vector3(1,1,1),0.6f);
			}

			if(m_stickerController != null)
			{
				m_stickerController.ClearAll();
			}
			if (m_textController != null) {
				m_textController.ClearAll();
			}
			if(m_frameController != null)
			{
				m_frameController.ClearAll();
			}
			m_PageMode = PAGEMODE.FLITER_PREVIEW;
		}
			break;
		case PAGEMODE.DECORATE_TEXT:
		{
			if(m_textController != null && m_textController.isNeedApply)
			{
				if(e_tipWindowCtr != null)
				{
					e_tipWindowCtr.Show();
				}
			}
			else
			{
				if (e_decoratePanlObjUGUI != null) {
					e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
				}
				if (e_textPanlObjUGUI != null) {
					e_textPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
				}
				if (m_textController != null) {
					m_textController.setWork(false);
				}
				m_PageMode = PAGEMODE.DECORATE_MENU;
			}
		}
			break;
		}
	}

	public void ApplyBtnClicked()
	{
		if (m_PageMode == PAGEMODE.DECORATE_STICKER) {

			if (e_decoratePanlObjUGUI != null) {
				e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
			}
			if (e_stikcerScrowViewPanlObjUGUI != null) {
				e_stikcerScrowViewPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
			}
			m_PageMode = PAGEMODE.DECORATE_MENU;
			if (e_tipWindowCtr != null) {
				e_tipWindowCtr.Hide ();
			}
			if (m_stickerController != null) {
				m_stickerController.setWork (false);
			}

		} else if (m_PageMode == PAGEMODE.DECORATE_FRAME) {
			if (e_decoratePanlObjUGUI != null) {
				e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
			}
			if (e_frameScrowViewPanlObjUGUI != null) {
				e_frameScrowViewPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
			}
			m_PageMode = PAGEMODE.DECORATE_MENU;
			if (e_tipWindowCtr != null) {
				e_tipWindowCtr.Hide ();
			}
		}
		else if (m_PageMode == PAGEMODE.DECORATE_TEXT) {
			if (e_decoratePanlObjUGUI != null) {
				e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
			}
			if (e_textPanlObjUGUI != null) {
				e_textPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
			}
			m_PageMode = PAGEMODE.DECORATE_MENU;
			if (e_tipWindowCtr != null) {
				e_tipWindowCtr.Hide ();
			}
			if (m_textController != null) {
				m_textController.setWork (false);
			}
		}
	}

	public void CancelBtnClicked()
	{
		if (m_PageMode == PAGEMODE.DECORATE_STICKER) {
			if(m_stickerController != null)
			{
				if (e_decoratePanlObjUGUI != null) {
					e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
				}
				if (e_stikcerScrowViewPanlObjUGUI != null) {
					e_stikcerScrowViewPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
				}
				if (m_stickerController != null) {
					m_stickerController.setWork (false);
				}
				m_PageMode = PAGEMODE.DECORATE_MENU;
			
				if(m_stickerController != null)
				{
					m_stickerController.ClearAll();
				}
				if (e_tipWindowCtr != null) {
					e_tipWindowCtr.Hide ();
				}
			}
		}

		if (m_PageMode == PAGEMODE.DECORATE_TEXT) {
			if(m_textController != null)
			{
				if (e_decoratePanlObjUGUI != null) {
					e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
				}
				if (e_textPanlObjUGUI != null) {
					e_textPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
				}
				if (m_textController != null) {
					m_textController.setWork (false);
				}
				m_PageMode = PAGEMODE.DECORATE_MENU;
				
				if(m_textController != null)
				{
					m_textController.ClearAll();
				}
				if (e_tipWindowCtr != null) {
					e_tipWindowCtr.Hide ();
				}
			}
		}

		if (m_PageMode == PAGEMODE.DECORATE_FRAME) {
			if (e_decoratePanlObjUGUI != null) {
				e_decoratePanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,0,0),0.6f);
			}
			if (e_frameScrowViewPanlObjUGUI != null) {
				e_frameScrowViewPanlObjUGUI.GetComponent<RectTransform> ().DOLocalMove (new Vector3(0,-150,0),0.6f);
			}

			m_PageMode = PAGEMODE.DECORATE_MENU;
			
			if(m_frameController != null)
			{
				m_frameController.ClearAll();
			}

			if (e_tipWindowCtr != null) {
				e_tipWindowCtr.Hide ();
			}
		}
	}

	void getTextureFromCaptrue(Texture2D tex)
	{
        if (mPhotoTex == null)
        {
            mPhotoTex = new Texture2D((int)tex.width, (int)tex.height, TextureFormat.RGB24, false);
        }

        if (mPhotoTex != null) {
			mPhotoTex.SetPixels (tex.GetPixels ());
			mPhotoTex.Apply ();
		}

		if (e_PhotoMaskTex != null) {

			e_PhotoMaskTex.GetComponent<Renderer>().material.mainTexture = mPhotoTex;
			e_PhotoMaskTex.SetActive(true);
		}
		PhotoController.ScreenCaptureFinished -= getTextureFromCaptrue;
	}

	public void changeCameraMode(CameraMode mode)
	{
		for (int i = 0; i!=planeObjArr.Length; i++) {
			planeObjArr[i].GetComponent<ItemPlaneController>().changeTranformByCameraMode(mode);
		}
	}
}
