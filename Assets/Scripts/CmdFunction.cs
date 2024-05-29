using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CmdFunction : MonoBehaviour
{
    public GameObject[] cmdPlane;
    [Tooltip("Input the cmd alphaCode related to the cmdPlane above")]
    public string[] cmdKey;
    private Dictionary<string, List<GameObject>> planeDictionary = new Dictionary<string, List<GameObject>>();
    public float rangeX = 3.0f;
    public float rangeY = 3.0f;
    public float startingZ = 3.0f;
    private Quaternion generateRotation;
    public float generateInterval = 1.5f;
    private float timer = 0f;
    public float moveSpeed = 1.0f; 
    private int cmdPlaneCount = 0;
    public Material hitMaterial;
    public GameObject particlesystem;
    public GameObject[] chord;
    void Start()
    {
        cmdPlaneCount = cmdPlane.Length;
        generateRotation = Quaternion.Euler(90, -180, 0);

        foreach (string key in cmdKey)
        {
            planeDictionary.Add(key, new List<GameObject>());
        }
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= generateInterval)
        {
            timer = 0f;

            int randomIndex = UnityEngine.Random.Range(0, cmdPlaneCount); 

            Vector3 position = new Vector3(UnityEngine.Random.Range(-rangeX, rangeX), UnityEngine.Random.Range(-rangeY, rangeY), startingZ);

            GameObject objClone = Instantiate(cmdPlane[randomIndex], position, generateRotation);
            planeDictionary[cmdKey[randomIndex]].Add(objClone);
        }
        foreach (KeyValuePair<string, List<GameObject>> entry in planeDictionary)
        {
            List<GameObject> planeList = entry.Value;

            foreach (GameObject obj in planeList.ToArray())
            {
                if(obj != null){
                    obj.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

                    if (obj.transform.position.z <= 0)
                    {
                        planeList.Remove(obj);
                        Destroy(obj);
                    }
                }

            }
        }
    }

    public void CreateSphere(){

    }
    public void SayHi(string cmd){
        Debug.Log("Hi");
    }
    public void LeftFootUp(string cmd){
        Debug.Log("Left Up");
    }
    public void RightFootUp(string cmd){
        Debug.Log("Right Up");
    }
    public void DeletePlane(string cmd){
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                // Renderer renderer = firstElement.GetComponent<Renderer>();

                // if (renderer != null)
                // {
                // // 将平面对象的材质设置为指定的材质
                //     Material originalMaterial = renderer.material;
                //     renderer.material = hitMaterial;

                // 延迟一定时间后恢复原始材质并销毁对象
                StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                // }
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    private IEnumerator DestroyDelayed(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);


        Destroy(obj); // 销毁对象
    }
    public void PlayAmChord(string cmd){
        GameObject cc = Instantiate(chord[0]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void PlayCChord(string cmd){
        GameObject cc = Instantiate(chord[1]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void PlayDChord(string cmd){
        GameObject cc = Instantiate(chord[2]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();        
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void PlayD7Chord(string cmd){
        GameObject cc = Instantiate(chord[3]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void PlayEmChord(string cmd){
        GameObject cc = Instantiate(chord[4]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void PlayGChord(string cmd){
        GameObject cc = Instantiate(chord[5]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void Play1Chord(string cmd){
        GameObject cc = Instantiate(chord[6]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void Play2Chord(string cmd){
        GameObject cc = Instantiate(chord[7]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void Play3Chord(string cmd){
        GameObject cc = Instantiate(chord[8]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void Play4Chord(string cmd){
        GameObject cc = Instantiate(chord[9]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void Play5Chord(string cmd){
        GameObject cc = Instantiate(chord[10]);
        AudioSource audioSource = cc.GetComponent<AudioSource>();
        audioSource.Play();
        if (planeDictionary.ContainsKey(cmd))
        {
            List<GameObject> planeList = planeDictionary[cmd];
            if (planeList.Count > 0)
            {
                GameObject firstElement = planeList[0];
                planeList.RemoveAt(0);
                //StartCoroutine(DestroyDelayed(firstElement, 4.0f));
                Instantiate(particlesystem, firstElement.transform.position, Quaternion.identity);
                Destroy(firstElement);   
            }
        }
    }
    public void otherSound(string cmd){
        if (cmd == "other")
        {
            GameObject cc = Instantiate(chord[11]);
            AudioSource audioSource = cc.GetComponent<AudioSource>();
            audioSource.Play();
        }
    }
}
