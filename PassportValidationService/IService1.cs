using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;
using PassportValidationLib;
using PassportValidationLib.Model;

namespace PassportValidationService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        ValidationResults ValidatePassportData(Passport passport);
    }
    
}
