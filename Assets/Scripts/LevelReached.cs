using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelReached : MonoBehaviour
{
    private Text text;
    private int level;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
        level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayLevelReached()
    {
        text.text = "You reached Level " + level + "!";
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }
}
