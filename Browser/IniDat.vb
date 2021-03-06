Public Module IniDat
    Public DAT_ErrorMsg As String = "DAT file decryption failed. Please check the password in the INI file."

    'Load DAT file
    Public Function LoadDat() As String
        LoadDat = String.Empty

        Try
            Using fs As New System.IO.FileStream(DatFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)
                Using sr As New System.IO.StreamReader(fs, System.Text.Encoding.Unicode)
                    LoadDat = sr.ReadToEnd
                End Using
            End Using

        Catch ex As Exception
            g_Main.txtMsg.Text = "File loading error:" & ex.ToString
        End Try
    End Function

    'Save DAT file
    Public Sub SaveDat(ByRef cipherText As String)
        Try
            Using fs As New System.IO.FileStream(DatFilePath, System.IO.FileMode.Create, IO.FileAccess.Write)
                Using sw As New System.IO.StreamWriter(fs, System.Text.Encoding.Unicode)
                    sw.Write(cipherText)
                    sw.Flush()
                End Using
            End Using

        Catch ex As Exception
            g_Main.txtMsg.Text = "File saving error:" & ex.ToString
        End Try
    End Sub

    'AES (Advanced Encryption Standard) / Rijndael encryption
    'Code example: http://www.obviex.com/samples/encryption.aspx
    Public Function Encrypt(plainText As String) As String
        Encrypt = String.Empty
        Dim passPhrase As String = Prog_Name & CStr("☼¥£¢¡ῧᾏ♫ﮋ﴾ƀ-‼₪Ω")
        Dim saltValue As String = INI.GetSection(INI_Settings).GetKey(INI_PW).GetValue() & CStr("Ƀ")
        Dim initVector As String = "№βλ¢฿+∞☺" 'Must be 16 bytes
        Dim passwordIterations As Integer = 2
        Dim keySize As Integer = 256

        Dim initVectorBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(initVector)
        Dim saltValueBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(saltValue)
        Dim plainTextBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(plainText)
        plainText = Nothing

        Dim encryptor As Security.Cryptography.ICryptoTransform

        Try
            Using password As New Security.Cryptography.Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations)
                Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)

                Using symmetricKey As New Security.Cryptography.RijndaelManaged()
                    symmetricKey.Mode = Security.Cryptography.CipherMode.CBC
                    encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
                End Using
            End Using

            Using memoryStream As New IO.MemoryStream()
                Using cryptoStream As New Security.Cryptography.CryptoStream(memoryStream, encryptor, Security.Cryptography.CryptoStreamMode.Write)
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)
                    cryptoStream.FlushFinalBlock()
                    Dim cipherTextBytes As Byte() = memoryStream.ToArray()

                    'Encrypted Text
                    Encrypt = Convert.ToBase64String(cipherTextBytes)
                    cipherTextBytes = Nothing
                End Using
            End Using

        Catch ex As Exception
            g_Main.txtMsg.Text = "Encrypt error:" & ex.ToString

        Finally
            initVectorBytes = Nothing
            saltValueBytes = Nothing
            plainTextBytes = Nothing
            encryptor = Nothing
        End Try
    End Function

    Public Function Decrypt(ByRef cipherText As String) As String
        Decrypt = String.Empty
        Dim passPhrase As String = Prog_Name & CStr("☼¥£¢¡ῧᾏ♫ﮋ﴾ƀ-‼₪Ω")
        Dim saltValue As String = INI.GetSection(INI_Settings).GetKey(INI_PW).GetValue() & CStr("Ƀ")
        Dim initVector As String = "№βλ¢฿+∞☺" 'Must be 16 bytes
        Dim passwordIterations As Integer = 2
        Dim keySize As Integer = 256

        Dim initVectorBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(initVector)
        Dim saltValueBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(saltValue)
        Dim cipherTextBytes As Byte() = Convert.FromBase64String(cipherText)
        cipherText = Nothing

        Dim decryptor As Security.Cryptography.ICryptoTransform

        Try
            Using password As New Security.Cryptography.Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations)
                Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)

                Using symmetricKey As New Security.Cryptography.RijndaelManaged()
                    symmetricKey.Mode = Security.Cryptography.CipherMode.CBC
                    decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)
                End Using
            End Using

            Using memoryStream As New IO.MemoryStream(cipherTextBytes)
                Using cryptoStream As New Security.Cryptography.CryptoStream(memoryStream, decryptor, Security.Cryptography.CryptoStreamMode.Read)
                    Dim plainTextBytes As Byte() = New Byte(cipherTextBytes.Length - 1) {}
                    Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)

                    'Plain Text
                    Decrypt = System.Text.Encoding.Unicode.GetString(plainTextBytes, 0, decryptedByteCount)
                    plainTextBytes = Nothing
                End Using
            End Using

        Catch ex As Exception
            g_Main.txtMsg.Text = "Decrypt error:" & ex.ToString

        Finally
            initVectorBytes = Nothing
            saltValueBytes = Nothing
            cipherTextBytes = Nothing
            decryptor = Nothing
        End Try
    End Function
End Module
