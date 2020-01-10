using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller_Dummy : MonoBehaviour
{

    public uint player_level = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePlayerLevel(float new_level)
    {
        player_level = (uint)new_level;
    }
}
