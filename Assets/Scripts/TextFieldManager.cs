    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFieldManager : MonoBehaviour
{
    public static TextFieldManager Instance = null; // 싱글톤

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
    public TMP_InputField plainTextInputField; // 평문 입력창
    public TMP_InputField cipherTextInputField; // 암호문 입력창
    public TMP_InputField plainTextField; // 평문 출력창
    public TMP_InputField cipherTextField; // 암호문 출력창

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
