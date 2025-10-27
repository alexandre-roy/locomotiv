public abstract class User
{
    private int _id;
    public int Id // Clé primaire
    {
        get;

        set
        {
            _id = value;
        }
    }

    private string _prenom;
    public string Prenom
    {
        get;

        set
        {
            _prenom = value;
        }
    }

    private string _nom;
    public string Nom
    {
        get;

        set
        {
            _nom = value;
        }
    }

    private string _username;
    public string Username
    {
        get;

        set
        {
            _username = value;
        }
    }

    private string _password;
    public string Password
    {
        get;

        set
        {
            _password = value;
        }
    }

    protected User(int id, string prenom, string nom, string username, string password)
    {
        Id = id;
        Prenom = prenom;
        Nom = nom;
        Username = username;
        Password = password;
    }
}
