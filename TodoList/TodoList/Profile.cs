using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
    {
        internal class Profile
        {
        private string fullName;
        private int birthYear;

            public Profile(string fullName, int birthYear)
            {
            this.fullName = fullName;
            this.birthYear = birthYear;
            }
        public string GetFullName()
        {
            return fullName;
        }
        public int GetAge()
        {
            return DateTime.Today.Year - birthYear;
        }

        public string GetInfo()
            {
            return $"{fullName}, возраст {GetAge()} лет";
            }
        }
    }


