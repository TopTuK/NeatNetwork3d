using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FPSInput : MonoBehaviour
{
	private Camera _camera;
	[SerializeField] private float sensitivity = 1.1f;
	[SerializeField] private Text positionText = null;
	[SerializeField] private Transform Spawner = null;
	
	private ObjectManager _objManager = null;

	void Start()
	{
		_camera = GetComponent<Camera>();
		
		_objManager = Spawner.GetComponent<ObjectManager>();
	}
	
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
		{
			transform.Translate(Vector3.forward * Time.deltaTime * sensitivity);
		}
		else if(Input.GetKey(KeyCode.S))
		{
			transform.Translate(Vector3.back * Time.deltaTime * sensitivity);
		}
		else if(Input.GetKey(KeyCode.A))
		{
			transform.Translate(Vector3.left * Time.deltaTime * sensitivity);
		}
		else if(Input.GetKey(KeyCode.D))
		{
			transform.Translate(Vector3.right * Time.deltaTime * sensitivity);
		}
		else if(Input.GetKey(KeyCode.R))
		{
			_objManager.ResetDetails();
		}
		else if(Input.GetKey(KeyCode.X))
		{
			_objManager.SelectAll();
		}
		else if(Input.GetKey(KeyCode.B))
		{
			_objManager.SelectBias();
		}
		else if(Input.GetKey(KeyCode.H))
		{
			_objManager.SelectHidden();
		}
		else if(Input.GetKey(KeyCode.I))
		{
			_objManager.SelectInput();
		}
		else if(Input.GetKey("escape"))
		{
			SceneManager.LoadScene("MainMenu");
		}
		else if(Input.GetMouseButtonDown(0))
		{
			Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
			
			Ray hitRay = _camera.ScreenPointToRay(point);
			RaycastHit hitObject;
			if(Physics.Raycast(hitRay, out hitObject))
			{
				Debug.Log("Hit" + hitObject.point);
				_objManager.SelectObject(hitObject.transform);
			}
			else
			{
				_objManager.ResetDetails();
			}
		}
		
		positionText.text = string.Format("P: {0} {1} {2}", 
			transform.position.x.ToString("0.00"), 
			transform.position.y.ToString("0.00"), 
			transform.position.z.ToString("0.00"));
    }
}
