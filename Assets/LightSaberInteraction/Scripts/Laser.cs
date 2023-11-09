using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField] GameObject laser;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip ignitionSound;
    [SerializeField] AudioClip hummingSound;
    [SerializeField] AudioClip offSound; 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("uPDATE is running");
    }

    public void StartLaser()
    {
        laser.SetActive(true);
        audioSource.PlayOneShot(ignitionSound);
        audioSource.PlayOneShot(hummingSound);

    }

    public void Stoplaser()
    {
        laser.SetActive(false);
        audioSource.Stop();
        audioSource.PlayOneShot(offSound);
   
    }
}
