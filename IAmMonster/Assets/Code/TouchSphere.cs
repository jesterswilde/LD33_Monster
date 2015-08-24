using UnityEngine;
using System.Collections;

public class TouchSphere : MonoBehaviour {
	bool _lost = false; 

	void OnTriggerStay(Collider _coll){
		if (_coll.gameObject.layer == 8 && !_lost ) {
			if(_coll.gameObject.GetComponent<Monster>().IsStone){
				_lost = true;
				World.t.GotTouched(); 
				Debug.Log ("touched"); 
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
