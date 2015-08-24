using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Monster : MonoBehaviour {

	[SerializeField]
	float _speed; 
	float _forward;
	float _right; 
	Rigidbody _rigid; 
	public static Transform trans; 
	public static Monster t; 
	bool _isStone = false; 
	public bool IsStone { get { return _isStone; } set { _isStone = value; } }

	
	List<GameObject> _seesMe = new List<GameObject>(); 
	List<GameObject> _nearMe = new List<GameObject>(); 
	[SerializeField]
	GameObject _effect; 
	MeshRenderer[] _renderers; 

	void GetInput(){
		if (!_isStone) {
			_forward = Input.GetAxisRaw ("Vertical");
			_right = Input.GetAxisRaw ("Horizontal"); 
			if(Input.GetKeyDown(KeyCode.Space)){
				Kill(); 
			}
		}
	}
	void MoveMonster(){
		if (!_isStone) {
			Vector3 _move = transform.right * _right;
			_move += transform.forward * _forward; 
			_rigid.velocity = _move.normalized * _speed; 
		} else {
			_rigid.velocity = Vector3.zero; 
		}
	}

	public void NowSeenBy(GameObject _go){
		if (!_seesMe.Contains (_go)) {
			_seesMe.Add(_go); 
			UpdateSeenState(); 
		}
	}
	public void NoLongerSeenBy(GameObject _go){
		if (_seesMe.Contains (_go)) {
			_seesMe.Remove(_go); 
			UpdateSeenState(); 
		}
	}
	public void NowNearMe(GameObject _go){
		if (!_nearMe.Contains (_go)) {
			_nearMe.Add(_go);
			_go.GetComponent<Human>().NearMonster(); 
		}
	}
	public void NoLongerNearMe(GameObject _go){
		if (_nearMe.Contains (_go)) {
			_nearMe.Remove(_go);
			_go.GetComponent<Human>().NoLongerNearMonster(); 
		}
	}
	void Kill(){
		if (_nearMe.Count > 0) {
			GameObject _go = _nearMe[0];
			_nearMe.Remove(_go); 
			_go.GetComponent<Human>().Murdered(); 
			Invoke ("Win", 1); 
		}

	}
	void Win(){
		World.t.Killed (); 
	}
	void UpdateSeenState(){
		if (_seesMe.Count > 0) {
			World.t.StartStoneWorld(); 
		} else {
			World.t.StartShadowWorld(); 
		}
	}

	public void NowStone(){
		_isStone = true; 
		_effect.SetActive (false); 
		for (int i = 0; i < _nearMe.Count; i++) {
			_nearMe[i].GetComponent<Human>().NoLongerNearMonster(); 
		}
		for (int i = 0; i < _renderers.Length; i++) {
			_renderers[i].enabled = true; 
		}
	}
	public void NowShadow(){
		_isStone = false; 
		_effect.SetActive (true); 
		for (int i = 0; i < _nearMe.Count; i++) {
			_nearMe[i].GetComponent<Human>().NearMonster(); 
		}
		for (int i = 0; i < _renderers.Length; i++) {
			_renderers[i].enabled = false; 
		}
	}

	void Awake(){
		_rigid = GetComponent<Rigidbody> (); 
		trans = this.transform; 
		t = this; 
		_renderers = GetComponentsInChildren<MeshRenderer> (); 
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		GetInput ();
	}

	void FixedUpdate(){
		MoveMonster (); 
	}
}
