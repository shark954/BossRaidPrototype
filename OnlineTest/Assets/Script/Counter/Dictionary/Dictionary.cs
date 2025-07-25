using UnityEngine;
using System.Collections.Generic;

public class Dictionary : MonoBehaviour
{

    public Dictionary<string, int> dic = new Dictionary<string, int>();

    public int value;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dic.Add("Banana", 50);
        dic.Add("Apple", 100);
        dic.Remove("Banana");
       
    }

    // Update is called once per frame
    void Update()
    {
        dic.TryGetValue("Banana", out value);
        Debug.Log(value);
    }


}
