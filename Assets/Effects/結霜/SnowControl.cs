using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowControl : MonoBehaviour
{

    public float SnowAmount = 1;
    public float coveringSpeed = 1;
    public bool IsSnowing = false;
    public ParticleSystem SnowParticles;
    public float snowRate;
    public float rateIncreaseSpeed;

    private Material mat;


    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            IsSnowing = true;
        if (Input.GetKeyDown(KeyCode.D))
            IsSnowing = false;
        if (IsSnowing)
            StartSnow(coveringSpeed);
        else
            StopSnow(coveringSpeed);
        mat.SetFloat("_SnowAmount", SnowAmount);
        var snowEmission = SnowParticles.emission;
        snowEmission.rateOverTime = snowRate;
    }

    public void StartSnow(float speed)
    {
        if (SnowAmount > 0.35)
            SnowAmount -= Time.deltaTime * speed;
        if (snowRate < 500)
            snowRate += Time.deltaTime * rateIncreaseSpeed;
    }

    public void StopSnow(float speed)
    {
        if (SnowAmount < 1)
            SnowAmount += Time.deltaTime * speed;
        if (snowRate > 0)
            snowRate -= Time.deltaTime * rateIncreaseSpeed;
    }
}
