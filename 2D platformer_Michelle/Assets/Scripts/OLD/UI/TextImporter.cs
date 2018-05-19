using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextImporter : MonoBehaviour {
    public TextAsset textFile;
    public string[] textlines;

	// Use this for initialization
	void Start () {
		if(textFile != null)
        {
            textlines = (textFile.text.Split('\n'));
        }
	}
	
}
