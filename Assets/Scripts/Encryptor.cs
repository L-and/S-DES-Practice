using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Encryptor : MonoBehaviour
{
    public static Encryptor Instance = null; // 싱글톤

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

    public KeyGenerator keyGenerator;

    public string plainText; // 평문

    public byte[] plainTextByte;

    public byte[] cipherTextByte;

    public static string BitArrayToString(BitArray bitary)
    {
        string stringBitArray = "";

        for (int i = 0; i < bitary.Length; i++)
        {
            if (bitary[i] == true) stringBitArray += "1";
            else stringBitArray += "0";
        }

        return stringBitArray;
    }

    public static BitArray IP(BitArray text,bool isReverse) // 8비트 평문을 받아 치환해줌(isReverse는 역함수로 사용여부)
    {
        int len = text.Length;

        BitArray result = new BitArray(len);

        int[] IPMap = { 2, 6, 3, 1, 4, 8, 5, 7 };
        int[] reverseIPMap = { 4, 1, 3, 5, 7, 2, 8, 6 };

        if(!isReverse) // 역함수 X
        {
            for (int i = 0; i < len; i++)
            {
                result[i] = text[IPMap[i] - 1];
            }
        }
        else // 역함수 O
        {
            for (int i = 0; i < len; i++)
                result[i] = text[reverseIPMap[i] - 1];
        }

        return result;
    }

    public static BitArray F(BitArray front4Bit, BitArray back4Bit, BitArray subKey) // 4비트를 받아 서브키와 XOR
    {
        int[] EP = { 4, 1, 2, 3, 2, 3, 4, 1 }; // EP 테이블

        int[,] SBox1 = { { 1, 0, 3, 2 }, { 3, 2, 1, 0 }, { 0, 2, 1, 3 }, { 3, 1, 3, 2 } };
        int[,] SBox2 = { { 0, 1, 2, 3 }, { 2, 0, 1, 3 }, { 3, 0, 1, 0 }, { 2, 1, 0, 3 } };

        int[] P4Map = { 2, 4, 3, 1 };

        BitArray tmpR = new BitArray(8);

        for(int i=0; i<EP.Length; i++) // EP테이블에 맞게 전치
        {
            tmpR[i] = back4Bit[EP[i] - 1];
        }

        BitArray P = tmpR.Xor(subKey); // subKey와 XOR

        //print("P: "+BitArrayToString(P));

        int[] SBoxRow = new int[2]; // S박스의 행
        int[] SBoxColumn = new int[2]; // S박스의 열

        // S-Box를 사용하여 치환

        SBoxRow[0] = Instance.GetIntFrom2Bit(P[0], P[3]); // SBox의 행 (앞 4비트)
        SBoxColumn[0] = Instance.GetIntFrom2Bit(P[1], P[2]); // SBox의 열


        SBoxRow[1] = Instance.GetIntFrom2Bit(P[4], P[7]); //SBox의 행 (뒤 4비트)
        SBoxColumn[1] = Instance.GetIntFrom2Bit(P[5], P[6]); // SBox의 열


        int[] tmpInt = new int[1];

        tmpInt[0] = SBox1[SBoxRow[0], SBoxColumn[0]]; // SBox를 사용해 Int값을 얻은 후

        BitArray tmp1 = new BitArray(tmpInt); // BitArray로 변환
        tmpInt[0] = SBox2[SBoxRow[1], SBoxColumn[1]];

        BitArray tmp2 = new BitArray(tmpInt);

        BitArray bitArray = new BitArray(4); // tmp1과 tmp2를 합쳐줌
        bitArray[0] = tmp1[0];
        bitArray[1] = tmp1[1];
        bitArray[2] = tmp2[0];
        bitArray[3] = tmp2[1];

        BitArray tmpBitArray = new BitArray(4);

        for(int i=0; i<4; i++)
        {
            tmpBitArray[i] = bitArray[P4Map[i] - 1];
        }

        return tmpBitArray.Xor(front4Bit); // R의 앞의 4비트와 XOR한 결과를 Return
    }

    public int GetIntFrom2Bit(bool bit1, bool bit2)
    {
        if (bit1)
            if (bit2)
                return 3; // 11
            else
                return 2; // 10
        else
            if (bit2)
            return 1; // 01
        else
            return 0; // 00
    }

    private byte Encryption(BitArray tmpBitArray) // 8비트를 입력받아 암호화
    {
        tmpBitArray = IP(tmpBitArray, false); // IP 실행

        //print("IP:" + BitArrayToString(tmpBitArray));

        BitArray front4Bit = new BitArray(4); // 앞의 4비트
        BitArray back4Bit = new BitArray(4); // 뒤의 4비트

        for (int i = 0; i < 4; i++) // 앞의 4비트값 지정
            front4Bit[i] = tmpBitArray[i];

        for (int i = 0; i < 4; i++) // 뒤의 4비트값 지정
            back4Bit[i] = tmpBitArray[4 + i];

        BitArray F1 = F(front4Bit, back4Bit, keyGenerator.key1); // 뒤의 4비트로 F(R, sk)함수 실행
        BitArray F2 = F(back4Bit, F1, keyGenerator.key2); // 비트 위치를 스위치 후 F함수 한번더 실행

        BitArray resultBitArray = new BitArray(8);
        for (int i = 0; i < 4; i++) // 결과값 만들기
        {
            resultBitArray[i] = F2[i];
            resultBitArray[4 + i] = F1[i];
        }

        resultBitArray = IP(resultBitArray, true);

        byte[] bytes = new byte[1];
        resultBitArray.CopyTo(bytes, 0);
        return bytes[0];
    }

    private void ConvertPlainTextToByte()
    {
       plainTextByte = Encoding.UTF8.GetBytes(plainText);
    }

    public void EncryptionString()
    {
        plainText = TextFieldManager.GetText();

        ConvertPlainTextToByte();

        cipherTextByte = new byte[plainTextByte.Length];

        int[] intByte = new int[1];

        for (int i=0; i< plainTextByte.Length; i++)
        {
            intByte[0] = plainTextByte[i];

            BitArray tmpBitArray = new BitArray(intByte);
            tmpBitArray.Length = 8; 
            cipherTextByte[i] = Encryption(tmpBitArray);
        }

        TextFieldManager.SetCipherTextField(Encoding.UTF8.GetString(cipherTextByte)); // 암호문출력창에 암호문을 출력
    }
}
