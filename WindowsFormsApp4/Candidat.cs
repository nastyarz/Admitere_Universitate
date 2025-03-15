public class Candidat
{
    public int ID { get; set; }
    public string Nume { get; set; }
    public string Prenume { get; set; }
    public string Domiciliu { get; set; }
    public double Nota1 { get; set; }
    public double Nota2 { get; set; }
    public double Nota3 { get; set; }
    public double MedieAdmitere { get; set; }
    public int FacultateID { get; set; } // ID-ul facultății la care este înscris candidatul
    public string Facultate { get; set; } // Numele facultății
}
