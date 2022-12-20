using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SilEncConverters40.EcTranslators
{
    public class EncryptionClass
    {
		/*
Imports System
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Security.Cryptography

' Namespace: YourCompany.Utils.Encryption
' Uses DES private key and vector to provide HTTP / XMLDOM - safe base64 string encryption
' Encrypted string such as account info, passwords, etc can be safely placed in XML element
' for transmission over the wire without any illegal characters
' Author: Peter Bromberg
' Date:   3/12/02
' Last Modified: 3/12/02

Public Class Encryption64

    ' Use DES CryptoService with Private key pair
    Private key() As Byte = {} ' we are going to pass in the key portion in our method calls
    Private IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}

    Public Function DecryptFromBase64String(ByVal stringToDecrypt As String, ByVal sEncryptionKey As String) As String
        Dim inputByteArray(stringToDecrypt.Length) As Byte
        ' Note: The DES CryptoService only accepts certain key byte lengths
        ' We are going to make things easy by insisting on an 8 byte legal key length

        Try
            key = System.Text.Encoding.UTF8.GetBytes(Left(sEncryptionKey, 8))
            Dim des As New DESCryptoServiceProvider()
            ' we have a base 64 encoded string so first must decode to regular unencoded (encrypted) string
            inputByteArray = Convert.FromBase64String(stringToDecrypt)
            ' now decrypt the regular string
            Dim ms As New MemoryStream()
            Dim cs As New CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write)
            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
            Return encoding.GetString(ms.ToArray())
        Catch e As Exception
            Return e.Message
        End Try
    End Function

    Public Function EncryptToBase64String(ByVal stringToEncrypt As String, ByVal SEncryptionKey As String) As String
        Try
            key = System.Text.Encoding.UTF8.GetBytes(Left(SEncryptionKey, 8))
            Dim des As New DESCryptoServiceProvider()
            ' convert our input string to a byte array
            Dim inputByteArray() As Byte = Encoding.UTF8.GetBytes(stringToEncrypt)
            'now encrypt the bytearray
            Dim ms As New MemoryStream()
            Dim cs As New CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write)
            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            ' now return the byte array as a "safe for XMLDOM" Base64 String
            Return Convert.ToBase64String(ms.ToArray())
        Catch e As Exception
            Return e.Message
        End Try
    End Function

End Class

#if encryptingNewCredentials
            var specialEncryptionKey = File.ReadAllText(KeyFileName);
            var clientId = EncryptionClass.Encrypt(..., specialEncryptionKey);
            var clientSecret = EncryptionClass.Encrypt(..., specialEncryptionKey);
#endif
        */
		private static byte[] _key;
        private static byte[] _IV = { 0xEB, 0xC9, 0xA3, 0x48, 0x3E, 0xE1, 0x4a, 0x75 };
        private const string strEncryptionKey = "yeh to mai nahii.n huu.n";

        public static string Encrypt(string stringToEncrypt, string specialDecryptionKey = strEncryptionKey)
        {
            try
            {
                _key = Encoding.UTF8.GetBytes(specialDecryptionKey.Substring(0, 8));
                var des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateEncryptor(_key, _IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                // now return the byte array as a "safe for XMLDOM" Base64 String
                return Convert.ToBase64String(ms.ToArray());
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public static string Decrypt(string stringToDecrypt, string specialDecryptionKey = strEncryptionKey)
        {
            try
            {
                _key = Encoding.UTF8.GetBytes(specialDecryptionKey.Substring(0, 8));
                var des = new DESCryptoServiceProvider();

                // we have a base 64 encoded string so first must decode to regular unencoded (encrypted) string
                byte[] inputByteArray = Convert.FromBase64String(stringToDecrypt);
                
                // now decrypt the regular string
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateDecryptor(_key, _IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				return null;
			}
		}
	}
}
