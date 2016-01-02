using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour {

    private Text textField;
    private object value;
    private string format;
    private Color defColor;
    public Color altColor = new Color(255, 200, 70);

	void Awake () {
        textField = GetComponent<Text>();
        format = textField.text;
        defColor = textField.color;
	}

    public void set(params object[] args) {
        textField.text = string.Format(format, args);
    }

    public void displayDefaultColor() {
        textField.color = defColor;
    }
    public void displayAltColor() {
        textField.color = altColor;
    }

    public void setDisplayColor(bool useAlt = false) {
        textField.color = useAlt ? altColor : defColor;
    }

}
