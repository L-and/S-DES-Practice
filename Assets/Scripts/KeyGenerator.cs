using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class KeyGenerator : MonoBehaviour
{
    [SerializeField] [Tooltip("키의 입력필드를 넣어주세요.")]
    TMP_InputField keyInputField;

    int[] intKey = new int[1];

    int[] P10Map = { 3, 5, 2, 7, 4, 10, 1, 9, 8, 6 }; // P10을 생성하기위한 규칙
    int[] P8Map = { 6, 3, 7, 4, 8, 5, 10, 9 };

    BitArray P10 = new BitArray(10);
    BitArray key;

    public BitArray key1;
    public BitArray key2;

    private void StringKeyToBitArray() // String키를 BitArray로 바꿔줌
    {
        intKey[0] = int.Parse(keyInputField.text); // 텍스트필드값을 Int로 변경

        key = new BitArray(intKey); // Int를 BitArray로 변경
    }

    private BitArray GetP10() // 규칙에 맞게 치환하여 P10을 생성하는 함수
    {
        BitArray tmpKey = new BitArray(10);
        key.Length = 10;

        for(int i=0; i<P10.Length; i++)
        {
            tmpKey[i] = key[P10Map[i] - 1];
        }

        return tmpKey;
    }

    private BitArray GetP8(BitArray inputArray) // 규칙에 맞게 치환하여 P8을 생성하는 함수
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

    private BitArray LeftRotate(BitArray inputArray ,int count) // count만큼 왼쪽으로 이동, 0번째 비트는 마지막 비트로 이동 
    {
        BitArray tmp1 = new BitArray(5);
        BitArray tmp2 = new BitArray(5);

        int len = inputArray.Length / 2;

        for (int i = 0; i < 5; i++) // 5비트씩 자름
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

        for (int i = 0; i < 5; i++) // 자른걸 다시 합쳐줌
        {
            tmpArray[i] = tmp1[i];
            tmpArray[5 + i] = tmp2[i];
        }

        print(BitArrayToString(tmpArray));
        return tmpArray;
    }

    public void tryGenerateKey()
    {
        StringKeyToBitArray(); // 텍스트로 입력받은 값을 BitArray로 변환
        if(intKey[0] >= 0 && intKey[0] <= 1023) // 키값의 범위가 0~1023이면 키 생성
        {
            GenerateKey();
        }
        else
        {
            print("키의 범위: 0~1023");
        }
    }

    private void GenerateKey() //key를 바탕으로 key1과 key2 생성
    {
        P10 = GetP10(); // P10 생성
        print("P10:"+BitArrayToString(P10));

        key1 = GetP8(LeftRotate(P10, 1)); // P8(LS(P10(key)))
        key2 = GetP8(LeftRotate(LeftRotate(P10, 1), 2)); // P8(LS(LS(P10(key))))

        print("Key1:" + BitArrayToString(key1));
        print("Key2:" + BitArrayToString(key2));
    }
}
