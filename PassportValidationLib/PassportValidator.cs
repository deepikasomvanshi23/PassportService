using System;
using System.Globalization;
using PassportValidationLib.Model;

namespace PassportValidationLib
{
    public class PassportValidator
    {
        public static ValidationResults IsValidPassportMrzRow2(Passport passportDetails, DateTime? currentDateTime = null)
        {
            ValidationResults results = new ValidationResults {PassportNumber = passportDetails.PassportNumber};

            currentDateTime = currentDateTime.HasValue ? currentDateTime : DateTime.Now;
            var row2Obj = new PassportNumberRow2(passportDetails.MrzRow2);

            DateTime row2ExpiryDate;
            var cultureInfo = new CultureInfo("en-GB");
            cultureInfo.Calendar.TwoDigitYearMax = 2030;

            //Passport number cross check
            results.PassportNumberCrossCheck = (passportDetails.PassportNumber == row2Obj.PassportNumber.Value);

            //Expiry date cross check
            DateTime.TryParseExact(row2Obj.ExpirationDate.Value, "yyMMdd", cultureInfo, DateTimeStyles.None,out row2ExpiryDate);
            results.ExpirationDataCrossCheck = (passportDetails.DateOfExpiry == row2ExpiryDate);

            //DOB no cross check
            DateTime row2DateOfBirth;
            DateTime.TryParseExact(row2Obj.DateOfBirth.Value, "yyMMdd", null, DateTimeStyles.None, out row2DateOfBirth);
            results.DateOfBirthCrossCheck = (passportDetails.DateOfBirth == row2DateOfBirth);

            //Nationality cross check
            results.NationalityCrossCheck = (string.IsNullOrWhiteSpace(passportDetails.Nationality) || passportDetails.Nationality == row2Obj.Nationality.Value);

            //Gender cross check
            results.GenderCrossCheck = (passportDetails.Gender.StartsWith(row2Obj.Sex.Value));

            //Validate all cross check digits
            row2Obj.ValidateCheckDigits(currentDateTime.Value, results);

            return results;
        }
    }
}
