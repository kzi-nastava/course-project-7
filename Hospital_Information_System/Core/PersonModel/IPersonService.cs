using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PersonModel
{
    public interface IPersonService
    {
        IEnumerable<Person> GetAll();
        void Add(Person person);
    }
}
