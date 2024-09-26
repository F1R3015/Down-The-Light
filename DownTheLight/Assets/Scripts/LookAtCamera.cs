using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private GameObject _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_camera.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); // NOTE : Depending if its enemy or ally, the sprite will be flipped or not
    }
}
