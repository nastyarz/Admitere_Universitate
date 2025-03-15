using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace WindowsFormsApp4
{
    public static class DatabaseHelper
    {
        public static string connectionString = "Data Source=candidati.db;Version=3;";
        public static string ConnectionString
        {
            get { return connectionString; }
        }
        // Inițializare bază de date
        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Creăm tabelele dacă nu există deja
                string createCandidatiTable = @"CREATE TABLE IF NOT EXISTS Candidati (
                                                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                    Nume TEXT,
                                                    Prenume TEXT,
                                                    Domiciliu TEXT,
                                                    FacultateID INTEGER,
                                                    DataNasterii TEXT, 
                                                    NotaMatematica REAL,
                                                    NotaRomana REAL,
                                                    NotaIstorie REAL,
                                                    NotaExamenAles REAL,
                                                    NumeExamenAles TEXT
                                                  )";

                string createFacultatiTable = @"CREATE TABLE IF NOT EXISTS Facultati (
                                                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                    Nume TEXT,
                                                    CodFacultate TEXT
                                                  )";

                string createAdmitereTable = @"CREATE TABLE IF NOT EXISTS Admitere (
                                                    CandidatID INTEGER,
                                                    FacultateID INTEGER,
                                                    MedieAdmitere REAL,
                                                    PRIMARY KEY (CandidatID, FacultateID)
                                                  )";

                using (var cmd = new SQLiteCommand(createCandidatiTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SQLiteCommand(createFacultatiTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SQLiteCommand(createAdmitereTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Adăugarea unei facultăți
        public static void AdaugaFacultate(string numeFacultate, string codFacultate)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertFacultateQuery = @"INSERT INTO Facultati (Nume, CodFacultate) 
                                                VALUES (@Nume, @CodFacultate)";

                using (var cmd = new SQLiteCommand(insertFacultateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Nume", numeFacultate);
                    cmd.Parameters.AddWithValue("@CodFacultate", codFacultate);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Adăugarea unui candidat
        public static int AdaugaCandidat(string nume, string prenume, string domiciliu, int facultateId, DateTime dataNasterii)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertCandidatQuery = @"INSERT INTO Candidati (Nume, Prenume, Domiciliu, FacultateID, DataNasterii) 
                                               VALUES (@Nume, @Prenume, @Domiciliu, @FacultateID, @DataNasterii)";
                using (var cmd = new SQLiteCommand(insertCandidatQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Nume", nume);
                    cmd.Parameters.AddWithValue("@Prenume", prenume);
                    cmd.Parameters.AddWithValue("@Domiciliu", domiciliu);
                    cmd.Parameters.AddWithValue("@FacultateID", facultateId);
                    cmd.Parameters.AddWithValue("@DataNasterii", dataNasterii.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }

                // Returnăm ID-ul candidatului
                string selectCandidatQuery = "SELECT LAST_INSERT_ROWID()";
                using (var cmd = new SQLiteCommand(selectCandidatQuery, connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Obținem candidații
        public static List<Candidat> GetCandidati()
        {
            var candidati = new List<Candidat>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectCandidatiQuery = @"SELECT C.ID, C.Nume, C.Prenume, C.Domiciliu, F.Nume as Facultate, C.DataNasterii, 
                                        C.NotaMatematica, C.NotaRomana, C.NotaIstorie, C.NotaExamenAles, C.NumeExamenAles, 
                                        (C.NotaMatematica + C.NotaRomana + C.NotaIstorie + C.NotaExamenAles) / 4 as MedieAdmitere
                                        FROM Candidati C
                                        JOIN Facultati F ON C.FacultateID = F.ID";
                using (var cmd = new SQLiteCommand(selectCandidatiQuery, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            candidati.Add(new Candidat
                            {
                                ID = reader.GetInt32(0),
                                Nume = reader.GetString(1),
                                Prenume = reader.GetString(2),
                                Domiciliu = reader.GetString(3),
                                Facultate = reader.GetString(4),
                                DataNasterii = reader.GetDateTime(5),
                                NotaMatematica = reader.GetDouble(6),
                                NotaRomana = reader.GetDouble(7),
                                NotaIstorie = reader.GetDouble(8),
                                NotaExamenAles = reader.GetDouble(9),
                                NumeExamenAles = reader.GetString(10),
                                MedieAdmitere = reader.GetDouble(11)  // Modificat pentru a corespunde indexului corect
                            });
                        }
                    }
                }
            }
            return candidati;
        }
        // Obținem facultățile
        public static List<Facultate> GetFacultati()
        {
            var facultati = new List<Facultate>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectFacultatiQuery = "SELECT ID, Nume FROM Facultati";
                using (var cmd = new SQLiteCommand(selectFacultatiQuery, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            facultati.Add(new Facultate
                            {
                                ID = reader.GetInt32(0),
                                Nume = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return facultati;
        }

        public static void SalveazaNote(int candidatId, double notaMatematica, double notaRomana, double notaIstorie, double notaExamenAles, string numeExamenAles)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string updateQuery = @"UPDATE Candidati 
                               SET NotaMatematica = @NotaMatematica, 
                                   NotaRomana = @NotaRomana, 
                                   NotaIstorie = @NotaIstorie,
                                   NumeExamenAles = @NumeExamenAles,
                                   NotaExamenAles = @NotaExamenAles
                               WHERE ID = @CandidatID";

                using (var cmd = new SQLiteCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@NotaMatematica", notaMatematica);
                    cmd.Parameters.AddWithValue("@NotaRomana", notaRomana);
                    cmd.Parameters.AddWithValue("@NotaIstorie", notaIstorie);
                    cmd.Parameters.AddWithValue("@NumeExamenAles", numeExamenAles);
                    cmd.Parameters.AddWithValue("@NotaExamenAles", notaExamenAles);
                    cmd.Parameters.AddWithValue("@CandidatID", candidatId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Obținem candidații admiși pentru o facultate
        public static List<Candidat> GetCandidatiAdmissi(int facultateId)
        {
            var candidati = new List<Candidat>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectCandidatiAdmissiQuery = @"SELECT C.ID, C.Nume, C.Prenume, C.Domiciliu, F.Nume as Facultate, 
                                                      C.NotaMatematica, C.NotaRomana, C.NotaIstorie, 
                                                      (C.NotaMatematica + C.NotaRomana + C.NotaIstorie) / 3 as MedieAdmitere
                                                      FROM Candidati C
                                                      JOIN Facultati F ON C.FacultateID = F.ID
                                                      WHERE C.FacultateID = @FacultateID";
                using (var cmd = new SQLiteCommand(selectCandidatiAdmissiQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@FacultateID", facultateId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            candidati.Add(new Candidat
                            {
                                ID = reader.GetInt32(0),
                                Nume = reader.GetString(1),
                                Prenume = reader.GetString(2),
                                Domiciliu = reader.GetString(3),
                                Facultate = reader.GetString(4),
                                NotaMatematica = reader.GetDouble(5),
                                NotaRomana = reader.GetDouble(6),
                                NotaIstorie = reader.GetDouble(7),
                                MedieAdmitere = reader.GetDouble(8)
                            });
                        }
                    }
                }
            }
            return candidati;
        }
    }

    public class Candidat
    {
        public int ID { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Domiciliu { get; set; }
        public string Facultate { get; set; }
        public DateTime DataNasterii { get; set; }
        public double NotaMatematica { get; set; }
        public double NotaRomana { get; set; }
        public double NotaIstorie { get; set; }
        public string NumeExamenAles { get; set; }
        public double NotaExamenAles { get; set; }
        public double MedieAdmitere { get; set; }
    }

    public class Facultate
    {
        public int ID { get; set; }
        public string Nume { get; set; }
        public string CodFacultate { get; set; }

        // Suprascrierea metodei ToString pentru a afișa doar numele facultății
        public override string ToString()
        {
            return Nume;  // Afișează doar numele facultății în ComboBox
        }
    }
}
