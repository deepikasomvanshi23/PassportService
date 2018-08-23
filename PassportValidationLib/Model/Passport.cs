using System;
using System.ComponentModel.DataAnnotations;

namespace PassportValidationLib.Model
{
    public class Passport
    {
        public Passport()
        {
            
        }
        public string PassportNumber { get; set; }
        
        public string Nationality { get; set; }
        
        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }
        
        public DateTime DateOfExpiry { get; set; }
       
        public string MrzRow2 { get; set; }
    }
}