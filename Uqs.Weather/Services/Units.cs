using System.ComponentModel.DataAnnotations;

namespace Uqs.Weather.Services
{
    public enum Units
    {
        [Display(Name = "standard")]
        Standard,
        [Display(Name = "metric")]
        Metric,
        [Display(Name = "imperial")]
        Imperial
    }
}
