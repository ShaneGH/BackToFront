using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utils
{
    public class ValidationContext
    {
        public Mocks Mocks { get; set; }

        public ValidationContext()
        {
            Mocks = new Mocks();
        }

        /// <summary>
        /// Copies all properties of the validation context down to 1 level
        /// </summary>
        /// <returns></returns>
        public ValidationContext Copy()
        {
            return new ValidationContext { Mocks = new Utils.Mocks(Mocks) };
        }
    }
}
