using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Decryptor : MonoBehaviour
{
    public static Decryptor Instance = null; // �̱���

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

    public string cipherText; // ��ȣ��

    public string plainText; // ��

    public byte[] cipherTextByte;

    public byte[] plainTextByte;

    private byte Decryption(BitArray tmpBitArray) // 8��Ʈ�� ��ȣȭ
    {
        tmpBitArray = Encryptor.IP(tmpBitArray, false); // IP ����

        print("IP:" + Encryptor.BitArrayToString(tmpBitArray));

        BitArray front4Bit = new BitArray(4); // ���� 4��Ʈ
        BitArray back4Bit = new BitArray(4); // ���� 4��Ʈ

        for (int i = 0; i < 4; i++) // ���� 4��Ʈ�� ����
            front4Bit[i] = tmpBitArray[i];

        for (int i = 0; i < 4; i++) // ���� 4��Ʈ�� ����
            back4Bit[i] = tmpBitArray[4 + i];

        BitArray F1 = Encryptor.F(front4Bit, back4Bit, keyGenerator.key2); // ���� 4��Ʈ�� F(R, sk)�Լ� ����
        BitArray F2 = Encryptor.F(back4Bit, F1, keyGenerator.key1); // ��Ʈ ��ġ�� ����ġ �� F�Լ� �ѹ��� ����

        BitArray resultBitArray = new BitArray(8);
        for (int i = 0; i < 4; i++) // ����� �����
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

        TextFieldManager.SetPlainTextField(Encoding.UTF8.GetString(plainTextByte)); // �� ���â�� �� ���
    }
}
