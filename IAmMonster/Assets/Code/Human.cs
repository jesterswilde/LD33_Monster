using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Human : MonoBehaviour {

	[SerializeField]
	float _viewAngle; 
	[SerializeField]
	float _spotDistance;
	[SerializeField]
	GameObject _heartEffect;
	[SerializeField]
	GameObject _nearEffect; 
	public Transform[] waypoints;
	public float moveSpeed;
	public float rotateSpeed;
	[SerializeField]
	string[] _behaviourString;
	public
	float[] behaviorVariable; 
	List<HumanBehavior> _behavior = new List<HumanBehavior>(); 
	Animator _anim; 

	bool _seesMonster = false; 
	int _behaviorI = 0; 
	public int CurBehav { get { return _behaviorI; } }

	void SpotMonster(){
		Vector3 _toMon = Monster.trans.position - transform.position; 
		float _angle = Vector3.Angle (transform.forward, _toMon); 
		if (_toMon.sqrMagnitude < _spotDistance * _spotDistance && _angle < _viewAngle) {//spotted monster
			if(!_seesMonster){
				Ray _ray = new Ray(transform.position, Monster.trans.position - transform.position);
				RaycastHit _hit; 
				if(Physics.Raycast(_ray, out _hit)){
					if(_hit.collider.gameObject.layer == 8){
						_seesMonster = true; 
						Monster.t.NowSeenBy(this.gameObject); 
					}
				}
			}
		} else { //not seeing monster
			if(_seesMonster){
				_seesMonster = false;
				Monster.t.NoLongerSeenBy(this.gameObject); 
			}
		}
	}
	public void NearMonster(){
		_nearEffect.SetActive (true); 
	}
	public void NoLongerNearMonster(){
		_nearEffect.SetActive (false); 
	}
	public void Murdered(){
		Instantiate (World.t._killEffect, transform.position, Quaternion.identity); 
		Destroy (gameObject); 
	}

	public void StartStoneWorld(){
		_heartEffect.SetActive (false); 
	}
	public void StartShadowWorld(){
		_heartEffect.SetActive (true); 
	}
	public void CollideWithMonster(){
		if (Monster.t.IsStone) {
			
		}
	}

	public void NextBehavior(){
		if (_behaviorI < _behavior.Count - 1) {
			_behaviorI++; 
		} else {
			_behaviorI = 0; 
		}
		_behavior [_behaviorI].StartBehaviour (this, transform); 
	}
	void DoBehavior(){
		_behavior [_behaviorI].OnUpdate (); 
	}
	void FillBehaviorArray(){
		for(int i = 0 ; i < _behaviourString.Length; i++){
			_behavior.Add(StringToBehavior(_behaviourString[i])); 
		}
	}
	public void IsRunning(bool _bool){
		_anim.SetBool ("running", _bool); 
	}
	HumanBehavior StringToBehavior(string _string){

		switch (_string) {
		case "forwards":
			return new HBFollowForwards();
		case "backwards":
			return new HBFollowBackwards(); 
		case "wait":
			return new HBWait();
		case "turn":
			return new HBTurn(); 
		}
		return new HumanBehavior (); 
	}

	// Use this for initialization
	void Start () {
		_anim = GetComponentInChildren<Animator> ();
		FillBehaviorArray (); 
		_behavior [_behaviorI].StartBehaviour (this, transform);
		_nearEffect.SetActive (false); 
	}
	
	// Update is called once per frame
	void Update () {
		SpotMonster (); 
		DoBehavior (); 
	}
}

public class HumanBehavior{
	protected Human _human; 
	protected Transform _humanTrans; 
	public virtual void StartBehaviour(Human _theHuman, Transform _theTrans){
		_human = _theHuman;
		_humanTrans = _theTrans; 

	}
	public virtual void OnUpdate(){
	
	}
	public virtual void EndBehaviour(){
		_human.NextBehavior (); 		
	}
}

