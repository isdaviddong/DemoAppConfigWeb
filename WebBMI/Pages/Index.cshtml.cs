using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace WebBMI.Pages
{
    public class IndexModel : PageModel
    {
        public float BmiResult = 0;

        [BindProperty]
        public int fieldHeight { get; set; }
        [BindProperty]
        public int fieldWeight { get; set; }

        public bool isEnabled = false;
        public readonly IConfiguration Configuration;
        public IndexModel(IConfiguration configuration, IFeatureManager featureManager)
        {
            Configuration = configuration;

            if (featureManager != null)
                isEnabled = featureManager.IsEnabledAsync("toggle01").Result;
        }

        public void OnGet()
        {

        }

        public void OnPostCalculate()
        {
            float height = (float)fieldHeight / 100;
            BmiResult = fieldWeight / (height * height);
        }
    }
}