namespace PassportValidationLib
{
    public class ValidationResults
    {
        public  string PassportNumber { get; set; }
        public  bool PassportNumberCheckDigit { get; set; }
        public  bool DateOfBirthCheckDigit { get; set; }
        public  bool PassportExpirationDateCheckDigit { get; set; }
        public  bool PersonalNumberCheckDigit { get; set; }
        public  bool FinalCheckDigit { get; set; }
        public  bool GenderCrossCheck { get; set; }
        public  bool DateOfBirthCrossCheck { get; set; }
        public  bool ExpirationDataCrossCheck { get; set; }
        public  bool NationalityCrossCheck { get; set; }
        public  bool PassportNumberCrossCheck { get; set; }
    }
}
