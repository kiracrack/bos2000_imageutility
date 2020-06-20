Imports System.Data.SqlClient

Module Connection
    Public conn As SqlConnection
    Public com As SqlCommand
    Public rst As SqlDataReader

    Public Function ConnectToServer()
        Try
            conn = New SqlConnection
            conn.ConnectionString = "Server=10.100.0.10;Database=cif;User Id=cif;Password=123456cif;"
            conn.Open()
            'com.Connection = conn
            'com.CommandTimeout = 6000000
        Catch errMYSQL As SqlException
            MsgBox(errMYSQL.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
        Return True
    End Function

End Module
