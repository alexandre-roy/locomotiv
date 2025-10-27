using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model
{
    public class Employe : User
    {

        public Employe(): base()
        {
        }

        public Employe(int id, string prenom, string nom, string username, string password) : base(id, prenom, nom, username, password)
        {
            Id = id;
            Prenom = prenom;
            Nom = nom;
            Username = username;
            Password = password;
        }
    }
}
