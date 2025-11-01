using System;

namespace TodoList.Commands
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

