using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconsManager : MonoBehaviour
{
    public GameObject[] hearts;
    public GameObject[] shells;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetIcons(GameObject[] iconGroup, int n)
    {
        for (int i = 0; i < iconGroup.Length; i++)
        {
            iconGroup[i].SetActive(i < n);
        }
    }

    public void SetIcons(int heartsCount, int shellsCount)
    {
        SetIcons(hearts, heartsCount);
        SetIcons(shells, shellsCount);
    }
}
