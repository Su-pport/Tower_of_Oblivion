using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    TextMeshPro _npcText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _npcText = transform.Find("Text").GetComponent<TextMeshPro>();
        _npcText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hello()
    {
        StartCoroutine(TalkCoroutine());
    }

    IEnumerator TalkCoroutine()
    {
        _npcText.enabled = true;
        yield return new WaitForSeconds(3f);
        _npcText.enabled = false;
    }
}
