Imports System.IO
Imports System.Data.SqlClient
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows.Media

Public Class ImageViewer

    Private Sub ImageViewer_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If ConnectionState.Open Then
            conn.Close()
        End If
    End Sub

    Private Sub ImageViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If ConnectToServer() Then
            'MyDataGridView.DataSource = Nothing : Dim dst = New DataSet
            'Dim msda = New SqlDataAdapter("select  (cast([image] as binary))  as img from cif.dbo.CIFImages where cifkey='1000000026' and imagetype='Photo';", conn)
            'msda.Fill(dst, 0)
            'MyDataGridView.DataSource = dst.Tables(0)
        End If
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ShowImage1()
        'Dim cmd As New SqlCommand("select cifkey, [image]  as img from cif.dbo.CIFImages where cifkey='1000000026' and imagetype='Photo';", conn)
        'Dim dt As New DataTable
        'Dim adp As New SqlDataAdapter(cmd)
        'adp.Fill(dt)

        'Dim imgByte() As Byte
        'If dt.Rows(0)("img").ToString IsNot Nothing Then 'See Rows(0)


        '    ToImage(dt.Rows(0)("img"))


        'End If

        'Dim com As New SqlCommand("select [image] as img from cif.dbo.CIFImages where cifkey='1000000026' and imagetype='Photo'", conn)
        'Dim dr As SqlDataReader
        'dr = com.ExecuteReader(CommandBehavior.CloseConnection)
        'If dr.Read Then
        '    Dim NewImage As Image
        '    Dim mgBytes() As Byte = CType(dr("img"), Byte())
        '    Dim Stream = New MemoryStream(mgBytes, 0, mgBytes.Length)
        '    'Dim returnImage As System.Drawing.Image = System.Drawing.Image.FromStream(Stream, True, True)
        '    If mgBytes.GetUpperBound(0) > 0 Then
        '        Dim ImageStream = New MemoryStream(mgBytes)
        '        NewImage = Image.FromStream(ImageStream)
        '    Else
        '        NewImage = Nothing
        '    End If
        'End If
        'dr.Close()


    End Sub

    Public Shared Function ToImage(byteArrayIn As Byte()) As Image
       ' Dim myArray = DirectCast(New ImageConverter().ConvertTo(byteArrayIn, GetType(Byte())), Byte())
        Dim ms As New MemoryStream(New [Byte]() {&H0, &H1, &H2})
        ms = New MemoryStream(byteArrayIn)
        Dim returnImage As System.Drawing.Image = System.Drawing.Image.FromStream(ms, False, True)
        Return returnImage
    End Function

    Public Function byteArrayToImage(byteArrayIn As Byte()) As Image

        Dim converter As New System.Drawing.ImageConverter()
        Dim img As Image = DirectCast(converter.ConvertFrom(byteArrayIn), Image)
        Return img
    End Function

    Public Sub ShowImage()
        Try
            Dim cmdSelect As New SqlCommand("select [image]  as img from cif.dbo.CIFImages where cifkey=@ID and imagetype='Photo';", conn)
            cmdSelect.Parameters.Add("@ID", SqlDbType.Int, 4)
            cmdSelect.Parameters("@ID").Value = cif.Text

            Dim barrImg As Byte() = DirectCast(cmdSelect.ExecuteScalar(), Byte())
            Dim strfn As String = Convert.ToString(DateTime.Now.ToFileTime())
            Dim fs As New FileStream(cif.Text, FileMode.CreateNew, FileAccess.Write)
            fs.Write(barrImg, 0, barrImg.Length)

            fs.Flush()
            'fs.Close()
            ' conn.Close()

            Dim bitmap As New Bitmap(fs)
            PictureBox1.BackgroundImage = bitmap
            PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
            PictureBox1.Image = Image.FromFile(strfn)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
        End Try
    End Sub
    Public Sub ShowImage1()
        Try
            Dim cmd As New SqlCommand("select [image] as img from cif.dbo.CIFImages where cifkey=@ID and imagetype='Signature';", conn)
            cmd.Parameters.Add("@ID", SqlDbType.Int, 4)
            cmd.Parameters("@ID").Value = cif.Text
            Dim reader As SqlDataReader = cmd.ExecuteReader()
            Dim obj As Object, B() As Byte

            If reader.HasRows Then
                reader.Read()
                Dim imgInBytes() As Byte = CType(reader.GetValue(0), Byte())
                File.WriteAllBytes(cif.Text, imgInBytes)
            End If
            reader.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles view.Click
        'Dim img As Byte() = File.ReadAllBytes("D:\" & cif.Text)


        Dim raw As Byte() = File.ReadAllBytes(cif.Text)
        Using img As Image = Image.FromStream(New MemoryStream(raw))
            img.Save("d:\foo.jpg", ImageFormat.Jpeg)
        End Using

        ''Dim jpegBytes As [Byte]()
        ''Using inStream As New MemoryStream(img)
        ''    Using outStream As New MemoryStream()
        ''        System.Drawing.Bitmap.FromStream(inStream).Save(outStream, System.Drawing.Imaging.ImageFormat.Jpeg)
        ''        jpegBytes = outStream.ToArray()
        ''    End Using
        ''End Using


        ''Dim fileData As Byte() = File.ReadAllBytes("D:\" & cif.Text)
        ''Using binaryReader = New BinaryReader(Request.Files(0).InputStream)
        ''    fileData = binaryReader.ReadBytes(Request.Files(0).ContentLength)
        ''End Using


        'Dim _fileInfo As Image("D:\" & cif.Text)
        'Dim objects As Object = _fileInfo
        'Dim imageConverter As ImageConverter = New System.Drawing.ImageConverter()
        'Dim image As System.Drawing.Image = TryCast(imageConverter.ConvertFrom(objects), System.Drawing.Image)
        'image.Save("d:\img.jpg", System.Drawing.Imaging.ImageFormat.Jpeg)

    End Sub
   

    Public Function ConvertImageFiletoBytes(ByVal ImageFilePath As String) As Byte()
        Dim _tempByte() As Byte = Nothing
        If String.IsNullOrEmpty(ImageFilePath) = True Then
            Throw New ArgumentNullException("Image File Name Cannot be Null or Empty", "ImageFilePath")
            Return Nothing
        End If
        Try
            Dim _fileInfo As New IO.FileInfo(ImageFilePath)
            Dim _NumBytes As Long = _fileInfo.Length
            Dim _FStream As New IO.FileStream(ImageFilePath, IO.FileMode.Open, IO.FileAccess.Read)
            Dim _BinaryReader As New IO.BinaryReader(_FStream)
            _tempByte = _BinaryReader.ReadBytes(Convert.ToInt32(_NumBytes))
            _fileInfo = Nothing
            _NumBytes = 0
            _FStream.Close()
            _FStream.Dispose()
            _BinaryReader.Close()
            ConvertBytesToImageFile(_tempByte, ImageFilePath)
            Return _tempByte
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function ConvertBytesToImageFile(ByVal ImageData As Byte(), ByVal FilePath As String)

        Try
            Dim fs As IO.FileStream = New IO.FileStream("d:\test", IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
            Dim bw As IO.BinaryWriter = New IO.BinaryWriter(fs)
            bw.Write(ImageData)
            Dim image As Image = System.Drawing.Image.FromStream(fs)
            bw.Flush()
            bw.Close()
            fs.Close()



            bw = Nothing
            fs.Dispose()
            ' Dim bitmap As New Bitmap("d:\test")
            PictureBox1.Image = image
        Catch ex As Exception
        End Try
    End Function

    Public Function ConvertBytesToMemoryStream(ByVal ImageData As Byte()) As IO.MemoryStream
        Try
            If IsNothing(ImageData) = True Then
                Return Nothing
                'Throw New ArgumentNullException("Image Binary Data Cannot be Null or Empty", "ImageData")
            End If
            Return New System.IO.MemoryStream(ImageData)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function



    ''' <summary>
    ''' Converts the Image File to Memory Stream
    ''' </summary>
    ''' <param name="ImageFilePath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConvertImageFiletoMemoryStream(ByVal ImageFilePath As String) As IO.MemoryStream
        If String.IsNullOrEmpty(ImageFilePath) = True Then
            Return Nothing
            ' Throw New ArgumentNullException("Image File Name Cannot be Null or Empty", "ImageFilePath")
        End If
        Return ConvertBytesToMemoryStream(ConvertImageFiletoBytes(ImageFilePath))
    End Function
End Class
