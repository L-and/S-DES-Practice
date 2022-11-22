    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFieldManager : MonoBehaviour
{
    public static TextFieldManager Instance = null; // �̱���

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public TMP_InputField plainTextInputField; // �� �Է�â
    public TMP_InputField cipherTextInputField; // ��ȣ�� �Է�â
    public TMP_InputField plainTextField; // �� ���â
    public TMP_InputField cipherTextField; // ��ȣ�� ���â

    public static string GetText()
    {
        return Instance.plainTextInputField.text;
    }

    public static void SetPlainTextField(string text)
    {
        Instance.plainTextField.text = text;
    }

    public static void SetCipherTextField(string text)
    {
        Instance.cipherTextField.text = text;
    }
}
