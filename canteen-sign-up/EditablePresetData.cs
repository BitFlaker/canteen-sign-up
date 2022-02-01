using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace canteen_sign_up
{
    [Serializable]
    public class EditablePresetData
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
    }
}