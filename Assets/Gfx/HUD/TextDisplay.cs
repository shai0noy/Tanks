using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour {

    private Text textField;
    private object value;
    private string format;

	void Awake () {
        textField = GetComponent<Text>();
        format = textField.text;
	}

    public void set(params object[] args) {
        textField.text = string.Format(format, args);
    }

}
