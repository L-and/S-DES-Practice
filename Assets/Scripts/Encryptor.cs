using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Encryptor : MonoBehaviour
{
    public static Encryptor Instance = null; // �̱���

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

    public string plainText; // ��

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

    public static BitArray IP(BitArray text,bool isReverse) // 8��Ʈ ���� �޾� ġȯ����(isReverse�� ���Լ��� ��뿩��)
    {
        int len = text.Length;

        BitArray result = new BitArray(len);

        int[] IPMap = { 2, 6, 3, 1, 4, 8, 5, 7 };
        int[] reverseIPMap = { 4, 1, 3, 5, 7, 2, 8, 6 };

        if(!isReverse) // ���Լ� X
        {
            for (int i = 0; i < len; i++)
            {
                result[i] = text[IPMap[i] - 1];
            }
        }
        else // ���Լ� O
        {
            for (int i = 0; i < len; i++)
                result[i] = text[reverseIPMap[i] - 1];
        }

        return result;
    }

    public static BitArray F(BitArray front4Bit, BitArray back4Bit, BitArray subKey) // 4��Ʈ�� �޾� ����Ű�� XOR
    {
        int[] EP = { 4, 1, 2, 3, 2, 3, 4, 1 }; // EP ���̺�

        int[,] SBox1 = { { 1, 0, 3, 2 }, { 3, 2, 1, 0 }, { 0, 2, 1, 3 }, { 3, 1, 3, 2 } };
        int[,] SBox2 = { { 0, 1, 2, 3 }, { 2, 0, 1, 3 }, { 3, 0, 1, 0 }, { 2, 1, 0, 3 } };

        int[] P4Map = { 2, 4, 3, 1 };

        BitArray tmpR = new BitArray(8);

        for(int i=0; i<EP.Length; i++) // EP���̺� �°� ��ġ
        {
            tmpR[i] = back4Bit[EP[i] - 1];
        }

        BitArray P = tmpR.Xor(subKey); // subKey�� XOR

        //print("P: "+BitArrayToString(P));

        int[] SBoxRow = new int[2]; // S�ڽ��� ��
        int[] SBoxColumn = new int[2]; // S�ڽ��� ��

        // S-Box�� ����Ͽ� ġȯ

        SBoxRow[0] = Instance.GetIntFrom2Bit(P[0], P[3]); // SBox�� �� (�� 4��Ʈ)
        SBoxColumn[0] = Instance.GetIntFrom2Bit(P[1], P[2]); // SBox�� ��


        SBoxRow[1] = Instance.GetIntFrom2Bit(P[4], P[7]); //SBox�� �� (�� 4��Ʈ)
        SBoxColumn[1] = Instance.GetIntFrom2Bit(P[5], P[6]); // SBox�� ��


        int[] tmpInt = new int[1];

        tmpInt[0] = SBox1[SBoxRow[0], SBoxColumn[0]]; // SBox�� ����� Int���� ���� ��

        BitArray tmp1 = new BitArray(tmpInt); // BitArray�� ��ȯ
        tmpInt[0] = SBox2[SBoxRow[1], SBoxColumn[1]];

        BitArray tmp2 = new BitArray(tmpInt);

        BitArray bitArray = new BitArray(4); // tmp1�� tmp2�� ������
        bitArray[0] = tmp1[0];
        bitArray[1] = tmp1[1];
        bitArray[2] = tmp2[0];
        bitArray[3] = tmp2[1];

        BitArray tmpBitArray = new BitArray(4);

        for(int i=0; i<4; i++)
        {
            tmpBitArray[i] = bitArray[P4Map[i] - 1];
        }

        return tmpBitArray.Xor(front4Bit); // R�� ���� 4��Ʈ�� XOR�� ����� Return
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

    private byte Encryption(BitArray tmpBitArray) // 8��Ʈ�� �Է¹޾� ��ȣȭ
    {
        tmpBitArray = IP(tmpBitArray, false); // IP ����

        //print("IP:" + BitArrayToString(tmpBitArray));

        BitArray front4Bit = new BitArray(4); // ���� 4��Ʈ
        BitArray back4Bit = new BitArray(4); // ���� 4��Ʈ

        for (int i = 0; i < 4; i++) // ���� 4��Ʈ�� ����
            front4Bit[i] = tmpBitArray[i];

        for (int i = 0; i < 4; i++) // ���� 4��Ʈ�� ����
            back4Bit[i] = tmpBitArray[4 + i];

        BitArray F1 = F(front4Bit, back4Bit, keyGenerator.key1); // ���� 4��Ʈ�� F(R, sk)�Լ� ����
        BitArray F2 = F(back4Bit, F1, keyGenerator.key2); // ��Ʈ ��ġ�� ����ġ �� F�Լ� �ѹ��� ����

        BitArray resultBitArray = new BitArray(8);
        for (int i = 0; i < 4; i++) // ����� �����
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

        TextFieldManager.SetCipherTextField(Encoding.UTF8.GetString(cipherTextByte)); // ��ȣ�����â�� ��ȣ���� ���
    }
}