public class HBFollowForwards : HumanBehavior{
	int _i = 0; 
	public override void StartBehaviour (Human _theHuman, Transform _theTrans)
	{
		base.StartBehaviour (_theHuman, _theTrans);
		_i = 0; 
		_humanTrans.position = _human.waypoints [_i].position; 
		_i++; 
		_human.IsRunning (true); 
	}
	public override void OnUpdate ()
	{
		base.OnUpdate ();
		Vector3 _dir = _human.waypoints[_i].position - _humanTrans.position; //get dir to next spot
		float _mag = _dir.magnitude; 
		_humanTrans.forward = _dir; 
		if (_mag > _human.moveSpeed * Time.deltaTime) { //if you won't reach your destinatio this update
			_humanTrans.position += _dir / _mag * _human.moveSpeed * Time.deltaTime; 
		} else {
			_humanTrans.position = _human.waypoints[_i].position; //if you would, just move to your destination instead 
			_i++ ; //start going to next waypoint
			if(_i == _human.waypoints.Length){ //unless your out of waypoints
				EndBehaviour(); 
			}
		}
	}

}

public class HBFollowBackwards : HumanBehavior{
	int _i = 0; 
	public override void StartBehaviour (Human _theHuman, Transform _theTrans)
	{
		base.StartBehaviour (_theHuman, _theTrans);
		_i = _human.waypoints.Length -1; 
		_humanTrans.position = _human.waypoints [_i].position; 
		_i--; 
		_human.IsRunning (true); 
	}
	public override void OnUpdate ()
	{
		base.OnUpdate ();
		Vector3 _dir = _human.waypoints [_i].position - _humanTrans.position; //get dir to next spot
		float _mag = _dir.magnitude; 
		_humanTrans.forward = _dir; 
		if (_mag > _human.moveSpeed * Time.deltaTime) { //if you won't reach your destinatio this update
			_humanTrans.position += _dir / _mag * _human.moveSpeed * Time.deltaTime; 
		} else {
			_humanTrans.position = _human.waypoints[_i].position; //if you would, just move to your destination instead 
			_i-- ; //start going to next waypoint
			if(_i == -1){ //unless your out of waypoints
				EndBehaviour(); 
			}
		}
	}
}
public class HBWait : HumanBehavior{
	float _counter = 0; 
	float _wait = 0; 
	public override void StartBehaviour (Human _theHuman, Transform _theTrans)
	{ 
		_counter = 0; 
		base.StartBehaviour (_theHuman, _theTrans);
		if (_human.behaviorVariable [_theHuman.CurBehav] != null) {
			_wait = _human.behaviorVariable [_theHuman.CurBehav]; 
		}
		_human.IsRunning (false); 
	}
	public override void OnUpdate ()
	{
		base.OnUpdate ();
		_counter += Time.deltaTime; 
		if (_counter > _wait) {
			EndBehaviour(); 
		}
	}
}

public class HBTurn: HumanBehavior {
	float _target; 
	float _dir; 
	public override void StartBehaviour (Human _theHuman, Transform _theTrans)
	{
		base.StartBehaviour (_theHuman, _theTrans);
		float _mod = _human.behaviorVariable [_theHuman.CurBehav];
		_target =  _humanTrans.rotation.eulerAngles.y +  _mod; 
		_dir = _mod / Mathf.Abs (_mod); 
		_human.IsRunning (false); 
	}
	public override void OnUpdate ()
	{
		base.OnUpdate ();
		//Debug.Log(Mathf.Abs (_humanTrans.rotation.eulerAngles.y - _target) % 360  + " | " + _human.rotateSpeed); 
		if (Mathf.Abs (_humanTrans.rotation.eulerAngles.y - _target) % 360 > _human.rotateSpeed) {
			_humanTrans.Rotate (new Vector3 (0, _human.rotateSpeed * _dir, 0)); 
		} else {
			_humanTrans.rotation = Quaternion.Euler(new Vector3( _humanTrans.rotation.x, _target, _humanTrans.rotation.z)); 
			EndBehaviour(); 
		}

	}
}

