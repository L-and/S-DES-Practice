using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class KeyGenerator : MonoBehaviour
{
    [SerializeField] [Tooltip("Ű�� �Է��ʵ带 �־��ּ���.")]
    TMP_InputField keyInputField;

    int[] intKey = new int[1];

    int[] P10Map = { 3, 5, 2, 7, 4, 10, 1, 9, 8, 6 }; // P10�� �����ϱ����� ��Ģ
    int[] P8Map = { 6, 3, 7, 4, 8, 5, 10, 9 };

    BitArray P10 = new BitArray(10);
    BitArray key;

    public BitArray key1;
    public BitArray key2;

    private void StringKeyToBitArray() // StringŰ�� BitArray�� �ٲ���
    {
        intKey[0] = int.Parse(keyInputField.text); // �ؽ�Ʈ�ʵ尪�� Int�� ����

        key = new BitArray(intKey); // Int�� BitArray�� ����
    }

    private BitArray GetP10() // ��Ģ�� �°� ġȯ�Ͽ� P10�� �����ϴ� �Լ�
    {
        BitArray tmpKey = new BitArray(10);
        key.Length = 10;

        for(int i=0; i<P10.Length; i++)
        {
            tmpKey[i] = key[P10Map[i] - 1];
        }

        return tmpKey;
    }

    private BitArray GetP8(BitArray inputArray) // ��Ģ�� �°� ġȯ�Ͽ� P8�� �����ϴ� �Լ�
    {
        BitArray tmpKey = new BitArray(8);

        for (int i = 0; i < P8Map.Length; i++)
        {
            tmpKey[i] = inputArray[P8Map[i] - 1];
        }

        return tmpKey;
    }

    private string BitArrayToString(BitArray bitary)
    {
        String stringBitArray = "";

        for(int i= 0; i< bitary.Length; i++)
        {
            if (bitary[i] == true) stringBitArray += "1";
            else stringBitArray += "0";
        }

        return stringBitArray;
    }

    private BitArray LeftRotate(BitArray inputArray ,int count) // count��ŭ �������� �̵�, 0��° ��Ʈ�� ������ ��Ʈ�� �̵� 
    {
        BitArray tmp1 = new BitArray(5);
        BitArray tmp2 = new BitArray(5);

        int len = inputArray.Length / 2;

        for (int i = 0; i < 5; i++) // 5��Ʈ�� �ڸ�
        {
            tmp1[i] = inputArray[i];
            tmp2[i] = inputArray[5 + i];
        }

        for (int i=0; i<len; i++)
        {
            tmp1[i] = inputArray[(i + count) % len];
            tmp2[i] = inputArray[len + ((i + count) % len)];
        }

        BitArray tmpArray = new BitArray(10);

        for (int i = 0; i < 5; i++) // �ڸ��� �ٽ� ������
        {
            tmpArray[i] = tmp1[i];
            tmpArray[5 + i] = tmp2[i];
        }

        print(BitArrayToString(tmpArray));
        return tmpArray;
    }

    public void tryGenerateKey()
    {
        StringKeyToBitArray(); // �ؽ�Ʈ�� �Է¹��� ���� BitArray�� ��ȯ
        if(intKey[0] >= 0 && intKey[0] <= 1023) // Ű���� ������ 0~1023�̸� Ű ����
        {
            GenerateKey();
        }
        else
        {
            print("Ű�� ����: 0~1023");
        }
    }

    private void GenerateKey() //key�� �������� key1�� key2 ����
    {
        P10 = GetP10(); // P10 ����
        print("P10:"+BitArrayToString(P10));

        key1 = GetP8(LeftRotate(P10, 1)); // P8(LS(P10(key)))
        key2 = GetP8(LeftRotate(LeftRotate(P10, 1), 2)); // P8(LS(LS(P10(key))))

        print("Key1:" + BitArrayToString(key1));
        print("Key2:" + BitArrayToString(key2));
    }
}
