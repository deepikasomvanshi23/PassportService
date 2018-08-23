using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PassportValidationLib
{
    internal abstract class PassportNumberRow
    {
        public const int ROW_LENGTH = 44;

        public string FullRowString { get; set; }
        public Dictionary<string, PassportNumberSection> Sections { get; set; }
        public List<string> ValidationErrors { get; set; }

        public virtual bool Validate(DateTime currentDateTime)
        {
            ValidationErrors = new List<string>();

            if (FullRowString.Length != ROW_LENGTH)
                ValidationErrors.Add(string.Format("The Passport MRZ number provided is an incorrect length. It must be {0} characters in length including any padding characters i.e. <", ROW_LENGTH));

            return ValidationErrors.Count == 0;
        }

        public bool ValidateCheckDigit(string checkStr, string checkDigit, string propertyName)
        {
            int calculatedCheckDigit = CalculateCheckDigit(checkStr);
            if (calculatedCheckDigit == 999)
            {
                bool result = (checkDigit == "<" || checkDigit == "0");
                if (!result)
                    ValidationErrors.Add("The Passport Number provided is not valid. Please correct the number before trying again.");
               
                return result;
            }

            if (calculatedCheckDigit.ToString() != checkDigit)
            {
                ValidationErrors.Add("The Passport Number provided is not valid. Please correct the number before trying again.");
                ValidationErrors.Add(string.Format("{0} Check Digit Mismatch. Digit Supplied: {1}, Digit Calculated {2}", propertyName, checkDigit, calculatedCheckDigit));
                return false;
            }
            return true;
        }

        private int CalculateCheckDigit(string s)
        {
            if (s.Distinct().Count() == 1 && s[0] == '<')
                return 999;

            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                var value = GetCheckValue(s[i]);
                var weight = GetCheckWeight(i);
                count += value * weight;
            }
            return count % 10;
        }

        private static int GetCheckValue(char c)
        {
            // The padding character has a value of 0
            if (c == '<')
                return 0;

            // Numbers have their own value
            int result;
            if (int.TryParse(c.ToString(), out result))
                return result;

            if (!char.IsLetter(c))
                throw new ArgumentException("The character supplied is not a number, a letter or the character <. Any other character is not permitted.");

            var index = char.ToUpper(c) - 64;
            return index + 9;
        }

        private static int GetCheckWeight(int position)
        {
            var remainder = position % 3;
            switch (remainder)
            {
                case 0:
                    return 7;
                case 1:
                    return 3;
                case 2:
                    return 1;
            }
            throw new Exception(string.Format("Failed to assign a check weight. position: {0}, remainder: {1}", position, remainder));
        }
    }

    internal class PassportNumberRow2 : PassportNumberRow
    {
        private const string PassportNumberIndex = "PassportNumber";
        private const string NationalityIndex = "Nationality";
        private const string PassportNumberCheckDigitIndex = "PassportNumberCheckDigit";
        private const string DateOfBirthIndex = "DateOfBirth";
        private const string DateOfBirthCheckDigitIndex = "DateOfBirthCheckDigit";
        private const string SexIndex = "Sex";
        private const string ExpirationDateIndex = "ExpirationDate";
        private const string ExpirationDateCheckDigitIndex = "ExpirationDateCheckDigit";
        private const string PersonalNumberIndex = "PersonalNumber";
        private const string PersonalNumberCheckDigitIndex = "PersonalNumberCheckDigit";
        private const string GlobalCheckDigitIndex = "GlobalCheckDigit";

        #region Properties

        public PassportNumberSection PassportNumber
        {
            get { try { return Sections[PassportNumberIndex]; } catch { return null; } }
            private set { Sections[PassportNumberIndex] = value; }
        }

        public PassportNumberSection PassportNumberCheckDigit
        {
            get { try { return Sections[PassportNumberCheckDigitIndex]; } catch { return null; } }
            private set { Sections[PassportNumberCheckDigitIndex] = value; }
        }

        public PassportNumberSection Nationality
        {
            get { try { return Sections[NationalityIndex]; } catch { return null; } }
            private set { Sections[NationalityIndex] = value; }
        }

        public PassportNumberSection DateOfBirth
        {
            get { try { return Sections[DateOfBirthIndex]; } catch { return null; } }
            private set { Sections[DateOfBirthIndex] = value; }
        }

        public PassportNumberSection DateOfBirthCheckDigit
        {
            get { try { return Sections[DateOfBirthCheckDigitIndex]; } catch { return null; } }
            private set { Sections[DateOfBirthCheckDigitIndex] = value; }
        }

        public PassportNumberSection Sex
        {
            get { try { return Sections[SexIndex]; } catch { return null; } }
            private set { Sections[SexIndex] = value; }
        }

        public PassportNumberSection ExpirationDate
        {
            get { try { return Sections[ExpirationDateIndex]; } catch { return null; } }
            private set { Sections[ExpirationDateIndex] = value; }
        }

        public PassportNumberSection ExpirationDateCheckDigit
        {
            get { try { return Sections[ExpirationDateCheckDigitIndex]; } catch { return null; } }
            private set { Sections[ExpirationDateCheckDigitIndex] = value; }
        }

        public PassportNumberSection PersonalNumber
        {
            get { try { return Sections[PersonalNumberIndex]; } catch { return null; } }
            private set { Sections[PersonalNumberIndex] = value; }
        }

        public PassportNumberSection PersonalNumberCheckDigit
        {
            get { try { return Sections[PersonalNumberCheckDigitIndex]; } catch { return null; } }
            private set { Sections[PersonalNumberCheckDigitIndex] = value; }
        }
        public PassportNumberSection GlobalCheckDigit
        {
            get { try { return Sections[GlobalCheckDigitIndex]; } catch { return null; } }
            private set { Sections[GlobalCheckDigitIndex] = value; }
        }

        #endregion

        #region Constructors

        public PassportNumberRow2(string rowStr)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rowStr) || rowStr.Length != ROW_LENGTH)
                    return;

                var number = rowStr.Substring(0, 9);
                var numberCheckDigit = rowStr.Substring(9, 1);
                var nationality = rowStr.Substring(10, 3);
                var dateOfBirth = rowStr.Substring(13, 6);
                var dateOfBirthCheckDigit = rowStr.Substring(19, 1);
                var sex = rowStr.Substring(20, 1);
                var expirationDate = rowStr.Substring(21, 6);
                var expirationDateCheckDigit = rowStr.Substring(27, 1);
                var personalNumber = rowStr.Substring(28, 14);
                var personalNumberCheckDigit = rowStr.Substring(42, 1);
                var globalCheckDigit = rowStr.Substring(43, 1);

                Init(number, numberCheckDigit, nationality, dateOfBirth, dateOfBirthCheckDigit, sex,
                     expirationDate, expirationDateCheckDigit, personalNumber, personalNumberCheckDigit, globalCheckDigit);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Error in PassportNumberRow2: The string argument is invalid.", "rowStr", e);
            }
        }

        public PassportNumberRow2(string number, string numberCheckDigit, string nationality, string dateOfBirth, string dateOfBirthCheckDigit, string sex,
                                  string expirationDate, string expirationDateCheckDigit, string personalNumber, string personalNumberCheckDigit, string globalCheckDigit)
        {
            Init(number, numberCheckDigit, nationality, dateOfBirth, dateOfBirthCheckDigit, sex,
                 expirationDate, expirationDateCheckDigit, personalNumber, personalNumberCheckDigit, globalCheckDigit);
        }

        private void Init(string passportNumber, string passportNumberCheckDigit, string nationality, string dateOfBirth, string dateOfBirthCheckDigit, string sex,
                                  string expirationDate, string expirationDateCheckDigit, string personalNumber, string personalNumberCheckDigit, string globalCheckDigit)
        {
            Sections = new Dictionary<string, PassportNumberSection>
            {
                {PassportNumberIndex, new PassportNumberSection(passportNumber, 9, 1, 9)},
                {PassportNumberCheckDigitIndex, new PassportNumberSection(passportNumberCheckDigit, 1, 10, 10)},
                {NationalityIndex, new PassportNumberSection(nationality, 3, 11, 13)},
                {DateOfBirthIndex, new PassportNumberSection(dateOfBirth, 6, 14, 19)},
                {DateOfBirthCheckDigitIndex, new PassportNumberSection(dateOfBirthCheckDigit, 1, 20, 20)},
                {SexIndex, new PassportNumberSection(sex, 1, 21, 21)},
                {ExpirationDateIndex, new PassportNumberSection(expirationDate, 6, 22, 27)},
                {ExpirationDateCheckDigitIndex, new PassportNumberSection(expirationDateCheckDigit, 1, 28, 28)},
                {PersonalNumberIndex, new PassportNumberSection(personalNumber, 14, 29, 42)},
                {PersonalNumberCheckDigitIndex, new PassportNumberSection(personalNumberCheckDigit, 1, 43, 43)},
                {GlobalCheckDigitIndex, new PassportNumberSection(globalCheckDigit, 1, 43, 43)}
            };
            FullRowString = ToPassportString();
        }

        #endregion

        public string ToPassportString()
        {
            return string.Concat(PassportNumber.Value, PassportNumberCheckDigit.Value, Nationality.Value, DateOfBirth.Value, DateOfBirthCheckDigit.Value, Sex.Value, ExpirationDate.Value,
                                 ExpirationDateCheckDigit.Value, PersonalNumber.Value, PersonalNumberCheckDigit.Value, GlobalCheckDigit.Value);
        }

        public bool ValidateCheckDigits(DateTime currentDateTime,ValidationResults results)
        {
            base.Validate(currentDateTime);

            // Validate Dob
            var dob = DateOfBirth.Value;
            DateTime Dob;
            if (!DateTime.TryParseExact(dob, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out Dob))
                ValidationErrors.Add("The Date Of Birth specified is not valid for the Passport Number provided.");

            // Validate Expiration
            var expiration = ExpirationDate.Value;
            DateTime expirationDate;
            var cultureInfo = new CultureInfo("en-GB");
            cultureInfo.Calendar.TwoDigitYearMax = 2030;
            if (!DateTime.TryParseExact(expiration, "yyMMdd", cultureInfo, DateTimeStyles.None, out expirationDate))
                ValidationErrors.Add("The Passport Expiry Date specified is not valid for the Passport Number supplied");

            if (expirationDate < currentDateTime)
                ValidationErrors.Add("The Passport Expiry Date specified is not valid for the Passport Number supplied");

            // Validate Sex
            var sex = Sex.Value.ToUpper();
            if (!(sex == "M" | sex == "F" | sex == "<"))
                ValidationErrors.Add("The Gender specified is not valid for the Passport Number supplied");

            // Validate Check Digits
            results.PassportNumberCheckDigit = ValidateCheckDigit(PassportNumber.Value, PassportNumberCheckDigit.Value, PassportNumberIndex);
            results.DateOfBirthCheckDigit = ValidateCheckDigit(DateOfBirth.Value, DateOfBirthCheckDigit.Value, DateOfBirthIndex);
            results.PassportExpirationDateCheckDigit = ValidateCheckDigit(ExpirationDate.Value, ExpirationDateCheckDigit.Value, ExpirationDateIndex);
            results.PersonalNumberCheckDigit = ValidateCheckDigit(PersonalNumber.Value, PersonalNumberCheckDigit.Value, PersonalNumberIndex);

            // Final Check Digit
            results.FinalCheckDigit = ValidateCheckDigit(String.Concat(PassportNumber.Value, PassportNumberCheckDigit.Value, DateOfBirth.Value, DateOfBirthCheckDigit.Value,
                                            ExpirationDate.Value, ExpirationDateCheckDigit.Value, PersonalNumber.Value, PersonalNumberCheckDigit.Value)
                               , GlobalCheckDigit.Value, GlobalCheckDigitIndex);
            return ValidationErrors.Count == 0;
        }
    }

    //Get Passport details from MRZ
    internal class PassportNumberSection
    {
        public string Value { get; set; }
        public int Length { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }

        public PassportNumberSection(string value, int length, int startPosition, int endPosition)
        {
            if (value.Length > length)
            {
                value = value.Substring(0, length);
            }
            else
            {
                while (value.Length < length)
                {
                    value = value + "<";
                }
            }

            Value = value;
            Length = length;
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
    }
}
