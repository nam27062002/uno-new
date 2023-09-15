using System;

namespace _Scripts.BEAN
{
    [Serializable]
    public class User
    {
        public User(string name)
        {
            Name = name;
        }
        public string Name { get; }
        
    }
}