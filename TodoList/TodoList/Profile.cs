﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
{
    internal class Profile
    {
        private string fullName;
        private DateTime birthDate;

        public Profile(string fullName, DateTime birthDate)
        {
            this.fullName = fullName;
            this.birthDate = birthDate;
        }

        public string GetInfo()
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                age--;

            return $"{fullName}, возраст: {age} лет";
        }
    }
}



