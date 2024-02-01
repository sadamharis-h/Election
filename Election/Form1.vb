Imports System.Data.OracleClient
Public Class Form1

    Public cmd As OracleCommand
    Public reader As OracleDataReader
    Public Conn_string As String

    Public Class ElectionCommisionForm
        Dim connectionString As String = ""

        Private Sub ManageMPSeats()
            Using connection As New OracleConnection(connectionString)
                connection.Open()

                Using cmd = New OracleCommand("SELECT state,MPSeats from MPSeats", connection)
                    Using reader = cmd.ExecuteReader
                        While reader.Read
                            Dim state As String = reader("State").ToString()
                            Dim mpSeats As Integer = Convert.ToInt32(reader("MPSeats"))
                            Console.WriteLine("State:" & state & ", MP Seats:" & mpSeats)
                        End While

                        reader.Close()
                        connection.Close()

                    End Using
                End Using
            End Using
        End Sub

        Private Sub RegisterParty(partyName As String, partySymbol As String)
            Using connection As New OracleConnection(connectionString)
                connection.Open()

                Using cmd = New OracleCommand("INSERT INTO Parties(PartyName, PartySymbol) VALUES(:PartyName, :PartySymbol)", connection)
                    cmd.Parameters.AddWithValue(":PartyName", partyName)
                    cmd.Parameters.AddWithValue(":PartySymbol", partySymbol)
                    cmd.ExecuteNonQuery()

                    connection.Close()
                End Using
            End Using
        End Sub

        Private Sub ManageVoterList(name As String, address As String, photo As Byte())
            Using connection As New OracleConnection(connectionString)
                connection.Open()

                Using cmd = New OracleCommand("INSERT INTO Voters(Name, Address, Photo) VALUES(:Name, :Address, :Photo)", connection)
                    cmd.Parameters.AddWithValue(":Name", name)
                    cmd.Parameters.AddWithValue(":Address", address)
                    cmd.Parameters.AddWithValue(":Photo", photo)

                    cmd.ExecuteNonQuery()

                    connection.Close()
                End Using
            End Using
        End Sub

        Private Sub RegisterCandidate(name As String, partyID As Integer, state As String)
            Using connection As New OracleConnection(connectionString)
                connection.Open()

                Using cmd = New OracleCommand("INSERT INTO Candidates(Name, PartyID, State) VALUES(:Name, :PartyID, :State)", connection)
                    cmd.Parameters.AddWithValue(":Name", name)
                    cmd.Parameters.AddWithValue(":PartyID", partyID)
                    cmd.Parameters.AddWithValue(":State", state)

                    cmd.ExecuteNonQuery()

                    connection.Close()
                End Using
            End Using
        End Sub

        Private Sub HandleVotingSystem(voterID As Integer, candidateID As Integer)
            Using connection As New OracleConnection(connectionString)
                connection.Open()

                Using cmd = New OracleCommand("SELECT COUNT(*) FROM Votes WHERE VoterID=:VoterID", connection)
                    cmd.Parameters.AddWithValue(":VoterID", voterID)
                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                    If count = 0 Then
                        Using cmd1 = New OracleCommand("INSERT INTO Votes(VoterID, CandidateID) VALUES(:VoterID, :CandidateID)", connection)
                            cmd1.Parameters.AddWithValue(":VoterID", voterID)
                            cmd1.Parameters.AddWithValue(":CandidateID", candidateID)

                            cmd1.ExecuteNonQuery()

                        End Using
                    Else
                        MessageBox.Show("You have already voted")
                    End If

                    connection.Close()

                End Using
            End Using
        End Sub

        Private Sub ViewElectioResult()
            Using connection As New OracleConnection(connectionString)
                connection.Open()

                Using cmd = New OracleCommand("SELECT Parties.PartyName, COUNT(*) as VoteCount FROM Votes INNER JOIN Candidates ON Votes.CandidateID=Candidates.CandidateID INNER JOIN Parties ON Candidates.PartyID = Parties.PartyID GROUPBY Parties.PartyName", connection)
                    Using reader = cmd.ExecuteReader
                        While reader.Read
                            Dim partyName As String = reader("PartyName").ToString()
                            Dim voteCount As Integer = Convert.ToInt32(reader("VoteCount"))
                            Console.WriteLine("Party:" & partyName & ", VoteCount:" & voteCount)
                        End While

                        reader.Close()
                        connection.Close()

                    End Using
                End Using
            End Using
        End Sub
    End Class




End Class
