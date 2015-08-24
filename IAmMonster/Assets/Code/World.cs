using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI; 

public class World : MonoBehaviour {

	public static World t; 
	List<Light> _nonShadowLights = new List<Light>(); 
	Light _monsterLight; 
	Human[] _allHumans; 
	public Object _killEffect; 
	[SerializeField]
	AudioSource _shadowSong; 
	float _shadowVolume = 0; 
	[SerializeField]
	Slider _slider; 
	[SerializeField]
	float _timeToComplete;
	float _totalTime;
	Canvas _canvas; 
	[SerializeField]
	Object _touchedGameOver;
	[SerializeField]
	Object _starvedGameOver;


	bool CheckForSurvivors(){
		for(int  i = 0; i < _allHumans.Length; i++){
			if(_allHumans[i] !=null){
				return true;
			}
		}
		return false; 
	}
	public void Killed(){
		if (!CheckForSurvivors ()) {
			Application.LoadLevel(Application.loadedLevel +1); 
		}
	}
	public void StartStoneWorld(){
		Monster.t.NowStone ();
		for(int i = 0; i< _nonShadowLights.Count; i++){
			_nonShadowLights[i].enabled = true; 
		}
		for (int i = 0; i < _allHumans.Length; i++) {
			if(_allHumans[i]){
				_allHumans[i].StartStoneWorld();
			}
		}
		_monsterLight.enabled = false;
		_shadowVolume = 0; 
	}
	public void StartShadowWorld(){
		_shadowVolume = 1; 
		Monster.t.NowShadow (); 
		for(int i = 0; i< _nonShadowLights.Count; i++){
			_nonShadowLights[i].enabled = false; 
		}
		for (int i = 0; i < _allHumans.Length; i++) {
			if(_allHumans[i] != null){
				_allHumans[i].StartShadowWorld();
			}
		}
		_monsterLight.enabled = true; 
	}

	void CollectLights(){
		Light[] _allLights = Object.FindObjectsOfType<Light> (); 
		for (var i = 0; i < _allLights.Length; i++) {
			if(_allLights[i].gameObject.tag != "Monster"){
				_nonShadowLights.Add(_allLights[i]); 
			}
			else {
				_monsterLight = _allLights[i]; 
			}
		}
	}
	void CollectHumans(){
		_allHumans = Object.FindObjectsOfType<Human> (); 
	}
	void LerpMusic(){
		_shadowSong.volume = Mathf.Lerp (_shadowSong.volume, _shadowVolume, Time.deltaTime); 
	}
	void Timer(){
		if (!Monster.t.IsStone) {
			_totalTime += Time.deltaTime;
			_slider.value = 1 - _totalTime / _timeToComplete; 
			if (_totalTime > _timeToComplete) {
				GameObject _go = Instantiate (_starvedGameOver) as GameObject;
				_go.transform.SetParent (_canvas.transform, false); 
				Invoke ("LoseLevel", 2);  
			}
		}
	}
	public void GotTouched(){
		GameObject _go =Instantiate (_touchedGameOver) as GameObject;
		_go.transform.SetParent (_canvas.transform, false); 
		Invoke("LoseLevel",2);  
	}
	void LoseLevel(){
		Application.LoadLevel(Application.loadedLevel); 
	}

	void Awake(){
		t = this; 
	}

	// Use this for initialization
	void Start () {
		_canvas = FindObjectOfType<Canvas> (); 
		_shadowSong = GetComponent<AudioSource> (); 
		CollectLights (); 
		CollectHumans (); 
		StartStoneWorld (); 
	}
	
	// Update is called once per frame
	void Update () {
		LerpMusic (); 
		Timer (); 
	}
}
