using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public enum RaceType
    {
        [Display(Name = "Other")]
        Other,
        [Display(Name = "Road race")]
        Road_RoadRace,
        [Display(Name = "Individual time trial")]
        Road_TimeTrial,
        [Display(Name = "MTB XCO")]
        MTB_CrossCountry,
        [Display(Name = "MTB Marathon")]
        MTB_Marathon,
        [Display(Name = "Full distance triathlon")]
        TRI_LongDistance,
        [Display(Name = "Half distance triathlon")]
        TRI_HalfDistance,
        [Display(Name = "Olimpic distance triathlon")]
        TRI_OlimpicDistance,
        [Display(Name = "Sprint distance triathlon")]
        TRI_SprintDistance,
        [Display(Name = "Running")]
        TRI_Running,
        [Display(Name = "Duathlon")]
        TRI_Duathlon
    }
}
