//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FlashColorEffect : MonoBehaviour
//{
//    public Renderer goRenderer;
//    public Color flashColor = new Color(20, 20, 20);
//    public float flashDuration = 2f;

//    private Color originalColor;

//    void Start()
//    {
//        if(goRenderer == null) goRenderer = GetComponent<Renderer>();
//        originalColor = goRenderer.material.color;
//    }
    
//    private IEnumerator DoFlash()
//    {
//        goRenderer.material.color = flashColor;
//        yield return new WaitForSeconds(flashDuration);
//        goRenderer.material.color = originalColor;
//    }

//    public void Flash()
//    {
//        StopAllCoroutines();
//        StartCoroutine(DoFlash());
//    }
//}
