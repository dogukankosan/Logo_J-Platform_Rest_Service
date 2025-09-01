using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoJ_Platform_Rest_Test.Entities.ImageGenerateModel
{
    public class ImageGenerationInput
    {
        public string Prompt { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float GuidanceScale { get; set; }
        public int NumInferenceSteps { get; set; }
        public int Samples { get; set; } = 1;
    }
}