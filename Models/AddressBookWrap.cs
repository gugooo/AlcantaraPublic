using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alcantara.Models
{
    public class AddressBookWrap
    {
        public IEnumerable<AddressBook> Addresses { get; set; }
        public ViewModels.MyAccount.AddAddressBookViewModel AddNewAddress { get; set; }
    }
}
