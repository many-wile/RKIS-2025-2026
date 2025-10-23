using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
    {
        internal class Profile
        {
            private string firstName;
            private string lastName;
            private int birthYear;

            public Profile(string firstName, string lastName, int birthYear)
            {
                this.firstName = firstName;
                this.lastName = lastName;
                this.birthYear = birthYear;
            }
            public string FirstName => firstName;
            public string LastName => lastName;
            public int BirthYear => birthYear;
            public string GetInfo()
            {
                int age = DateTime.Today.Year - birthYear;
                return $"{firstName} {lastName}, возраст {age}";
            }
        }
    }


