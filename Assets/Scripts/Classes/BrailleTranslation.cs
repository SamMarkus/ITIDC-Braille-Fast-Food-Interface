using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Class that translates text to braille using BiDictionary
public class BrailleTranslator : MonoBehaviour
{
    [Header("BiDictionary Settings")]
    private BiDictionary<char, string> braille = new();

    [Header("Haptics Device Settings")]
    public HapticPlugin hapticPlugin;

    [Header("Raised Dot Vibration")]
    public Vector3 raisedGDirection;
    public float raisedGMag = 1.0f;
    public int raisedFrequency = 100;
    public float raisedTime = 10.0f;

    [Header("Lowered Dot Vibration")]
    public Vector3 loweredGDirection;
    public float loweredGMag = 1.0f;
    public int loweredFrequency = 10;
    public float loweredTime = 10.0f;

    [Header("Delay Times")]
    public float delayTime = 1.0f;

    private bool _readStatus;
    private IEnumerator coroutine;

    // Constructor
    public BrailleTranslator()
    {
        braille.Add('a', ".-----"); // Dots 1
        braille.Add('b', "..----"); // Dots 1, 2
        braille.Add('c', ".--.--"); // Dots 1, 4
        braille.Add('d', ".--..-");
        braille.Add('e', ".---.-");
        braille.Add('f', "..-.--");
        braille.Add('g', "..-..-");
        braille.Add('h', "..--.-");
        braille.Add('i', "-.-.--");
        braille.Add('j', "-.-..-"); 
        braille.Add('k', ".-.---"); 
        braille.Add('l', "...---"); 
        braille.Add('m', ".-..--");
        braille.Add('n', ".-...-");
        braille.Add('o', ".-.-.-");
        braille.Add('p', "....--");
        braille.Add('q', ".....-");
        braille.Add('r', "...-.-");
        braille.Add('s', "-...--");
        braille.Add('t', "-....-");
        braille.Add('u', ".-.--.");
        braille.Add('v', "...--.");
        braille.Add('w', "-.-...");
        braille.Add('x', ".-..-.");
        braille.Add('y', ".-....");
        braille.Add('z', ".-.-..");
    }


    // Function that handles translation from character to braille 
    public string CharTranslate(char c)
    {
        if (braille.ContainsKey(c))
            return braille[c];
        else
            return "------";
    }

    // Function that handles translation from braille to character
    public char BrailleSingleTranslate(string b)
    {
        if (braille.ContainsValue(b))
            return braille[b];
        else
            return ' ';
    }

    public string StringTranslate(string text)
    {
        string output = "";
        string str = text.ToLower();
        for (int i = 0; i < text.Length; i++)
        {
            output += CharTranslate(str[i]);

            // If index is not the last, add a space.
            if (i != text.Length - 1)
            {
                output += " ";
            }
        }
        return output;
    }

    public string BrailleTranslate(string braille)
    {
        string output = "";
        foreach (string character in braille.Split(' '))
        {
            output += BrailleSingleTranslate(character);
        }

        return output;
    }

    private void HapticSingleTranslate(char c)
    {
        if (c == '.')
        {
            hapticPlugin.ActivateVibration(loweredGDirection, loweredGMag, loweredFrequency, loweredTime);
        }
        else if (c == '-')
        {
            hapticPlugin.ActivateVibration(raisedGDirection, raisedGMag, raisedFrequency, raisedTime);
        }
    }

    public IEnumerator HapticCoroutine(string braille)
    {
        foreach (char c in braille)
        {
            if (c != ' ')
            {
                if (!TestManager.instance.GetTestStatus())
                    UnityEngine.Debug.Log(c);
                HapticSingleTranslate(c);
            }
            yield return new WaitForSeconds(delayTime);
        }
        coroutine = null;
    }

    public void HapticTranslation(string braille)
    {
        if (coroutine == null)
        {
            coroutine = HapticCoroutine(braille);
            _readStatus = true;
            StartCoroutine(coroutine);
        }
        else
        {
            if (!TestManager.instance.GetTestStatus())
                UnityEngine.Debug.Log("Coroutine deactivated");
            StopCoroutine(coroutine);
            coroutine = null;
            _readStatus = false;
        }
    }

    public bool GetReadStatus()
    {
        return _readStatus;
    }
}