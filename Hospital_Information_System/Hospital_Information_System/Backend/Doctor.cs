using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalIS.Backend
{
    public class Doctor : Entity
    {
        public override string ToString()
        {
            return $"Doctor{{Id = {Id}}}";
        }
    }
}
