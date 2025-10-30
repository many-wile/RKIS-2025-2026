using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList
{
    internal class ProfileCommand : ICommand
    {
        private Profile profile;

        public ProfileCommand(Profile profile)
        {
            this.profile = profile;
        }

        public void Execute()
        {
            Console.WriteLine(profile.GetInfo());
        }
    }
}

