using UnityEngine;
using System.Collections;

public class KillSphere : MonoBehaviour {

	GameObject _parent; 

	void OnTriggerEnter(Collider _coll){
		if (_coll.gameObject.layer == 8) {
			Monster.t.NowNearMe(_parent); 
		}
	}

	void OnTriggerExit(Collider _coll){
		if (_coll.gameObject.layer == 8) {
			Monster.t.NoLongerNearMe(_parent); 
		}
	}

	// Use this for initialization
	void Start () {
		_parent = GetComponentInParent<Human> ().gameObject; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
