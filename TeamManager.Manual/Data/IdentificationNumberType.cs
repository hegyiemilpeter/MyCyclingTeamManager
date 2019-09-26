using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public enum IdentificationNumberType
    {
        [Display(Name = "AKESZ number")]
        AKESZ,
        [Display(Name = "UCI Licence")]
        UCILicence,
        [Display(Name = "Ötpróba number")]
        OtProba,
        [Display(Name = "Triathlon Licence")]
        TriathleteLicence
    }
}