using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalIS.Backend
{
    public class Patient : Entity
    {
        public override string ToString()
        {
            return $"Patient{{Id = {Id}}}";
        }
    }
}
