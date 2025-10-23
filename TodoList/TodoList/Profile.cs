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
        public string GetFullName()
        {
            return firstName + " " + lastName;
        }
        public int GetAge()
        {
            return DateTime.Today.Year - birthYear;
        }

        public string GetInfo()
            {
            return $"{GetFullName()}, возраст {GetAge()} лет";
            }
        }
    }


