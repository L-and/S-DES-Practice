using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Decryptor : MonoBehaviour
{
    public static Decryptor Instance = null; // 싱글톤

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

    public string cipherText; // 암호문

    public string plainText; // 평문

    public byte[] cipherTextByte;

    public byte[] plainTextByte;

    private byte Decryption(BitArray tmpBitArray) // 8비트씩 복호화
    {
        tmpBitArray = Encryptor.IP(tmpBitArray, false); // IP 실행

        print("IP:" + Encryptor.BitArrayToString(tmpBitArray));

        BitArray front4Bit = new BitArray(4); // 앞의 4비트
        BitArray back4Bit = new BitArray(4); // 뒤의 4비트

        for (int i = 0; i < 4; i++) // 앞의 4비트값 지정
            front4Bit[i] = tmpBitArray[i];

        for (int i = 0; i < 4; i++) // 뒤의 4비트값 지정
            back4Bit[i] = tmpBitArray[4 + i];

        BitArray F1 = Encryptor.F(front4Bit, back4Bit, keyGenerator.key2); // 뒤의 4비트로 F(R, sk)함수 실행
        BitArray F2 = Encryptor.F(back4Bit, F1, keyGenerator.key1); // 비트 위치를 스위치 후 F함수 한번더 실행

        BitArray resultBitArray = new BitArray(8);
        for (int i = 0; i < 4; i++) // 결과값 만들기
        {
            resultBitArray[i] = F2[i];
            resultBitArray[4 + i] = F1[i];
        }

        resultBitArray = Encryptor.IP(resultBitArray, true);

        byte[] bytes = new byte[1];
        resultBitArray.CopyTo(bytes, 0);

        return bytes[0];
    }

    private void ConvertCipherTextToByte()
    {
        cipherTextByte = Encoding.UTF8.GetBytes(cipherText);
    }

    public void DecryptionString()
    {


        //ConvertCipherTextToByte();

        cipherTextByte = Encryptor.Instance.cipherTextByte;

        plainTextByte = new byte[cipherTextByte.Length];

        int[] intByte = new int[1];

        for (int i = 0; i < cipherTextByte.Length; i++)
        {
            intByte[0] = cipherTextByte[i];

            BitArray tmpBitArray = new BitArray(intByte);
            tmpBitArray.Length = 8;
            plainTextByte[i] = Decryption(tmpBitArray);
        }

        TextFieldManager.SetPlainTextField(Encoding.UTF8.GetString(plainTextByte)); // 평문 출력창에 평문 출력
    }
}
